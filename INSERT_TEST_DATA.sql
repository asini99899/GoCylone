-- ============================================================================
-- GoCylone Bus Search - Sample Data Script
-- ============================================================================
-- This script adds test data to help with bus search functionality
-- Run this in SQL Server Management Studio against your GoCylone database
-- ============================================================================
-- Clear existing data (optional - comment out if you want to keep existing data)
-- DELETE FROM Schedules;
-- DELETE FROM Routes;
-- DELETE FROM Buses;
-- ============================================================================
-- 1. Insert Routes
-- ============================================================================
INSERT INTO Routes (
        FromLocation,
        ToLocation,
        Distance,
        EstimatedTime
    )
VALUES ('Colombo', 'Kandy', 120, '3 hours'),
    ('Kandy', 'Galle', 140, '4 hours'),
    ('Colombo', 'Galle', 240, '6 hours'),
    ('Colombo', 'Jaffna', 300, '8 hours'),
    ('Matara', 'Colombo', 160, '4 hours'),
    ('Negombo', 'Colombo', 40, '1 hour'),
    ('Colombo', 'Kurunegala', 100, '2.5 hours'),
    ('Galle', 'Matara', 50, '1.5 hours'),
    ('Kandy', 'Nuwara Eliya', 60, '2 hours'),
    ('Colombo', 'Anuradhapura', 200, '5 hours');
-- ============================================================================
-- 2. Insert Buses
-- ============================================================================
INSERT INTO Buses (
        NumberPlate,
        NumberOfSeats,
        SeatStructure,
        Condition
    )
VALUES ('WP-ABC-1234', 50, '2*3', 'Good'),
    ('WP-XYZ-5678', 45, '2*2', 'Excellent'),
    ('WP-DEF-9012', 52, '2*3', 'Good'),
    ('WP-GHI-3456', 48, '2*2', 'Excellent'),
    ('WP-JKL-7890', 50, '2*3', 'Good'),
    ('WP-MNO-2345', 55, '3*2', 'Excellent'),
    ('WP-PQR-6789', 40, '2*2', 'Fair'),
    ('WP-STU-0123', 52, '2*3', 'Good'),
    ('WP-VWX-4567', 48, '2*2', 'Excellent'),
    ('WP-YZA-8901', 50, '2*3', 'Good');
