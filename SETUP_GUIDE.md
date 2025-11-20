# GoCeylon Setup Guide

## Prerequisites

- .NET 9.0 SDK installed
- SQL Server Express (LAPTOP-RDNMEQ3T\SQLEXPRESS)
- Visual Studio Code or Visual Studio

## Step 1: Restore NuGet Packages

```powershell
cd "c:\Users\ccs\Desktop\projects for Job\GoCylone"
dotnet restore
```

## Step 2: Create Database Migrations

```powershell
dotnet ef migrations add InitialCreate
dotnet ef database update
```

**Note:** Make sure SQL Server is running before running migrations.

## Step 3: Insert Sample Data

After database creation, run the SQL scripts in `DATABASE_SETUP.sql` using SQL Server Management Studio:

- Connect to: `LAPTOP-RDNMEQ3T\SQLEXPRESS`
- Database: `ABCD`
- Execute the sample data insertion queries

## Step 4: Run the Application

```powershell
dotnet run
```

The application will start at:

- **HTTP**: http://localhost:5000
- **HTTPS**: https://localhost:5001

## Step 5: Access the Application

### Web Interface

- **Admin Dashboard**: https://localhost:5001/admin/dashboard
- **Buses Management**: https://localhost:5001/admin/buses
- **Routes Management**: https://localhost:5001/admin/routes
- **Schedules Management**: https://localhost:5001/admin/schedules
- **Fares Management**: https://localhost:5001/admin/fares
- **Users Management**: https://localhost:5001/admin/users

### API Endpoints

Base URL: `https://localhost:5001/api`

**Example cURL Commands:**

#### Get All Buses

```bash
curl -X GET "https://localhost:5001/api/bus" -k
```

#### Add a Bus

```bash
curl -X POST "https://localhost:5001/api/bus" \
  -H "Content-Type: application/json" \
  -d '{
    "numberPlate": "WP-CD-9999",
    "numberOfSeats": 48,
    "seatStructure": "2*2",
    "conductorNumber": "C999",
    "condition": "AC"
  }' -k
```

#### Login User

```bash
curl -X POST "https://localhost:5001/api/user/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@gocylon.com",
    "password": "Admin@123"
  }' -k
```

## Default Credentials

### Admin Account

- **Email**: admin@gocylon.com
- **Password**: Admin@123
- **Role**: admin

### Regular User

- **Email**: user1@gocylon.com
- **Password**: Admin@123
- **Role**: user

## Project Structure

```
GoCeylon/
├── Controllers/
│   ├── BusController.cs               # Bus CRUD operations
│   ├── RouteController.cs             # Route CRUD operations
│   ├── ScheduleController.cs          # Schedule CRUD operations
│   ├── BusFareController.cs           # Fare CRUD operations
│   ├── UserController.cs              # User management & authentication
│   ├── AdminController.cs             # Admin view controller
│   └── HomeController.cs              # Home page
│
├── Models/
│   ├── Bus.cs                         # Bus entity
│   ├── Route.cs                       # Route entity
│   ├── Schedule.cs                    # Schedule entity
│   ├── BusFare.cs                     # Bus fare entity
│   ├── User.cs                        # User entity
│   └── ErrorViewModel.cs
│
├── Data/
│   └── GoCyloneDbContext.cs           # Entity Framework DbContext
│
├── Views/
│   ├── Admin/
│   │   ├── Dashboard.cshtml           # Admin dashboard
│   │   ├── Buses.cshtml               # Buses management UI
│   │   ├── Routes.cshtml              # Routes management UI
│   │   ├── Schedules.cshtml           # Schedules management UI
│   │   ├── Fares.cshtml               # Fares management UI
│   │   └── Users.cshtml               # Users management UI
│   ├── Home/
│   │   ├── Index.cshtml
│   │   └── Privacy.cshtml
│   └── Shared/
│       ├── _Layout.cshtml             # Main layout with navigation
│       └── _Layout.cshtml.css
│
├── wwwroot/
│   ├── css/
│   │   └── site.css                   # Red & Yellow GoCeylon theme
│   ├── js/
│   │   └── site.js
│   └── lib/
│
├── Program.cs                         # Application startup
├── GoCeylon.csproj                    # Project file
├── appsettings.json                   # Configuration
├── API_DOCUMENTATION.md               # Complete API documentation
├── DATABASE_SETUP.sql                 # SQL sample data
└── SETUP_GUIDE.md                     # This file
```

## Key Features Implemented

### 1. Bus Management

- ✅ Add new buses with details (number plate, seats, structure, conductor, condition)
- ✅ View all buses
- ✅ Edit bus information
- ✅ Delete buses (with validation for active schedules)

### 2. Route Management

- ✅ Create routes with distance and estimated time
- ✅ View all routes
- ✅ Update route information
- ✅ Delete routes

### 3. Schedule Management

- ✅ Schedule buses on routes
- ✅ Set specific date and departure time
- ✅ View schedules by bus or route
- ✅ Edit and delete schedules
- ✅ Date validation (no past dates)

### 4. Fare Management

- ✅ Initialize fare per kilometer
- ✅ Calculate total fare based on distance
- ✅ View and update fare configuration
- ✅ Multiple fare entries support

### 5. User Management

- ✅ Register admin and regular users
- ✅ User authentication with password hashing
- ✅ Role-based user management
- ✅ View all users
- ✅ Edit and delete users

### 6. UI/UX

- ✅ Red (#DC143C) and Yellow (#FFD700) GoCeylon theme
- ✅ Responsive navigation bar with admin dropdown
- ✅ Dashboard with statistics
- ✅ Form-based interfaces for all CRUD operations
- ✅ Data tables with action buttons

## API Response Format

All API responses follow a consistent format:

```json
{
  "success": true/false,
  "data": {...},
  "message": "Success or error message"
}
```

## Troubleshooting

### Database Connection Issues

1. Verify SQL Server is running
2. Check connection string in `appsettings.json`
3. Ensure database `ABCD` exists

### Migration Issues

```powershell
# Remove latest migration if needed
dotnet ef migrations remove

# Create fresh migration
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update
```

### HTTPS Certificate Issues

If you get certificate warnings, add `-k` flag to curl commands or ignore warnings in your client.

## Flutter Integration

For Flutter apps, use the API endpoints with appropriate HTTP client setup:

```dart
import 'package:http/http.dart' as http;

// Example: Get all buses
final response = await http.get(
  Uri.parse('https://your-server:5001/api/bus'),
  headers: {'Content-Type': 'application/json'},
);
```

## Testing with Postman

1. Import the API endpoints into Postman
2. Use environment variables for base URL
3. Set up authentication tokens if needed
4. Test all CRUD operations

## Performance Tips

- Use connection pooling for database
- Implement pagination for large datasets
- Add caching for frequently accessed data
- Use async/await for better performance

## Security Notes

- Passwords are hashed using SHA-256
- Implement JWT tokens for API authentication (future enhancement)
- Use HTTPS for all API communications
- Validate all user inputs

## Future Enhancements

1. JWT token-based authentication
2. Role-based authorization middleware
3. Booking/Reservation system for users
4. Payment integration
5. Real-time notifications
6. Mobile app push notifications
7. Trip history and analytics
8. Admin reports and statistics

## Support & Documentation

- **API Documentation**: See `API_DOCUMENTATION.md`
- **Database Setup**: See `DATABASE_SETUP.sql`
- **Code Comments**: Each controller has inline documentation

## Contact

For questions or issues, refer to the comprehensive documentation or check the controller comments.

---

**GoCeylon v1.0.0** - Proudly Sri Lankan. One nation. One route. One app. 🌴
