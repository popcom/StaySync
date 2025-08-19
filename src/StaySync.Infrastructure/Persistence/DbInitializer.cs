using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StaySync.Domain.Entities;
using StaySync.Domain.ValueObjects;
using System.Security.Cryptography;
using System.Text;
using TimeZoneConverter;

namespace StaySync.Infrastructure.Persistence;

public static class DbInitializer
{
    /// <summary>
    /// Applies migrations and seeds a demo dataset if missing:
    /// - Hotel (API key: "demo-key", timezone Europe/Berlin)
    /// - Rooms: 0101 (2), 0102 (3), 0201 (2)
    /// - TravelGroup A12B34 (arrives today local), 4 travellers (1 unassigned for today)
    /// - RoomAssignments for today: Jane+John -> 0101, Amir -> 0201
    /// </summary>
    public static async Task MigrateAndSeedAsync(this IServiceProvider services, CancellationToken ct = default)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StaySyncDbContext>();
        await db.Database.MigrateAsync(ct);

        // --- Hotel (API key: "demo-key") ---
        var apiKey = "demo-key";
        var apiKeyHash = Sha256HexLower(apiKey);
        var hotel = await db.Hotels.FirstOrDefaultAsync(h => h.ApiKeyHash == apiKeyHash, ct);
        if (hotel is null)
        {
            hotel = new Hotel(
                id: Guid.Parse("11111111-1111-1111-1111-111111111111"),
                name: "Demo Hotel",
                timezone: "Europe/Berlin",
                apiKeyHash: apiKeyHash
            );
            db.Hotels.Add(hotel);
            await db.SaveChangesAsync(ct);
        }

        // Compute hotel-local "today"
        var tz = TZConvert.GetTimeZoneInfo(hotel.Timezone);
        var todayLocal = DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(DateTime.UtcNow, tz).Date);

        // --- Rooms ---
        var roomsWanted = new[]
        {
            new { Code = new RoomCode("0101"), Beds = 2 },
            new { Code = new RoomCode("0102"), Beds = 3 },
            new { Code = new RoomCode("0201"), Beds = 2 }
        };

        var existingCodes = await db.Rooms
            .Where(r => r.HotelId == hotel.Id)
            .Select(r => r.RoomCode.Value)
            .ToListAsync(ct);

        var newRooms = roomsWanted
            .Where(r => !existingCodes.Contains(r.Code.Value))
            .Select(r => new Room(hotel.Id, r.Code, r.Beds))
            .ToList();

        if (newRooms.Count > 0)
        {
            db.Rooms.AddRange(newRooms);
            await db.SaveChangesAsync(ct);
        }

        // Re-load rooms dictionary (code -> entity) for id lookup
        var roomsByCode = await db.Rooms
            .Where(r => r.HotelId == hotel.Id)
            .ToDictionaryAsync(r => r.RoomCode.Value, r => r, ct);

        // --- Travel Group A12B34 ---
        var groupId = new GroupId("A12B34"); // valid: 6 chars, <=2 letters, not starting '0'
        var group = await db.TravelGroups.FirstOrDefaultAsync(
            g => g.HotelId == hotel.Id && g.GroupId == groupId, ct);

        if (group is null)
        {
            group = new TravelGroup(
                hotelId: hotel.Id,
                groupId: groupId,
                arrivalDate: todayLocal,
                travellerCount: 4 // we will add 4 travellers below
            );
            db.TravelGroups.Add(group);
            await db.SaveChangesAsync(ct);
        }

        // --- Travellers (4 total; 1 stays unassigned for "today") ---
        // NOTE: Surname/FirstName are normalized to UPPERCASE by the domain constructor.
        var travellersWanted = new (string Surname, string FirstName, DateOnly Dob)[]
        {
            ("Doe", "Jane", new DateOnly(1990, 5, 1)),
            ("Doe", "John", new DateOnly(1988, 3, 2)),
            ("Ali", "Amir", new DateOnly(1995, 12, 12)),
            ("Doe", "Sara", new DateOnly(1992, 2, 2)) // will be unassigned for today
        };

        // find existing travellers by identity
        var existingTravellers = await db.Travellers
            .Where(t => t.GroupId == group.Id)
            .Select(t => new { t.Id, t.Surname, t.FirstName, t.DateOfBirth })
            .ToListAsync(ct);

        var toAddTravellers = new List<Traveller>();
        foreach (var (surname, first, dob) in travellersWanted)
        {
            var su = surname.Trim().ToUpperInvariant();
            var fu = first.Trim().ToUpperInvariant();
            var exists = existingTravellers.Any(t => t.Surname == su && t.FirstName == fu && t.DateOfBirth == dob);
            if (!exists)
                toAddTravellers.Add(new Traveller(group.Id, new TravellerKey(surname, first, dob)));
        }

        if (toAddTravellers.Count > 0)
        {
            db.Travellers.AddRange(toAddTravellers);
            await db.SaveChangesAsync(ct);
        }

        // Reload travellers map
        var travellers = await db.Travellers
            .Where(t => t.GroupId == group.Id)
            .ToListAsync(ct);

        // --- RoomAssignments for today ---
        // Jane + John -> 0101; Amir -> 0201; Sara -> unassigned
        var jane = travellers.Single(t => t.Surname == "DOE" && t.FirstName == "JANE");
        var john = travellers.Single(t => t.Surname == "DOE" && t.FirstName == "JOHN");
        var amir = travellers.Single(t => t.Surname == "ALI" && t.FirstName == "AMIR");

        var assignmentsWanted = new (string RoomCode, Guid TravellerId)[]
        {
            ("0101", jane.Id),
            ("0101", john.Id),
            ("0201", amir.Id)
        };

        // Check existing assignments for today to avoid duplicates
        var existingAssignments = await db.RoomAssignments
            .Where(a => a.HotelId == hotel.Id && a.AssignedOnDate == todayLocal)
            .Select(a => new { a.RoomId, a.TravellerId })
            .ToListAsync(ct);
        var existingPairs = existingAssignments.ToHashSet();

        foreach (var (roomCode, travellerId) in assignmentsWanted)
        {
            var room = roomsByCode[roomCode];
            var pair = new { RoomId = room.Id, TravellerId = travellerId };
            if (!existingPairs.Contains(pair))
            {
                db.RoomAssignments.Add(new RoomAssignment(
                    hotelId: hotel.Id,
                    roomId: room.Id,
                    travellerId: travellerId,
                    assignedOnDate: todayLocal));
            }
        }

        await db.SaveChangesAsync(ct);
    }

    private static string Sha256HexLower(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
