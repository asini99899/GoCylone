# 🌴 GoCeylon - Bus Management System

**Proudly Sri Lankan. Combines heritage with modern mobility.**  
_Tagline: "One nation. One route. One app."_

---

## 📱 Project Overview

GoCeylon is a comprehensive bus management system designed for Sri Lanka's bus industry. This admin panel application enables administrators to manage buses, routes, schedules, and fares efficiently. The system provides RESTful APIs that integrate seamlessly with your Flutter mobile application.

### Color Scheme

- **Primary Red**: `#DC143C` - Represents energy and passion
- **Primary Yellow**: `#FFD700` - Represents the sun and Sri Lankan heritage
- **Dark Red**: `#8B0000` - Accent color for highlights

---

## ✨ Key Features

### 🚌 Bus Management

- Add new buses with comprehensive details
- Track buses by number plate, seat count, and configuration
- Manage seat structure (2×2 or 2×3)
- Monitor conductor details and bus condition (AC/Non-AC)
- View, edit, and delete bus records

### 🛣️ Route Management

- Create routes between cities
- Set distance and estimated travel time
- View all routes with detailed information
- Update and delete routes
- Support for multiple routes

### 📅 Schedule Management

- Schedule buses on specific routes
- Set departure dates and times
- View schedules filtered by bus or route
- Date validation (prevent past dates)
- Edit and cancel schedules

### 💰 Fare Management

- Initialize fare per kilometer
- Automatic fare calculation based on distance
- Update fare rates
- Support for multiple fare entries
- Real-time fare calculator

### 👤 User Management

- Create admin and regular user accounts
- Secure password hashing (SHA-256)
- User authentication system
- Role-based access control
- Manage user profiles

### 📊 Admin Dashboard

- Real-time statistics and metrics
- Quick access to all management sections
- API endpoint overview
- Performance monitoring

---

## 🏗️ Technology Stack

| Component      | Technology                | Version    |
| -------------- | ------------------------- | ---------- |
| Framework      | ASP.NET Core              | 9.0        |
| Database       | SQL Server Express        | Latest     |
| ORM            | Entity Framework Core     | 9.0        |
| Frontend       | Razor Views + Bootstrap 5 | Latest     |
| API            | REST API                  | HTTP/HTTPS |
| Authentication | SHA-256 Password Hashing  | N/A        |

---

## 📋 API Endpoints

### Base URL

```
https://localhost:5001/api
```

### Bus Endpoints

```
GET    /api/bus              - Get all buses
GET    /api/bus/{id}         - Get specific bus
POST   /api/bus              - Create new bus
PUT    /api/bus/{id}         - Update bus
DELETE /api/bus/{id}         - Delete bus
```

### Route Endpoints

```
GET    /api/route            - Get all routes
GET    /api/route/{id}       - Get specific route
POST   /api/route            - Create new route
PUT    /api/route/{id}       - Update route
DELETE /api/route/{id}       - Delete route
```

### Schedule Endpoints

```
GET    /api/schedule         - Get all schedules
GET    /api/schedule/{id}    - Get specific schedule
GET    /api/schedule/bus/{busId}        - Get by bus
GET    /api/schedule/route/{routeId}    - Get by route
POST   /api/schedule         - Create schedule
PUT    /api/schedule/{id}    - Update schedule
DELETE /api/schedule/{id}    - Delete schedule
```

### Fare Endpoints

```
GET    /api/busfare          - Get all fares
GET    /api/busfare/{id}     - Get specific fare
GET    /api/busfare/calculate/{distance} - Calculate fare
POST   /api/busfare          - Create fare
PUT    /api/busfare/{id}     - Update fare
DELETE /api/busfare/{id}     - Delete fare
```

### User Endpoints

```
GET    /api/user             - Get all users
GET    /api/user/{id}        - Get specific user
POST   /api/user/register    - Register new user
POST   /api/user/login       - Login user
PUT    /api/user/{id}        - Update user
DELETE /api/user/{id}        - Delete user
```

---

## 🗄️ Database Schema

### Users Table

```
UserId (PK)
Email (Unique)
PasswordHash
FullName
Role (admin/user)
CreatedAt
```

### Buses Table

```
BusId (PK)
NumberPlate (Unique)
NumberOfSeats
SeatStructure
ConductorNumber
Condition
CreatedAt
UpdatedAt
```

### Routes Table

