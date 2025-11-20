# GoCeylon - Flutter App Integration Guide

## Quick Reference for Flutter Developers

This guide provides essential information for integrating the GoCeylon backend APIs with your Flutter application.

---

## 🔗 API Base URL

```
https://localhost:5001/api
```

**For Production**: Replace `localhost:5001` with your server address

---

## 🎨 Theme Colors

Add these to your Flutter `pubspec.yaml`:

```yaml
# Material 3 Colors
colors:
  primary: Color(0xFFDC143C) # Red
  secondary: Color(0xFFFFD700) # Yellow
  tertiary: Color(0xFF8B0000) # Dark Red
```

Or create a theme class:

```dart
class GoCylonTheme {
  static const Color primaryRed = Color(0xFFDC143C);
  static const Color primaryYellow = Color(0xFFFFD700);
  static const Color darkRed = Color(0xFF8B0000);

  static ThemeData lightTheme = ThemeData(
    primaryColor: primaryRed,
    colorScheme: ColorScheme.light(
      primary: primaryRed,
      secondary: primaryYellow,
    ),
  );
}
```

---

## 🚀 Key API Implementations

### 1. User Authentication

```dart
// User login
Future<void> loginUser(String email, String password) async {
  try {
    final response = await http.post(
      Uri.parse('$baseUrl/user/login'),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode({'email': email, 'password': password}),
    );

    if (response.statusCode == 200) {
      final data = jsonDecode(response.body);
      if (data['success']) {
        // Save user data
        _user = data['data'];
        notifyListeners();
      }
    } else {
      throw Exception('Login failed');
    }
  } catch (e) {
    print('Error: $e');
  }
}

// User registration
Future<void> registerUser(String email, String password, String fullName) async {
  try {
    final response = await http.post(
      Uri.parse('$baseUrl/user/register'),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode({
        'email': email,
        'password': password,
        'fullName': fullName,
        'role': 'user'
      }),
    );

    if (response.statusCode == 201) {
      final data = jsonDecode(response.body);
      if (data['success']) {
        // Registration successful
        notifyListeners();
      }
    }
  } catch (e) {
    print('Error: $e');
  }
}
```

### 2. Get All Buses

```dart
Future<List<Bus>> getAllBuses() async {
  try {
    final response = await http.get(
      Uri.parse('$baseUrl/bus'),
      headers: {'Content-Type': 'application/json'},
    );

    if (response.statusCode == 200) {
      final data = jsonDecode(response.body);
      if (data['success']) {
        List<Bus> buses = (data['data'] as List)
            .map((bus) => Bus.fromJson(bus))
            .toList();
        return buses;
      }
    }
    throw Exception('Failed to load buses');
  } catch (e) {
    print('Error: $e');
    return [];
  }
}
```

**Bus Model**:

```dart
class Bus {
  final int busId;
  final String numberPlate;
  final int numberOfSeats;
  final String seatStructure;
  final String conductorNumber;
  final String condition;
  final DateTime createdAt;

  Bus({
    required this.busId,
    required this.numberPlate,
    required this.numberOfSeats,
    required this.seatStructure,
    required this.conductorNumber,
    required this.condition,
    required this.createdAt,
  });

  factory Bus.fromJson(Map<String, dynamic> json) {
    return Bus(
      busId: json['busId'],
      numberPlate: json['numberPlate'],
      numberOfSeats: json['numberOfSeats'],
      seatStructure: json['seatStructure'],
      conductorNumber: json['conductorNumber'],
      condition: json['condition'],
      createdAt: DateTime.parse(json['createdAt']),
    );
  }
}
```

### 3. Get All Routes

```dart
Future<List<Route>> getAllRoutes() async {
  try {
    final response = await http.get(
      Uri.parse('$baseUrl/route'),
      headers: {'Content-Type': 'application/json'},
    );

    if (response.statusCode == 200) {
      final data = jsonDecode(response.body);
      if (data['success']) {
        List<Route> routes = (data['data'] as List)
            .map((route) => Route.fromJson(route))
            .toList();
        return routes;
      }
    }
    throw Exception('Failed to load routes');
  } catch (e) {
    print('Error: $e');
    return [];
  }
}
```

**Route Model**:

```dart
class Route {
  final int routeId;
  final String fromLocation;
  final String toLocation;
  final double distance;
  final String estimatedTime;

  Route({
    required this.routeId,
    required this.fromLocation,
    required this.toLocation,
    required this.distance,
    required this.estimatedTime,
  });

  factory Route.fromJson(Map<String, dynamic> json) {
    return Route(
      routeId: json['routeId'],
      fromLocation: json['fromLocation'],
      toLocation: json['toLocation'],
      distance: json['distance'],
      estimatedTime: json['estimatedTime'],
    );
  }
}
```

### 4. Get Schedules

