-- Bus Search Feature - Test Data Setup
-- Use this SQL to populate test data for the bus search feature
-- First, insert test routes
INSERT INTO Routes (
        FromLocation,
        ToLocation,
        Distance,
        EstimatedTime,
        CreatedAt
    )
VALUES (
        'New York',
        'Boston',
        215.50,
        '3h 45m',
        GETDATE()
    ),
    (
        'New York',
        'Philadelphia',
        95.25,
        '2h 15m',
        GETDATE()
    ),
    (
        'Boston',
        'Washington DC',
        440.00,
        '7h 30m',
        GETDATE()
    ),
    (
        'Philadelphia',
        'Washington DC',
        140.75,
        '2h 45m',
        GETDATE()
    ),
    (
        'New York',
        'Washington DC',
        225.00,
        '4h 00m',
        GETDATE()
    );
-- Insert test buses
INSERT INTO Buses (
        NumberPlate,
        NumberOfSeats,
        SeatStructure,
        ConductorNumber,
        Condition,
        CreatedAt
    )
VALUES (
        'NY-BUS-001',
        45,
        '2*2',
        'COND001',
        'AC',
        GETDATE()
    ),
    (
        'NY-BUS-002',
        50,
        '2*3',
        'COND002',
        'Non-AC',
        GETDATE()
    ),
    (
        'NY-BUS-003',
        40,
        '2*2',
        'COND003',
        'AC',
        GETDATE()
    ),
    (
        'NY-BUS-004',
        55,
        '2*3',
        'COND004',
        'AC',
        GETDATE()
    ),
    (
        'NY-BUS-005',
        42,
        '2*2',
        'COND005',
        'Non-AC',
        GETDATE()
    );
-- Insert test schedules for today and upcoming days
-- You need to replace CAST(GETDATE() AS DATE) with actual dates if needed
-- New York to Boston - Today
INSERT INTO Schedules (
        BusId,
        RouteId,
        ScheduledDate,
        DepartureTime,
        CreatedAt
    )
VALUES (
        1,
        1,
        CAST(GETDATE() AS DATE),
        '08:30:00',
        GETDATE()
    ),
    (
        2,
        1,
        CAST(GETDATE() AS DATE),
        '12:00:00',
        GETDATE()
    ),
    (
        3,
        1,
        CAST(GETDATE() AS DATE),
        '17:30:00',
        GETDATE()
    );
-- New York to Philadelphia - Today
INSERT INTO Schedules (
        BusId,
        RouteId,
        ScheduledDate,
        DepartureTime,
        CreatedAt
    )
VALUES (
        4,
        2,
        CAST(GETDATE() AS DATE),
        '09:00:00',
        GETDATE()
    ),
    (
        5,
        2,
        CAST(GETDATE() AS DATE),
        '14:00:00',
        GETDATE()
    );
-- Tomorrow - Multiple routes
INSERT INTO Schedules (
        BusId,
        RouteId,
        ScheduledDate,
        DepartureTime,
        CreatedAt
    )
VALUES (
        1,
        1,
        DATEADD(DAY, 1, CAST(GETDATE() AS DATE)),
        '07:00:00',
        GETDATE()
    ),
    (
        2,
        3,
        DATEADD(DAY, 1, CAST(GETDATE() AS DATE)),
        '06:00:00',
        GETDATE()
    ),
    (
        3,
        4,
        DATEADD(DAY, 1, CAST(GETDATE() AS DATE)),
        '08:30:00',
        GETDATE()
    ),
    (
        4,
        5,
        DATEADD(DAY, 1, CAST(GETDATE() AS DATE)),
        '10:00:00',
        GETDATE()
    );
-- Day after tomorrow
INSERT INTO Schedules (
        BusId,
        RouteId,
        ScheduledDate,
        DepartureTime,
        CreatedAt
    )
VALUES (
        5,
        1,
        DATEADD(DAY, 2, CAST(GETDATE() AS DATE)),
        '09:30:00',
        GETDATE()
    ),
    (
        1,
        2,
        DATEADD(DAY, 2, CAST(GETDATE() AS DATE)),
        '11:00:00',
        GETDATE()
    ),
    (
        2,
        3,
        DATEADD(DAY, 2, CAST(GETDATE() AS DATE)),
        '13:00:00',
        GETDATE()
    );
-- View all schedules with bus and route details
SELECT s.ScheduleId,
    b.NumberPlate,
    b.NumberOfSeats,
    b.SeatStructure,
    b.Condition,
    r.FromLocation,
    r.ToLocation,
    r.Distance,
    r.EstimatedTime,
    s.ScheduledDate,
    s.DepartureTime
FROM Schedules s
    INNER JOIN Buses b ON s.BusId = b.BusId
    INNER JOIN Routes r ON s.RouteId = r.RouteId
ORDER BY s.ScheduledDate DESC,
    s.DepartureTime;
-- Test queries (copy and run in SQL Server to verify data):
-- Query 1: Find all buses from New York to Boston today
-- Uncomment and replace GETDATE() with specific date if needed
/*
 SELECT 
 s.ScheduleId,
 b.NumberPlate,
 b.NumberOfSeats,
 b.Condition,
 r.FromLocation,
 r.ToLocation,
 s.DepartureTime,
 s.ScheduledDate
 FROM Schedules s
 INNER JOIN Buses b ON s.BusId = b.BusId
 INNER JOIN Routes r ON s.RouteId = r.RouteId
 WHERE r.FromLocation LIKE '%New York%'
 AND r.ToLocation LIKE '%Boston%'
 AND CAST(s.ScheduledDate AS DATE) = CAST(GETDATE() AS DATE)
 ORDER BY s.DepartureTime;
 */
-- Query 2: Find all available locations
/*
 SELECT DISTINCT FromLocation FROM Routes
 UNION
 SELECT DISTINCT ToLocation FROM Routes
 ORDER BY FromLocation;
 */
-- Query 3: Check schedules for next 7 days from New York
/*
 SELECT 
 s.ScheduleId,
 b.NumberPlate,
 r.FromLocation,
 r.ToLocation,
 s.DepartureTime,
 s.ScheduledDate
 FROM Schedules s
 INNER JOIN Buses b ON s.BusId = b.BusId
 INNER JOIN Routes r ON s.RouteId = r.RouteId
 WHERE r.FromLocation LIKE '%New York%'
 AND CAST(s.ScheduledDate AS DATE) >= CAST(GETDATE() AS DATE)
 AND CAST(s.ScheduledDate AS DATE) <= DATEADD(DAY, 7, CAST(GETDATE() AS DATE))
 ORDER BY s.ScheduledDate, s.DepartureTime;
 */