```
RouteId (PK)
FromLocation
ToLocation
Distance
EstimatedTime
CreatedAt
UpdatedAt
```

### Schedules Table

```
ScheduleId (PK)
BusId (FK)
RouteId (FK)
ScheduledDate
DepartureTime
CreatedAt
UpdatedAt
```

### BusFares Table

```
FareId (PK)
FarePerKm
Description
CreatedAt
UpdatedAt
```

---

## 🚀 Getting Started

### Prerequisites

- .NET 9.0 SDK
- SQL Server Express
- Visual Studio Code or Visual Studio

### Installation

1. **Restore Dependencies**

   ```powershell
   dotnet restore
   ```

2. **Create Migrations**

   ```powershell
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

3. **Run Application**

   ```powershell
   dotnet run
   ```

4. **Access Admin Panel**
   ```
   https://localhost:5001/admin/dashboard
   ```

### Default Admin Credentials

```
Email: admin@gocylon.com
Password: Admin@123
Role: admin
```

---

## 📁 Project Structure

```
GoCeylon/
├── Controllers/
│   ├── BusController.cs
│   ├── RouteController.cs
│   ├── ScheduleController.cs
│   ├── BusFareController.cs
│   ├── UserController.cs
│   ├── AdminController.cs
│   └── HomeController.cs
├── Models/
│   ├── Bus.cs
│   ├── Route.cs
│   ├── Schedule.cs
│   ├── BusFare.cs
│   ├── User.cs
│   └── ErrorViewModel.cs
├── Data/
│   └── GoCyloneDbContext.cs
├── Views/
│   ├── Admin/
│   │   ├── Dashboard.cshtml
│   │   ├── Buses.cshtml
│   │   ├── Routes.cshtml
│   │   ├── Schedules.cshtml
│   │   ├── Fares.cshtml
│   │   └── Users.cshtml
│   ├── Home/
│   └── Shared/
│       └── _Layout.cshtml
├── wwwroot/
│   ├── css/
│   │   └── site.css (Red & Yellow Theme)
│   ├── js/
│   └── lib/
├── Program.cs
├── appsettings.json
├── GoCeylon.csproj
├── API_DOCUMENTATION.md
├── DATABASE_SETUP.sql
├── SETUP_GUIDE.md
└── README.md (this file)
```

---

## 🔧 Configuration

### Connection String

Update in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=LAPTOP-RDNMEQ3T\\SQLEXPRESS;Database=ABCD;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### Application Settings

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

---

## 🎨 User Interface

### Admin Dashboard

- **Statistics Dashboard**: Real-time counts of buses, routes, schedules, and users
- **Quick Action Buttons**: Fast access to all management features
- **API Information**: Display of available endpoints
- **Navigation Menu**: Easy access to all admin sections

### Management Pages

- **Buses Management**: Full CRUD interface with form validation
- **Routes Management**: Create and manage routes with distance calculator
- **Schedules Management**: Schedule buses with date/time pickers
- **Fares Management**: Fare configuration and calculator
- **Users Management**: User registration and role assignment

### Theme

- Red (#DC143C) navigation bar with gradient
- Yellow (#FFD700) accents and text highlights
- Responsive Bootstrap 5 layout
- Card-based interface design
- Interactive data tables

---

## 📱 Flutter Integration Example

```dart
import 'package:http/http.dart' as http;
import 'dart:convert';

class GoCylonAPI {
  static const String baseUrl = 'https://your-server:5001/api';

  // Get all buses
  static Future<List<dynamic>> getAllBuses() async {
    final response = await http.get(
      Uri.parse('$baseUrl/bus'),
      headers: {'Content-Type': 'application/json'},
    );

    if (response.statusCode == 200) {
      final data = jsonDecode(response.body);
      return data['data'] ?? [];
    }
    throw Exception('Failed to load buses');
  }

  // Login
  static Future<Map<String, dynamic>> login(String email, String password) async {
    final response = await http.post(
      Uri.parse('$baseUrl/user/login'),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode({'email': email, 'password': password}),
    );

    if (response.statusCode == 200) {
      return jsonDecode(response.body)['data'] ?? {};
    }
    throw Exception('Login failed');
  }