```dart
Future<List<Schedule>> getSchedulesByRoute(int routeId) async {
  try {
    final response = await http.get(
      Uri.parse('$baseUrl/schedule/route/$routeId'),
      headers: {'Content-Type': 'application/json'},
    );

    if (response.statusCode == 200) {
      final data = jsonDecode(response.body);
      if (data['success']) {
        List<Schedule> schedules = (data['data'] as List)
            .map((schedule) => Schedule.fromJson(schedule))
            .toList();
        return schedules;
      }
    }
    throw Exception('Failed to load schedules');
  } catch (e) {
    print('Error: $e');
    return [];
  }
}
```

**Schedule Model**:

```dart
class Schedule {
  final int scheduleId;
  final int busId;
  final int routeId;
  final DateTime scheduledDate;
  final TimeOfDay departureTime;
  final Bus? bus;
  final Route? route;

  Schedule({
    required this.scheduleId,
    required this.busId,
    required this.routeId,
    required this.scheduledDate,
    required this.departureTime,
    this.bus,
    this.route,
  });

  factory Schedule.fromJson(Map<String, dynamic> json) {
    return Schedule(
      scheduleId: json['scheduleId'],
      busId: json['busId'],
      routeId: json['routeId'],
      scheduledDate: DateTime.parse(json['scheduledDate']),
      departureTime: _parseTimeOfDay(json['departureTime']),
      bus: json['bus'] != null ? Bus.fromJson(json['bus']) : null,
      route: json['route'] != null ? Route.fromJson(json['route']) : null,
    );
  }

  static TimeOfDay _parseTimeOfDay(String time) {
    final parts = time.split(':');
    return TimeOfDay(
      hour: int.parse(parts[0]),
      minute: int.parse(parts[1]),
    );
  }
}
```

### 5. Calculate Fare

```dart
Future<double> calculateFare(double distance) async {
  try {
    final response = await http.get(
      Uri.parse('$baseUrl/busfare/calculate/$distance'),
      headers: {'Content-Type': 'application/json'},
    );

    if (response.statusCode == 200) {
      final data = jsonDecode(response.body);
      if (data['success']) {
        return (data['data']['totalFare'] as num).toDouble();
      }
    }
    throw Exception('Failed to calculate fare');
  } catch (e) {
    print('Error: $e');
    return 0.0;
  }
}
```

---

## 📦 Required Packages

Add to `pubspec.yaml`:

```yaml
dependencies:
  flutter:
    sdk: flutter
  http: ^1.1.0
  provider: ^6.0.0
  intl: ^0.19.0
  cached_network_image: ^3.3.0
  google_maps_flutter: ^2.5.0 # For bus tracking (optional)
```

---

## 🗂️ Recommended Flutter Project Structure

```
lib/
├── models/
│   ├── bus.dart
│   ├── route.dart
│   ├── schedule.dart
│   ├── user.dart
│   └── fare.dart
├── services/
│   └── api_service.dart
├── providers/
│   ├── bus_provider.dart
│   ├── route_provider.dart
│   ├── schedule_provider.dart
│   └── auth_provider.dart
├── screens/
│   ├── login_screen.dart
│   ├── home_screen.dart
│   ├── buses_screen.dart
│   ├── routes_screen.dart
│   ├── schedule_details_screen.dart
│   └── booking_screen.dart
├── widgets/
│   ├── bus_card.dart
│   ├── route_card.dart
│   └── schedule_card.dart
├── constants/
│   ├── colors.dart
│   ├── strings.dart
│   └── api_endpoints.dart
└── main.dart
```

---

## 🔐 API Service Class Example

```dart
import 'package:http/http.dart' as http;
import 'dart:convert';

class ApiService {
  static const String baseUrl = 'https://localhost:5001/api';

  // Certificate handling for development
  static final HttpClient httpClient = HttpClient()
    ..badCertificateCallback = (X509Certificate cert, String host, int port) => true;

  static Future<Map<String, dynamic>> get(String endpoint) async {
    try {
      final response = await http.get(
        Uri.parse('$baseUrl$endpoint'),
        headers: {'Content-Type': 'application/json'},
      );

      if (response.statusCode >= 200 && response.statusCode < 300) {
        return jsonDecode(response.body);
      } else {
        throw Exception('API Error: ${response.statusCode}');
      }
    } catch (e) {
      print('Error: $e');
      return {'success': false, 'message': e.toString()};
    }
  }

  static Future<Map<String, dynamic>> post(
    String endpoint,
    Map<String, dynamic> body,
  ) async {
    try {
      final response = await http.post(
        Uri.parse('$baseUrl$endpoint'),
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode(body),
      );

      if (response.statusCode >= 200 && response.statusCode < 300) {
        return jsonDecode(response.body);
      } else {
        throw Exception('API Error: ${response.statusCode}');
      }
    } catch (e) {
      print('Error: $e');
      return {'success': false, 'message': e.toString()};
    }
  }
}
```

