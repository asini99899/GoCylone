# GoCeylon - Bus Management System API

**🌴 GoCeylon: Proudly Sri Lankan. Combines heritage with modern mobility.**  
_Tagline: "One nation. One route. One app."_

## Project Overview

GoCeylon is a comprehensive bus management system built with ASP.NET Core 9.0 and Entity Framework Core. It provides admin capabilities for managing buses, routes, schedules, and fares, with support for both admin and user roles.

### Features

- ✅ **Bus Management**: Add, edit, delete, and view buses with seat configurations
- ✅ **Route Management**: Create and manage routes with distance and estimated time
- ✅ **Schedule Management**: Schedule buses on routes with specific dates and times
- ✅ **Fare Initialization**: Set fare per kilometer and calculate total fares
- ✅ **User Management**: Admin and user role management with authentication
- ✅ **RESTful APIs**: Complete REST API endpoints for Flutter integration
- ✅ **Red & Yellow Theme**: GoCeylon branding throughout the UI

## Database Setup

**Server**: `LAPTOP-RDNMEQ3T\SQLEXPRESS`  
**Database**: `ABCD`

### Connection String

```
Server=LAPTOP-RDNMEQ3T\SQLEXPRESS;Database=ABCD;Trusted_Connection=True;TrustServerCertificate=True;
```

## API Endpoints

### 🚌 Bus Management

#### Get All Buses

```
GET /api/bus
```

**Response**:

```json
{
  "success": true,
  "data": [
    {
      "busId": 1,
      "numberPlate": "WP-CD-1234",
      "numberOfSeats": 48,
      "seatStructure": "2*2",
      "conductorNumber": "C001",
      "condition": "AC",
      "createdAt": "2025-01-15T10:30:00",
      "updatedAt": null
    }
  ],
  "message": "Buses retrieved successfully"
}
```

#### Get Bus by ID

```
GET /api/bus/{id}
```

#### Add Bus

```
POST /api/bus
Content-Type: application/json

{
  "numberPlate": "WP-CD-1234",
  "numberOfSeats": 48,
  "seatStructure": "2*2",
  "conductorNumber": "C001",
  "condition": "AC"
}
```

#### Update Bus

```
PUT /api/bus/{id}
Content-Type: application/json

{
  "busId": 1,
  "numberPlate": "WP-CD-1234",
  "numberOfSeats": 48,
  "seatStructure": "2*2",
  "conductorNumber": "C001",
  "condition": "AC"
}
```

#### Delete Bus

```
DELETE /api/bus/{id}
```

---

### 🛣️ Route Management

#### Get All Routes

```
GET /api/route
```

**Response**:

```json
{
  "success": true,
  "data": [
    {
      "routeId": 1,
      "fromLocation": "Colombo",
      "toLocation": "Kandy",
      "distance": 115.5,
      "estimatedTime": "2h 30m",
      "createdAt": "2025-01-15T10:30:00",
      "updatedAt": null
    }
  ],
  "message": "Routes retrieved successfully"
}
```

#### Get Route by ID

```
GET /api/route/{id}
```

#### Add Route

```
POST /api/route
Content-Type: application/json

{
  "fromLocation": "Colombo",
  "toLocation": "Kandy",
  "distance": 115.5,
  "estimatedTime": "2h 30m"
}
```

#### Update Route

```
PUT /api/route/{id}
Content-Type: application/json

{
  "routeId": 1,
  "fromLocation": "Colombo",
  "toLocation": "Kandy",
  "distance": 115.5,
  "estimatedTime": "2h 30m"
}
```

#### Delete Route

```
DELETE /api/route/{id}
```

---

### 📅 Schedule Management

#### Get All Schedules

```
GET /api/schedule
```

**Response**:

```json
{
  "success": true,
  "data": [
    {
      "scheduleId": 1,
      "busId": 1,
      "routeId": 1,
      "scheduledDate": "2025-01-20T00:00:00",
      "departureTime": "08:30:00",
      "bus": { "busId": 1, "numberPlate": "WP-CD-1234", ... },
      "route": { "routeId": 1, "fromLocation": "Colombo", ... },
      "createdAt": "2025-01-15T10:30:00",
      "updatedAt": null
    }
  ],
  "message": "Schedules retrieved successfully"
}
```

#### Get Schedule by ID

```
GET /api/schedule/{id}
```

#### Get Schedules by Bus

```
GET /api/schedule/bus/{busId}
```

#### Get Schedules by Route

```
GET /api/schedule/route/{routeId}
```

#### Add Schedule

```
POST /api/schedule
Content-Type: application/json

{
  "busId": 1,
  "routeId": 1,
  "scheduledDate": "2025-01-20T00:00:00",
  "departureTime": "08:30:00"
}
```

#### Update Schedule

```
PUT /api/schedule/{id}
Content-Type: application/json

{
  "scheduleId": 1,
  "busId": 1,
  "routeId": 1,
  "scheduledDate": "2025-01-20T00:00:00",
  "departureTime": "08:30:00"
}
```

#### Delete Schedule

```
DELETE /api/schedule/{id}
```

---

### 💰 Fare Management

#### Get All Fares

```
GET /api/busfare
```

**Response**:

```json
{
  "success": true,
  "data": [
    {
      "fareId": 1,
      "farePerKm": 50.0,
      "description": "Base fare per km",
      "createdAt": "2025-01-15T10:30:00",
      "updatedAt": null
    }
  ],
  "message": "Fares retrieved successfully"
}
```

#### Get Fare by ID

```
GET /api/busfare/{id}
```

#### Add Fare