  // Calculate fare
  static Future<double> calculateFare(double distance) async {
    final response = await http.get(
      Uri.parse('$baseUrl/busfare/calculate/$distance'),
    );

    if (response.statusCode == 200) {
      final data = jsonDecode(response.body);
      return (data['data']['totalFare'] ?? 0).toDouble();
    }
    throw Exception('Fare calculation failed');
  }
}
```

---

## 🧪 Testing

### Unit Testing

Create test file in `Tests/` directory:

```csharp
[TestClass]
public class BusControllerTests
{
    private GoCyloneDbContext _context;
    private BusController _controller;

    [TestInitialize]
    public void Setup()
    {
        // Initialize test context
    }

    [TestMethod]
    public async Task GetBuses_ReturnsAllBuses()
    {
        // Arrange, Act, Assert
    }
}
```

### API Testing with Postman

1. Import endpoints into Postman
2. Set environment variable: `baseUrl = https://localhost:5001/api`
3. Test each endpoint with sample data
4. Verify response format and status codes

### cURL Testing

```bash
# Get all buses
curl -X GET "https://localhost:5001/api/bus" \
  -H "Content-Type: application/json" -k

# Login
curl -X POST "https://localhost:5001/api/user/login" \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@gocylon.com","password":"Admin@123"}' -k

# Calculate fare for 100 km
curl -X GET "https://localhost:5001/api/busfare/calculate/100" -k
```

---

## 🔒 Security Features

1. **Password Hashing**: SHA-256 encryption
2. **HTTPS**: Secure communication over SSL/TLS
3. **Input Validation**: All user inputs validated
4. **Entity Relationships**: Foreign key constraints
5. **Error Handling**: Comprehensive exception handling

### Security Best Practices

- Never commit credentials to repository
- Use environment variables for sensitive data
- Implement JWT tokens for API (future enhancement)
- Enable CORS only for trusted domains
- Regular security audits

---

## 📊 Sample Data

### Default Users

```
Admin: admin@gocylon.com / Admin@123
User1: user1@gocylon.com / Admin@123
User2: user2@gocylon.com / Admin@123
```

### Sample Routes

- Colombo ↔ Kandy (115.5 km)
- Colombo ↔ Galle (140 km)
- Kandy ↔ Nuwara Eliya (52 km)
- Galle ↔ Matara (48 km)

### Sample Buses

- WP-CD-0001 (48 seats, 2×2, AC)
- WP-CD-0002 (50 seats, 2×3, Non-AC)
- WP-CD-0003 (48 seats, 2×2, AC)
- WP-CD-0004 (45 seats, 2×2, Non-AC)

---

## 🐛 Troubleshooting

| Issue                      | Solution                                                      |
| -------------------------- | ------------------------------------------------------------- |
| Database connection failed | Verify SQL Server is running and connection string is correct |
| Migrations failed          | Delete migration file and create new one                      |
| Port already in use        | Change port in `launchSettings.json`                          |
| HTTPS certificate error    | Add `-k` flag to curl or ignore in browser                    |
| API 404 errors             | Check route mappings in `Program.cs`                          |

---

## 📈 Performance Optimization

1. **Database Indexing**: Indexes on frequently queried columns
2. **Async/Await**: Non-blocking operations
3. **Connection Pooling**: Efficient database connections
4. **Caching**: Implement caching for static data
5. **Pagination**: Paginate large datasets

---

## 🔄 Future Enhancements

- [ ] JWT token-based authentication
- [ ] Real-time notifications
- [ ] Advanced analytics and reporting
- [ ] Booking and reservation system
- [ ] Payment gateway integration
- [ ] GPS tracking for buses
- [ ] Mobile app (Flutter)
- [ ] Multi-language support
- [ ] Admin role hierarchy
- [ ] Audit logging

---

## 📚 Documentation

- **API Documentation**: See `API_DOCUMENTATION.md`
- **Setup Guide**: See `SETUP_GUIDE.md`
- **Database Setup**: See `DATABASE_SETUP.sql`
- **Inline Comments**: Code has comprehensive comments

---

## 📞 Support

For issues or questions:

1. Check the comprehensive documentation
2. Review controller comments
3. Check API response messages
4. Verify database connectivity

---

## 📄 License

This project is developed for GoCeylon Bus Management System.

---

## 👏 Acknowledgments

**GoCeylon** celebrates Sri Lankan heritage and modern technology, bringing together the best of both worlds for efficient bus transportation management.

---

<div align="center">

## 🌴 One nation. One route. One app. 🌴

**GoCeylon v1.0.0**

Proudly Sri Lankan | Built with ASP.NET Core 9.0 | Entity Framework Core

_"Combines heritage with modern mobility"_

</div>
