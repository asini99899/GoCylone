-- Quick Setup: Add sample routes for testing bus search
-- First check if routes exist
SELECT *
FROM Routes;
-- If no routes exist, add these sample routes for testing:
INSERT INTO Routes (
        FromLocation,
        ToLocation,
        Distance,
        EstimatedTime,
        CreatedAt
    )
VALUES ('Galewela', 'Matale', 45.50, '1h 15m', GETDATE()),
    ('Matale', 'Galewela', 45.50, '1h 15m', GETDATE()),
    ('Galewela', 'Kandy', 85.75, '2h 30m', GETDATE()),
    ('Kandy', 'Matale', 55.25, '1h 45m', GETDATE());
-- Then add sample buses
INSERT INTO Buses (
        NumberPlate,
        NumberOfSeats,
        SeatStructure,
        ConductorNumber,
        Condition,
        CreatedAt
    )
VALUES (
        'NP-ABC-001',
        45,
        '2*2',
        'COND001',
        'AC',
        GETDATE()
    ),
    (
        'NP-ABC-002',
        50,
        '2*3',
        'COND002',
        'Non-AC',
        GETDATE()
    );
-- Then add schedules for today and tomorrow
-- Get today's date first
DECLARE @Today DATE = CAST(GETDATE() AS DATE);
INSERT INTO Schedules (
        BusId,
        RouteId,
        ScheduledDate,
        DepartureTime,
        CreatedAt
    )
VALUES (1, 1, @Today, '08:30:00', GETDATE()),
    -- Galewela to Matale today
    (2, 1, @Today, '14:00:00', GETDATE()),
    -- Galewela to Matale today
    (1, 3, @Today, '10:00:00', GETDATE()),
    -- Galewela to Kandy today
    (
        2,
        2,
        DATEADD(DAY, 1, @Today),
        '09:00:00',
        GETDATE()
    );
-- Matale to Galewela tomorrow
-- Verify the data
SELECT s.ScheduleId,
    b.NumberPlate,
    b.Condition,
    r.FromLocation,
    r.ToLocation,
    r.Distance,
    s.ScheduledDate,
    s.DepartureTime
FROM Schedules s
    INNER JOIN Buses b ON s.BusId = b.BusId
    INNER JOIN Routes r ON s.RouteId = r.RouteId
ORDER BY s.ScheduledDate DESC;