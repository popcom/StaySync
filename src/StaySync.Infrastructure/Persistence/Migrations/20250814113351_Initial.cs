using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaySync.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Hotels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Timezone = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ApiKeyHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hotels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HotelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomCode = table.Column<string>(type: "char(4)", nullable: false),
                    BedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rooms_Hotels_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TravelGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HotelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<string>(type: "char(6)", nullable: false),
                    ArrivalDate = table.Column<DateOnly>(type: "date", nullable: false),
                    TravellerCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TravelGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TravelGroups_Hotels_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Travellers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Travellers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Travellers_TravelGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "TravelGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoomAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HotelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TravellerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedOnDate = table.Column<DateOnly>(type: "date", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomAssignments_Hotels_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoomAssignments_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoomAssignments_Travellers_TravellerId",
                        column: x => x.TravellerId,
                        principalTable: "Travellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoomAssignments_AssignedOnDate_HotelId",
                table: "RoomAssignments",
                columns: new[] { "AssignedOnDate", "HotelId" });

            migrationBuilder.CreateIndex(
                name: "IX_RoomAssignments_HotelId",
                table: "RoomAssignments",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomAssignments_RoomId_AssignedOnDate_TravellerId",
                table: "RoomAssignments",
                columns: new[] { "RoomId", "AssignedOnDate", "TravellerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoomAssignments_TravellerId",
                table: "RoomAssignments",
                column: "TravellerId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_HotelId_RoomCode",
                table: "Rooms",
                columns: new[] { "HotelId", "RoomCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TravelGroups_HotelId_GroupId",
                table: "TravelGroups",
                columns: new[] { "HotelId", "GroupId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Travellers_GroupId_Surname_FirstName_DateOfBirth",
                table: "Travellers",
                columns: new[] { "GroupId", "Surname", "FirstName", "DateOfBirth" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoomAssignments");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Travellers");

            migrationBuilder.DropTable(
                name: "TravelGroups");

            migrationBuilder.DropTable(
                name: "Hotels");
        }
    }
}
