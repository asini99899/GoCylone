-- GoCeylon Bus Management System - SQL Initialization Script
-- Database: ABCD
-- Server: LAPTOP-RDNMEQ3T\SQLEXPRESS
-- Create Tables (Run migrations first)
-- ============================================
-- SAMPLE DATA INSERTION
-- ============================================
-- Insert Sample Users
INSERT INTO Users (Email, PasswordHash, FullName, Role, CreatedAt)
VALUES (
        'admin@gocylon.com',
        'X9YsZBqkp1EfAqh1v7FD+8QhBqMz7F9v7L5gB8c7yLM=',
        'Admin User',
        'admin',
        GETDATE()
    ),
    (
        'user1@gocylon.com',
        'X9YsZBqkp1EfAqh1v7FD+8QhBqMz7F9v7L5gB8c7yLM=',
        'John Doe',
        'user',
        GETDATE()
    ),
    (
        'user2@gocylon.com',
        'X9YsZBqkp1EfAqh1v7FD+8QhBqMz7F9v7L5gB8c7yLM=',
        'Jane Smith',
        'user',
        GETDATE()
    );
-- Insert Sample Buses
INSERT INTO Buses (
        NumberPlate,
        NumberOfSeats,
        SeatStructure,
        ConductorNumber,
        Condition,
        CreatedAt
    )
VALUES ('WP-CD-0001', 48, '2*2', 'C001', 'AC', GETDATE()),
    (
        'WP-CD-0002',
        50,
        '2*3',
        'C002',
        'Non-AC',
        GETDATE()
    ),
    ('WP-CD-0003', 48, '2*2', 'C003', 'AC', GETDATE()),
    (
        'WP-CD-0004',
        45,
        '2*2',
        'C004',
        'Non-AC',
        GETDATE()
    );
-- Insert Sample Routes
INSERT INTO Routes (
        FromLocation,
        ToLocation,
        Distance,
        EstimatedTime,
        CreatedAt
    )
VALUES ('Colombo', 'Kandy', 115.5, '2h 30m', GETDATE()),
    ('Colombo', 'Galle', 140.0, '3h 15m', GETDATE()),
    (
        'Kandy',
        'Nuwara Eliya',
        52.0,
        '1h 30m',
        GETDATE()
    ),
    ('Galle', 'Matara', 48.0, '1h 15m', GETDATE()),
    ('Colombo', 'Negombo', 35.0, '1h 00m', GETDATE());
-- Insert Sample Schedules
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
        '08:00:00',
        GETDATE()
    ),
    (
        1,
        1,
        DATEADD(DAY, 1, CAST(GETDATE() AS DATE)),
        '14:30:00',
        GETDATE()
    ),
    (
        2,
        2,
        DATEADD(DAY, 1, CAST(GETDATE() AS DATE)),
        '09:00:00',
        GETDATE()
    ),
    (
        3,
        3,
        DATEADD(DAY, 1, CAST(GETDATE() AS DATE)),
        '10:00:00',
        GETDATE()
    ),
    (
        4,
        4,
        DATEADD(DAY, 1, CAST(GETDATE() AS DATE)),
        '11:00:00',
        GETDATE()
    );
-- Insert Sample Bus Fares
INSERT INTO BusFares (FarePerKm, Description, CreatedAt)
VALUES (
        50.00,
        'Base fare per km for all routes',
        GETDATE()
    );
-- ============================================
-- VERIFICATION QUERIES
-- ============================================
-- Check Users
SELECT COUNT(*) as TotalUsers
FROM Users;
SELECT *
FROM Users;
-- Check Buses
SELECT COUNT(*) as TotalBuses
FROM Buses;
SELECT *
FROM Buses;
-- Check Routes
SELECT COUNT(*) as TotalRoutes
FROM Routes;
SELECT *
FROM Routes;
-- Check Schedules
SELECT COUNT(*) as TotalSchedules
FROM Schedules;
SELECT s.*,
    b.NumberPlate,
    r.FromLocation,
    r.ToLocation
FROM Schedules s
    LEFT JOIN Buses b ON s.BusId = b.BusId
    LEFT JOIN Routes r ON s.RouteId = r.RouteId;
-- Check Fares
SELECT COUNT(*) as TotalFares
FROM BusFares;
SELECT *
FROM BusFares;