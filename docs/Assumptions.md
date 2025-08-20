### A-01: TravelGroup belongs to one hotel
- Assumption: Each TravelGroup is owned by exactly one Hotel.
- Rationale: API key scopes all reads/writes; routes & “today” are hotel-local.
- Risk: Multi-property tours would need a higher-level “Tour” later.
- Status: Confirmed.
- Tests/Guards: Unique keys include HotelId; integration tests use API-key tenancy.

### A-02: “Today” uses hotel-local timezone
- Assumption: All “today” queries use the Hotel’s IANA timezone.
- Rationale: Acceptance criteria are hotel-facing; UTC would be wrong at DST edges.
- Risk: If hotel tz is misconfigured, dates could shift.
- Status: Confirmed.
- Tests/Guards: DateOnly usage; TodayOccupancy integration test.

### A-03: Room codes are hotel-scoped, not global
- Assumption: 4-digit codes collide across hotels; uniqueness is per-hotel.
- Rationale: Spec uses only digits; global uniqueness would be brittle.
- Risk: If a cross-hotel dashboard appears, we’ll need composite keys.
- Status: Confirmed.
- Tests/Guards: Unique index (HotelId, RoomCode); endpoint tests.
