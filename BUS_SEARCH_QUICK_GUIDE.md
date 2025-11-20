# Bus Search Feature - Quick Reference

## What Was Implemented

✅ **Home Page Search Interface**

- Search form with From, To, and Date fields
- Autocomplete suggestions for locations
- Beautiful, responsive UI with gradient design

✅ **Backend Search API**

- `POST /Home/SearchBuses` - Search for buses
- `GET /Home/GetLocations` - Get available locations for autocomplete

✅ **Database Filtering**

- Filters schedules by route (From and To locations)
- Filters by travel date
- Returns complete bus and route information

✅ **Frontend Features**

- Real-time search results display
- Detailed bus information cards
- Loading states and error handling
- Mobile-responsive design
- Autocomplete for locations

## File Changes

### Modified Files:

1. **Controllers/HomeController.cs**

   - Added `SearchBuses()` method for POST requests
   - Added `GetLocations()` method for autocomplete
   - Added database context injection

2. **Views/Home/Index.cshtml**
   - Completely redesigned home page
   - Added search form with modern styling
   - Added JavaScript for form handling and API calls
   - Added results display with bus cards

## How to Use

### For Users:

1. Go to home page (after login)
2. Enter departure location
3. Enter destination
4. Select travel date
5. Click "Search Buses"
6. View available buses with details
7. Click "Book Now" to proceed (future feature)

### For Developers:

**API Endpoints:**

```
POST /Home/SearchBuses
Body:
{
  "fromLocation": "City A",
  "toLocation": "City B",
  "searchDate": "2025-11-20"
}

Response:
{
  "success": true,
  "data": [
    {
      "scheduleId": 1,
      "numberPlate": "ABC-123",
      "numberOfSeats": 45,
      "departureTime": "08:30:00",
      "distance": 150.50,
      "condition": "AC",
      ...
    }
  ],
  "message": "Found X buses"
}
```

```
GET /Home/GetLocations

Response:
{
  "success": true,
  "data": ["City A", "City B", "City C"]
}
```

## Key Features

### Search Criteria:

- **Flexible Location Matching**: Uses "Contains" for partial matches (case-insensitive)
- **Exact Date Matching**: Searches for specific travel dates only
- **Real-time Results**: Shows matching buses immediately after search

### Data Displayed:

- Bus number plate
- AC/Non-AC condition
- Number of seats and seat structure
- Departure time
- Route (From → To)
- Distance and estimated time
- Scheduled date

## Database Structure

The feature uses these relationships:

```
Schedule
├── Bus (many-to-one)
│   ├── NumberPlate
│   ├── NumberOfSeats
│   ├── SeatStructure
│   └── Condition
├── Route (many-to-one)
│   ├── FromLocation
│   ├── ToLocation
│   ├── Distance
│   └── EstimatedTime
├── ScheduledDate
└── DepartureTime
```

## Validation

- **From Location**: Required, non-empty
- **To Location**: Required, non-empty
- **Search Date**: Required, cannot be before today
- **Locations**: Auto-populated from database routes

## Error Handling

- Invalid inputs: Shows validation error message
- No buses found: Displays user-friendly message
- API errors: Shows error details and logs to server

## Future Enhancements

1. **Booking System**: Complete booking workflow
2. **Seat Availability**: Show available seats
3. **Fare Display**: Calculate and show fare
4. **User Preferences**: Save favorite routes
5. **Advanced Filters**: Filter by bus type, time, etc.
6. **Reviews & Ratings**: Show bus operator ratings

## Notes

- The home page is now the search interface
- Authentication can be added to show user-specific features
- The "Book Now" button is a placeholder for future development
- Locations are dynamically fetched from all routes in the database
- Search is case-insensitive for better user experience