-- ============================================================================
-- 3. Insert Schedules - TODAY and FUTURE DATES
-- ============================================================================
-- NOTE: GETDATE() represents today's date
-- These schedules will be searchable for today and the next few days
INSERT INTO Schedules (BusId, RouteId, ScheduledDate, DepartureTime)
VALUES -- Colombo to Kandy - Today's schedules
    (1, 1, CAST(GETDATE() AS DATE), '06:00:00'),
    (2, 1, CAST(GETDATE() AS DATE), '08:30:00'),
    (3, 1, CAST(GETDATE() AS DATE), '14:00:00'),
    (4, 1, CAST(GETDATE() AS DATE), '17:30:00'),
    -- Colombo to Kandy - Tomorrow
    (5, 1, CAST(GETDATE() + 1 AS DATE), '06:00:00'),
    (6, 1, CAST(GETDATE() + 1 AS DATE), '09:00:00'),
    (1, 1, CAST(GETDATE() + 1 AS DATE), '15:00:00'),
    -- Colombo to Kandy - Day after tomorrow
    (2, 1, CAST(GETDATE() + 2 AS DATE), '07:00:00'),
    (3, 1, CAST(GETDATE() + 2 AS DATE), '13:00:00'),
    -- Kandy to Galle - Today
    (1, 2, CAST(GETDATE() AS DATE), '10:00:00'),
    (2, 2, CAST(GETDATE() AS DATE), '15:30:00'),
    (4, 2, CAST(GETDATE() AS DATE), '18:00:00'),
    -- Kandy to Galle - Tomorrow
    (5, 2, CAST(GETDATE() + 1 AS DATE), '10:30:00'),
    (6, 2, CAST(GETDATE() + 1 AS DATE), '16:00:00'),
    -- Colombo to Galle - Today
    (3, 3, CAST(GETDATE() AS DATE), '07:00:00'),
    (5, 3, CAST(GETDATE() AS DATE), '16:00:00'),
    (7, 3, CAST(GETDATE() AS DATE), '19:30:00'),
    -- Colombo to Galle - Tomorrow
    (1, 3, CAST(GETDATE() + 1 AS DATE), '08:00:00'),
    (4, 3, CAST(GETDATE() + 1 AS DATE), '17:00:00'),
    -- Colombo to Jaffna - Today
    (6, 4, CAST(GETDATE() AS DATE), '05:30:00'),
    (8, 4, CAST(GETDATE() AS DATE), '20:00:00'),
    -- Colombo to Jaffna - Tomorrow
    (2, 4, CAST(GETDATE() + 1 AS DATE), '05:30:00'),
    -- Matara to Colombo - Today
    (2, 5, CAST(GETDATE() AS DATE), '05:30:00'),
    (4, 5, CAST(GETDATE() AS DATE), '18:00:00'),
    (7, 5, CAST(GETDATE() AS DATE), '10:00:00'),
    -- Matara to Colombo - Tomorrow
    (5, 5, CAST(GETDATE() + 1 AS DATE), '06:00:00'),
    (9, 5, CAST(GETDATE() + 1 AS DATE), '19:00:00'),
    -- Negombo to Colombo - Today
    (3, 6, CAST(GETDATE() AS DATE), '07:00:00'),
    (8, 6, CAST(GETDATE() AS DATE), '09:00:00'),
    (10, 6, CAST(GETDATE() AS DATE), '11:00:00'),
    (1, 6, CAST(GETDATE() AS DATE), '13:00:00'),
    -- Negombo to Colombo - Tomorrow
    (4, 6, CAST(GETDATE() + 1 AS DATE), '08:00:00'),
    (6, 6, CAST(GETDATE() + 1 AS DATE), '14:00:00'),
    -- Colombo to Kurunegala - Today
    (5, 7, CAST(GETDATE() AS DATE), '09:00:00'),
    (2, 7, CAST(GETDATE() AS DATE), '14:30:00'),
    -- Colombo to Kurunegala - Tomorrow
    (3, 7, CAST(GETDATE() + 1 AS DATE), '09:30:00'),
    (9, 7, CAST(GETDATE() + 1 AS DATE), '16:00:00'),
    -- Galle to Matara - Today
    (1, 8, CAST(GETDATE() AS DATE), '08:00:00'),
    (4, 8, CAST(GETDATE() AS DATE), '12:00:00'),
    (7, 8, CAST(GETDATE() AS DATE), '17:00:00'),
    -- Galle to Matara - Tomorrow
    (2, 8, CAST(GETDATE() + 1 AS DATE), '08:30:00'),
    (10, 8, CAST(GETDATE() + 1 AS DATE), '18:00:00'),
    -- Kandy to Nuwara Eliya - Today
    (6, 9, CAST(GETDATE() AS DATE), '09:00:00'),
    (8, 9, CAST(GETDATE() AS DATE), '15:00:00'),
    -- Kandy to Nuwara Eliya - Tomorrow
    (3, 9, CAST(GETDATE() + 1 AS DATE), '09:30:00'),
    -- Colombo to Anuradhapura - Today
    (9, 10, CAST(GETDATE() AS DATE), '05:00:00'),
    (10, 10, CAST(GETDATE() AS DATE), '19:30:00'),
    -- Colombo to Anuradhapura - Tomorrow
    (1, 10, CAST(GETDATE() + 1 AS DATE), '05:30:00'),
    (5, 10, CAST(GETDATE() + 1 AS DATE), '20:00:00');
-- ============================================================================
-- 4. Verification Queries
-- ============================================================================
-- Run these queries to verify the data was inserted correctly
-- Check routes inserted
-- SELECT COUNT(*) as 'Total Routes' FROM Routes;
-- SELECT * FROM Routes ORDER BY FromLocation, ToLocation;
-- Check buses inserted
-- SELECT COUNT(*) as 'Total Buses' FROM Buses;
-- SELECT * FROM Buses ORDER BY NumberPlate;
-- Check schedules inserted
-- SELECT COUNT(*) as 'Total Schedules' FROM Schedules;
-- SELECT s.ScheduleId, b.NumberPlate, r.FromLocation + ' -> ' + r.ToLocation as Route, 
--        s.ScheduledDate, s.DepartureTime
-- FROM Schedules s
-- INNER JOIN Buses b ON s.BusId = b.BusId
-- INNER JOIN Routes r ON s.RouteId = r.RouteId
-- ORDER BY s.ScheduledDate, s.DepartureTime;
-- Check schedules for today
-- SELECT s.ScheduleId, b.NumberPlate, r.FromLocation + ' -> ' + r.ToLocation as Route, 
--        s.ScheduledDate, s.DepartureTime
-- FROM Schedules s
-- INNER JOIN Buses b ON s.BusId = b.BusId
-- INNER JOIN Routes r ON s.RouteId = r.RouteId
-- WHERE CAST(s.ScheduledDate AS DATE) = CAST(GETDATE() AS DATE)
-- ORDER BY s.DepartureTime;
-- ============================================================================
-- Test Search Example
-- ============================================================================
-- After inserting this data, you can search for:
-- From: Colombo, To: Kandy, Date: Today -> Should find 4 buses
-- From: Colombo, To: Galle, Date: Today -> Should find 3 buses
-- From: Matara, To: Colombo, Date: Today -> Should find 3 buses
-- From: Kandy, To: Galle, Date: Today -> Should find 3 buses
-- etc.
-- ============================================================================
-- End of Script
-- ============================================================================