# Bus Search Not Showing Results - Debugging Guide

## Problem
When searching for buses, the system always shows "No buses found" even though schedules should exist.

## Root Causes

The issue could be one of these:

1. **No data in database** - Routes, Buses, or Schedules tables are empty
2. **Date mismatch** - Scheduled dates are in the past or future
3. **Location name mismatch** - Location names in search don't match exactly in database
4. **Null or missing relationships** - Routes or Buses not properly linked to Schedules

## Solution: Check Database Content

### Step 1: Use Debug Endpoint

Open your browser and visit:
```
http://localhost:5020/Home/DebugData
```

This will show you:
- All Routes in the database
- All Buses in the database  
- All Schedules with their linked Route and Bus information

**Example output:**
```json
{
  "success": true,
  "routes": [
    {"routeId": 1, "fromLocation": "Colombo", "toLocation": "Kandy", "distance": 120, "estimatedTime": "3 hours"},
    {"routeId": 2, "fromLocation": "Kandy", "toLocation": "Galle", "distance": 140, "estimatedTime": "4 hours"}
  ],
  "buses": [
    {"busId": 1, "numberPlate": "ABC-1234", "numberOfSeats": 50, "seatStructure": "2*3", "condition": "Good"},
    {"busId": 2, "numberPlate": "XYZ-5678", "numberOfSeats": 45, "seatStructure": "2*2", "condition": "Excellent"}
  ],
  "schedules": [
    {"scheduleId": 1, "scheduledDate": "2025-11-20", "departureTime": "06:00:00", "busNumberPlate": "ABC-1234", "fromLocation": "Colombo", "toLocation": "Kandy"}
  ]
}
```

### Step 2: Add Test Data (if empty)

If the debug endpoint shows empty arrays, you need to add test data. Run this SQL script in your SQL Server:

```sql
-- Add Routes
INSERT INTO Routes (FromLocation, ToLocation, Distance, EstimatedTime)
VALUES 
  ('Colombo', 'Kandy', 120, '3 hours'),
  ('Kandy', 'Galle', 140, '4 hours'),
  ('Colombo', 'Galle', 240, '6 hours'),
  ('Colombo', 'Jaffna', 300, '8 hours'),
  ('Matara', 'Colombo', 160, '4 hours');

-- Add Buses
INSERT INTO Buses (NumberPlate, NumberOfSeats, SeatStructure, Condition)
VALUES 
  ('ABC-1234', 50, '2*3', 'Good'),
  ('XYZ-5678', 45, '2*2', 'Excellent'),
  ('DEF-9012', 52, '2*3', 'Good'),
  ('GHI-3456', 48, '2*2', 'Excellent'),
  ('JKL-7890', 50, '2*3', 'Good');

-- Add Schedules for TODAY and FUTURE DATES
INSERT INTO Schedules (BusId, RouteId, ScheduledDate, DepartureTime)
VALUES 
  -- Colombo to Kandy schedules (today and future)
  (1, 1, CAST(GETDATE() AS DATE), '06:00:00'),
  (2, 1, CAST(GETDATE() AS DATE), '08:30:00'),
  (3, 1, CAST(GETDATE() AS DATE), '14:00:00'),
  (4, 1, CAST(GETDATE() + 1 AS DATE), '06:00:00'),
  
  -- Kandy to Galle schedules
  (1, 2, CAST(GETDATE() AS DATE), '10:00:00'),
  (2, 2, CAST(GETDATE() AS DATE), '15:30:00'),
  
  -- Colombo to Galle schedules
  (3, 3, CAST(GETDATE() AS DATE), '07:00:00'),
  (5, 3, CAST(GETDATE() AS DATE), '16:00:00'),
  
  -- Matara to Colombo schedules
  (2, 5, CAST(GETDATE() AS DATE), '05:30:00'),
  (4, 5, CAST(GETDATE() AS DATE), '18:00:00');
```

### Step 3: Verify After Adding Data

1. Run the above SQL script in your database
2. Visit `http://localhost:5020/Home/DebugData` again
3. You should see routes, buses, and schedules
4. Try searching with matching location names (e.g., "Colombo" to "Kandy")

## Step 4: Test the Search

1. Go to the home page: `http://localhost:5020/`
2. **From Location**: Type "Colombo"
3. **To Location**: Type "Kandy"
4. **Date**: Select today's date
5. Click **Search Buses**
6. You should see the buses that were just added

## Browser Console Debugging

Open Developer Tools (F12) and check the Console tab for:

1. **Sent payload:**
```javascript
{fromLocation: "Colombo", toLocation: "Kandy", searchDate: "2025-11-20T00:00:00.000Z"}
```

2. **Response:**
```javascript
{success: true, data: [...buses...], message: "Found 3 buses"}
```

## Important Notes

1. **Dates must be TODAY or FUTURE** - The search uses `s.ScheduledDate.Date == searchDate.Date`
2. **Location names are case-insensitive** - The search does `.ToLower()` comparison
3. **Partial matching works** - "Colom" will match "Colombo"
4. **Both exact and partial matching** - First tries exact match, then partial match

## If Still Not Working

1. Check the application logs (the console where you ran `dotnet run`)
2. Look for messages like:
   - `Total routes in DB: 0` (means no routes)
   - `Total schedules for date: 0` (means no schedules for that date)
3. Verify your search parameters match location names in the database exactly

## Advanced: View All Endpoints

**Check locations:**
```
GET http://localhost:5020/Home/GetLocations
```

**Search buses:**
```
POST http://localhost:5020/Home/SearchBuses
Body: {"fromLocation":"Colombo","toLocation":"Kandy","searchDate":"2025-11-20T00:00:00Z"}
```

**Debug data:**
```
GET http://localhost:5020/Home/DebugData
```