---

## 🎯 Common Use Cases

### List All Buses in UI

```dart
Consumer<BusProvider>(
  builder: (context, busProvider, child) {
    return FutureBuilder<List<Bus>>(
      future: busProvider.getAllBuses(),
      builder: (context, snapshot) {
        if (snapshot.connectionState == ConnectionState.waiting) {
          return const Center(child: CircularProgressIndicator());
        }

        if (snapshot.hasError) {
          return Center(child: Text('Error: ${snapshot.error}'));
        }

        final buses = snapshot.data ?? [];
        return ListView.builder(
          itemCount: buses.length,
          itemBuilder: (context, index) {
            final bus = buses[index];
            return BusCard(bus: bus);
          },
        );
      },
    );
  },
)
```

### Search Routes

```dart
Future<List<Route>> searchRoutes(String fromLocation) async {
  final allRoutes = await getAllRoutes();
  return allRoutes
      .where((route) =>
          route.fromLocation.toLowerCase().contains(fromLocation.toLowerCase()))
      .toList();
}
```

### Display Fare Quote

```dart
class FareQuote extends StatefulWidget {
  @override
  State<FareQuote> createState() => _FareQuoteState();
}

class _FareQuoteState extends State<FareQuote> {
  double calculatedFare = 0;
  bool isLoading = false;

  void calculateFare(double distance) async {
    setState(() => isLoading = true);
    final fare = await ApiService.calculateFare(distance);
    setState(() {
      calculatedFare = fare;
      isLoading = false;
    });
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        TextField(
          onChanged: (value) {
            if (value.isNotEmpty) {
              calculateFare(double.parse(value));
            }
          },
          decoration: InputDecoration(
            label: Text('Distance (km)'),
          ),
        ),
        if (isLoading) CircularProgressIndicator(),
        if (calculatedFare > 0)
          Text(
            'Fare: Rs. ${calculatedFare.toStringAsFixed(2)}',
            style: TextStyle(
              color: Color(0xFFDC143C),
              fontSize: 18,
              fontWeight: FontWeight.bold,
            ),
          ),
      ],
    );
  }
}
```

---

## 🧪 Testing APIs

### Unit Test Example

```dart
import 'package:flutter_test/flutter_test.dart';
import 'package:your_app/services/api_service.dart';

void main() {
  group('API Service Tests', () {
    test('Get all buses returns list', () async {
      final response = await ApiService.get('/bus');
      expect(response['success'], true);
      expect(response['data'], isA<List>());
    });

    test('Login with valid credentials returns user', () async {
      final response = await ApiService.post('/user/login', {
        'email': 'admin@gocylon.com',
        'password': 'Admin@123',
      });
      expect(response['success'], true);
      expect(response['data'], isNotNull);
    });
  });
}
```

---

## 🚨 Error Handling

```dart
Future<void> handleApiError(dynamic response) {
  if (response is Map && response['success'] == false) {
    String message = response['message'] ?? 'An error occurred';

    // Show snackbar or alert
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(message),
        backgroundColor: Color(0xFFDC143C),
      ),
    );
  }
}
```

---

## 📱 Sample App Structure

```dart
void main() {
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return MultiProvider(
      providers: [
        ChangeNotifierProvider(create: (_) => AuthProvider()),
        ChangeNotifierProvider(create: (_) => BusProvider()),
        ChangeNotifierProvider(create: (_) => RouteProvider()),
        ChangeNotifierProvider(create: (_) => ScheduleProvider()),
      ],
      child: MaterialApp(
        title: 'GoCeylon',
        theme: ThemeData(
          primaryColor: Color(0xFFDC143C),
          colorScheme: ColorScheme.light(
            primary: Color(0xFFDC143C),
            secondary: Color(0xFFFFD700),
          ),
        ),
        home: const LoginScreen(),
      ),
    );
  }
}
```

---

## 📚 Additional Resources

- Full API Documentation: See `API_DOCUMENTATION.md` in backend
- Database Schema: See `DATABASE_SETUP.sql`
- Backend Setup: See `SETUP_GUIDE.md`

---

## 🔗 Quick Links

- **API Base URL**: https://localhost:5001/api
- **Admin Dashboard**: https://localhost:5001/admin/dashboard
- **Default Admin Email**: admin@gocylon.com
- **Default Admin Password**: Admin@123

---

## 💡 Tips

1. Store user token in SharedPreferences after login
2. Implement connection timeout for API calls
3. Cache bus and route lists locally
4. Use provider pattern for state management
5. Implement proper error handling
6. Add loading indicators for async operations
7. Use models for type safety
8. Validate user input before API calls

---

<div align="center">

**GoCeylon - Flutter Integration Guide v1.0**

_"One nation. One route. One app."_ 🌴

</div>