```
POST /api/busfare
Content-Type: application/json

{
  "farePerKm": 50.00,
  "description": "Base fare per km"
}
```

#### Update Fare

```
PUT /api/busfare/{id}
Content-Type: application/json

{
  "fareId": 1,
  "farePerKm": 50.00,
  "description": "Base fare per km"
}
```

#### Calculate Fare

```
GET /api/busfare/calculate/{distance}
```

**Example**: `/api/busfare/calculate/115.5`  
**Response**:

```json
{
  "success": true,
  "data": {
    "distance": 115.5,
    "farePerKm": 50.0,
    "totalFare": 5775.0
  },
  "message": "Fare calculated successfully"
}
```

#### Delete Fare

```
DELETE /api/busfare/{id}
```

---

### 👤 User Management

#### Get All Users

```
GET /api/user
```

**Response**:

```json
{
  "success": true,
  "data": [
    {
      "userId": 1,
      "email": "admin@gocylon.com",
      "fullName": "Admin User",
      "role": "admin",
      "createdAt": "2025-01-15T10:30:00"
    }
  ],
  "message": "Users retrieved successfully"
}
```

#### Get User by ID

```
GET /api/user/{id}
```

#### Register User

```
POST /api/user/register
Content-Type: application/json

{
  "email": "user@gocylon.com",
  "password": "SecurePassword123",
  "fullName": "John Doe",
  "role": "user"
}
```

#### Login User

```
POST /api/user/login
Content-Type: application/json

{
  "email": "admin@gocylon.com",
  "password": "SecurePassword123"
}
```

**Response**:

```json
{
  "success": true,
  "data": {
    "userId": 1,
    "email": "admin@gocylon.com",
    "fullName": "Admin User",
    "role": "admin"
  },
  "message": "Login successful"
}
```

#### Update User

```
PUT /api/user/{id}
Content-Type: application/json

{
  "fullName": "John Doe Updated",
  "role": "user"
}
```

#### Delete User

```
DELETE /api/user/{id}
```

---

## Data Models

### Bus

```csharp
{
  "busId": int,
  "numberPlate": string,
  "numberOfSeats": int,
  "seatStructure": string, // "2*2" or "2*3"
  "conductorNumber": string,
  "condition": string, // "AC" or "Non-AC"
  "createdAt": DateTime,
  "updatedAt": DateTime?
}
```

### Route

```csharp
{
  "routeId": int,
  "fromLocation": string,
  "toLocation": string,
  "distance": decimal,
  "estimatedTime": string,
  "createdAt": DateTime,
  "updatedAt": DateTime?
}
```

### Schedule

```csharp
{
  "scheduleId": int,
  "busId": int,
  "routeId": int,
  "scheduledDate": DateTime,
  "departureTime": TimeSpan,
  "createdAt": DateTime,
  "updatedAt": DateTime?
}
```

### BusFare

```csharp
{
  "fareId": int,
  "farePerKm": decimal,
  "description": string,
  "createdAt": DateTime,
  "updatedAt": DateTime?
}
```

### User

```csharp
{
  "userId": int,
  "email": string,
  "passwordHash": string,
  "role": string, // "admin" or "user"
  "fullName": string,
  "createdAt": DateTime
}
```

---

## Project Structure

```
GoCeylon/
├── Controllers/
│   ├── BusController.cs
│   ├── RouteController.cs
│   ├── ScheduleController.cs
│   ├── BusFareController.cs
│   ├── UserController.cs
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
│   ├── Home/
│   ├── Shared/
│   └── _Layout.cshtml
├── wwwroot/
│   ├── css/
│   │   └── site.css (Red & Yellow Theme)
│   ├── js/
│   └── lib/
├── Program.cs
├── appsettings.json
└── GoCeylon.csproj
```

---

## Color Scheme

- **Primary Red**: `#DC143C`
- **Primary Yellow**: `#FFD700`
- **Dark Red**: `#8B0000`

---

## How to Run

1. **Restore NuGet packages**:

   ```powershell
   dotnet restore
   ```

2. **Update Database** (Create migrations if needed):

   ```powershell
   dotnet ef database update
   ```

3. **Run the application**:

   ```powershell
   dotnet run
   ```

4. **Access the API**:
   - Base URL: `https://localhost:5001`
   - API URLs: `https://localhost:5001/api/bus`, etc.

---

## Flutter App Integration

Use these endpoints to integrate with your Flutter app:

**Example Dart/Flutter code**:

```dart
import 'package:http/http.dart' as http;
import 'dart:convert';

class GoCylonAPI {
  static const String baseUrl = 'https://your-server:5001/api';

  // Get all buses
  static Future<List<Bus>> getAllBuses() async {
    final response = await http.get(Uri.parse('$baseUrl/bus'));
    if (response.statusCode == 200) {
      final data = json.decode(response.body);
      return (data['data'] as List).map((b) => Bus.fromJson(b)).toList();
    }
    throw Exception('Failed to load buses');
  }

  // Add a new bus
  static Future<Bus> addBus(Bus bus) async {
    final response = await http.post(
      Uri.parse('$baseUrl/bus'),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode(bus.toJson()),
    );
    if (response.statusCode == 201) {
      return Bus.fromJson(json.decode(response.body)['data']);
    }
    throw Exception('Failed to add bus');
  }
}
```

---

## Default Admin User Setup

After running migrations, create the default admin:

```
Email: admin@gocylon.com
Password: Admin@123
Role: admin
```

---

## Version

**GoCeylon v1.0.0**  
Built with ASP.NET Core 9.0 | Entity Framework Core 9.0

---

**Proudly Sri Lankan. One nation. One route. One app. 🌴**
