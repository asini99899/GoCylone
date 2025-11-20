# Bus Search Feature Implementation

## Overview

A complete bus search feature has been implemented on the home page that allows users to search for buses by route (from location, to location) and date. The feature filters buses from the database that match the search criteria.

## Features Implemented

### 1. **Home Page Search Interface** (`Views/Home/Index.cshtml`)

- **Search Form** with three input fields:

  - **From Location**: Departure location with autocomplete
  - **To Location**: Destination with autocomplete
  - **Date**: Travel date selector with minimum date set to today

- **Dynamic Results Display**:

  - Shows matching buses with all relevant details
  - Each bus card displays:
    - Bus number plate
    - AC/Non-AC condition indicator
    - Route information (from → to)
    - Departure time
    - Number of seats
    - Seat structure (e.g., 2x2, 2x3)
    - Distance and estimated travel time
    - Scheduled date
    - "Book Now" button

- **User Experience Features**:
  - Loading spinner during search
  - Success/error messages
  - Real-time form validation
  - Responsive design for mobile and desktop
  - Auto-complete location suggestions from database
  - Clean, modern UI with gradient background

### 2. **Backend Controller Methods** (`Controllers/HomeController.cs`)

#### `SearchBuses(string fromLocation, string toLocation, DateTime searchDate)` - POST

**Purpose**: Search for buses matching the criteria

**Parameters**:

- `fromLocation`: Starting location
- `toLocation`: Destination
- `searchDate`: Date of travel

**Logic**:

1. Validates that from and to locations are provided
2. Queries the database for schedules that match:
   - Route's FromLocation contains the search fromLocation
   - Route's ToLocation contains the search toLocation
   - ScheduledDate matches the search date
3. Includes related Bus and Route data
4. Returns bus details with schedule information

**Response**:

```json
{
  "success": true,
  "data": [
    {
      "scheduleId": 1,
      "busId": 1,
      "numberPlate": "ABC-123",
      "numberOfSeats": 45,
      "seatStructure": "2*2",
      "condition": "AC",
      "fromLocation": "City A",
      "toLocation": "City B",
      "departureTime": "08:30:00",
      "scheduledDate": "2025-11-20T00:00:00",
      "distance": 150.5,
      "estimatedTime": "3h 30m"
    }
  ],
  "message": "Found X buses"
}
```

#### `GetLocations()` - GET

**Purpose**: Get all available locations for autocomplete

**Logic**:

1. Retrieves all unique locations from Routes table
2. Combines FromLocation and ToLocation
3. Returns distinct, sorted locations

**Response**:

```json
{
  "success": true,
  "data": ["City A", "City B", "City C"]
}
```

## Database Query Logic

The search uses Entity Framework Core with Include statements to:

```csharp
Schedules
  .Where(s => s.Route.FromLocation.Contains(fromLocation) &&
             s.Route.ToLocation.Contains(toLocation) &&
             s.ScheduledDate.Date == searchDate.Date)
  .Include(s => s.Bus)
  .Include(s => s.Route)
  .Select(s => new { /* bus and route details */ })
```

This ensures:

- Only schedules for the specified date are returned
- Bus and Route navigation properties are eagerly loaded
- Partial location matching is supported (case-insensitive contains)

## Frontend JavaScript Functions

### `loadLocations()`

Fetches available locations from the server and populates the datalist for autocomplete.

### `searchBuses(event)`

- Validates form inputs
- Sends POST request to `/Home/SearchBuses`
- Displays results or error messages

### `displayResults(buses, fromLocation, toLocation)`

- Renders bus cards with all information
- Handles empty results gracefully
- Provides booking button (placeholder for future implementation)

### `formatTime(timeString)` & `formatDate(dateString)`

Utility functions for formatting time and date display.

## Styling

- Modern gradient design with purple theme
- Responsive grid layout
- Smooth hover effects and animations
- Loading spinner animation
- Mobile-optimized layout
- Error and success message styling

## How It Works

1. **User Lands on Home Page**:

   - Page loads and automatically fetches available locations
   - Locations are displayed in autocomplete dropdowns

2. **User Searches**:

   - Enters from location, to location, and date
   - Clicks "Search Buses" button

3. **Search Processing**:

   - Frontend validates inputs
   - Sends POST request to backend
   - Backend queries database for matching schedules
   - Results are filtered by route and date

4. **Results Display**:
   - If buses found: Display as cards with full details
   - If no buses found: Show "No buses found" message
   - User can click "Book Now" to proceed with booking (future feature)

## Integration Points

This feature integrates with existing models:

- `Schedule`: Contains bus, route, date, and departure time
- `Bus`: Contains vehicle details (plate, seats, condition)
- `Route`: Contains location and distance information

## Future Enhancements

1. Booking system implementation
2. Fare calculation and display
3. Seat availability visualization
4. User authentication/login integration
5. Booking history
6. Payment gateway integration
7. Ratings and reviews

## Testing

To test the feature:

1. Ensure the application is running (`dotnet run`)
2. Navigate to http://localhost:5020
3. Enter search criteria and submit
4. Verify results display correctly

**Note**: Results will only appear if there are buses scheduled for the selected route and date in the database.
