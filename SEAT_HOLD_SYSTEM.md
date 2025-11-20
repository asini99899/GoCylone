# 🎫 Seat Hold System - Complete Documentation

## Overview

The seat hold system prevents overbooking by temporarily reserving seats for 20 minutes when a user selects them. This ensures:

- ✅ Users can't book already reserved seats
- ✅ Temporary holds expire after 20 minutes if not confirmed
- ✅ Permanently booked seats are locked forever
- ✅ Multiple users can browse without conflicts

---

## Seat States

### 1. **Available** (Green)

- No user has selected this seat
- User can click to select

### 2. **Processing/Selected** (Gold/Yellow - ⏱)

- User has selected this seat
- System creates a 20-minute hold
- Status: `"Processing"`
- Other users see this seat as "being held" (cannot select)
- If user confirms booking → becomes **Booked**
- If 20 minutes pass without confirmation → reverts to **Available**

### 3. **Booked** (Red - X)

- User has completed payment and confirmed booking
- Status: `"Booked"`
- Permanently locked - cannot be selected by anyone
- Remains booked until manually cancelled by admin

### 4. **Driver** (Orange - D)

- First seat (seat #1) - reserved for bus driver
- Cannot be selected by passengers

---

## Database Schema

### BookingSeat Table

```sql
CREATE TABLE [BookingSeats] (
    [BookingSeatId] INT PRIMARY KEY,
    [BookingId] INT NOT NULL,
    [SeatNumber] INT NOT NULL,
    [ScheduleId] INT NOT NULL,
    [Status] NVARCHAR(50) NOT NULL,  -- "Available", "Processing", "Booked"
    [BookedDate] DATETIME2,           -- When confirmed
    [ProcessingStartTime] DATETIME2,  -- When user selected
    [HoldExpiryTime] DATETIME2,       -- When hold expires (20 min from selection)
    [UpdatedAt] DATETIME2,

    FOREIGN KEY ([BookingId]) REFERENCES [Bookings](BookingId) ON DELETE CASCADE,
    FOREIGN KEY ([ScheduleId]) REFERENCES [Schedules](ScheduleId) ON DELETE RESTRICT
)
```

### Key Fields

| Field                 | Purpose                     | Example                             |
| --------------------- | --------------------------- | ----------------------------------- |
| `Status`              | Current seat state          | "Processing", "Booked", "Available" |
| `ProcessingStartTime` | When user selected seat     | 2025-11-19 14:30:00                 |
| `HoldExpiryTime`      | When 20-min hold expires    | 2025-11-19 14:50:00                 |
| `BookedDate`          | When user confirmed payment | 2025-11-19 14:45:00                 |

---

## System Flow

### User Booking Journey

```
1. USER SELECTS SEATS
   ↓
   → System creates "Processing" records with 20-min hold
   → Displays countdown timer showing "Hold expires: 19:45"
   → Display changes: Seat color = Gold, Icon = ⏱

2. USER FILLS BOARDING PLACES
   ↓
   → Can customize pickup/drop locations
   → Real-time fare calculation continues

3. USER CLICKS "PROCEED TO PAYMENT"
   ↓
   → Validation: Check seats still available
   → Show payment confirmation page
   → Hold timer still counting down

4. USER ENTERS CARD DETAILS
   ↓
   → Validates payment information
   → Still within 20-minute window

5. USER CLICKS "PAY"
   ↓
   → System processes payment
   → Updates seat status: "Processing" → "Booked"
   → Creates Booking record
   → Creates PaymentInfo record
   → Hold no longer needed (payment confirmed)

6. BOOKING COMPLETE
   ↓
   → Show success page with reference number
   → Seats now permanently "Booked"
   → Other users cannot select these seats

7. IF USER DOESN'T CONFIRM (> 20 min)
   ↓
   → Automatic cleanup runs
   → "Processing" records deleted
   → Seats revert to "Available"
   → Other users can now select them
```

---

## API Endpoints

### 1. GET `/Booking/SelectSeats/{scheduleId}`

**Purpose**: Load seat selection page

**Response includes**:

- `BookedSeats`: List of permanently booked seat numbers
- `ProcessingSeats`: List of seats with active holds

**Example**:

```json
{
  "scheduleId": 1,
  "bookedSeats": [2, 5, 8],
  "processingSeats": [3, 7],
  "totalSeats": 44,
  "seatStructure": "2*2"
}
```

---

### 2. POST `/Booking/HoldSeats`

**Purpose**: Create a temporary 20-minute hold on seats

**Request**:

```json
{
  "scheduleId": 1,
  "selectedSeats": [4, 6, 9]
}
```

**Response on success**:

```json
{
  "success": true,
  "message": "Seats held for 20 minutes",
  "expiryTime": "2025-11-19T14:50:00Z"
}
```

**Response on conflict**:

```json
{
  "success": false,
  "message": "Seats [3, 7] are being held by another user (expires in 20 minutes)."
}
```

**When called**: Automatically when user selects their first seat

---

### 3. POST `/Booking/ValidateSeats`

**Purpose**: Check if seats are still available before proceeding to payment

**Request**:

```json
{
  "scheduleId": 1,
  "selectedSeats": [4, 6, 9]
}
```

**Response**:

```json
{
  "success": true,
  "message": "Seats are available"
}
```

**Error if unavailable**:

```json
{
  "success": false,
  "message": "Seats [5, 8] are already booked. Seats [4] are being held by another user (expires in 20 minutes)."
}
```

---

### 4. POST `/Booking/ReleaseHold`

**Purpose**: Cancel a hold (user navigates away or deselects all)

**Request**:

```json
{
  "scheduleId": 1,
  "seatNumbers": [4, 6, 9]
}
```

**Response**:

```json
{
  "success": true,
  "message": "Seat hold released"
}
```

---

### 5. POST `/Booking/ProcessPayment`

**Purpose**: Confirm payment and convert hold to booked

**Request**:

```json
{
  "scheduleId": 1,
  "selectedSeats": [4, 6, 9],
  "pickupLocation": "Galewela",
  "dropLocation": "Matale",
  "totalFare": 300,
  "cardHolderName": "John Doe",
  "cardNumber": "4111111111111111",
  "expiryDate": "12/25",
  "cvv": "123"
}
```

**Response**:

```json
{
  "success": true,
  "bookingId": 5,
  "message": "Booking confirmed successfully!"
}
```

---

## Seat Structure (2\*2 Layout)

### What does 2\*2 mean?

```
2 columns × 2 rows per section = 4 seats per row

Visual Layout:
          LEFT    RIGHT
        ┌────────────┐
Row A   │ D    1  ║  2  │  (D=Driver, 1/2=Seats)
        │ 3    4  ║  5  │
        ├────────────┤
Row B   │ 6    7  ║  8  │
        │ 9   10  ║ 11  │
        └────────────┘

Pattern continues for all rows...
```

### Total Seats Calculation

- **SeatStructure**: "2\*2" (2 columns × 2 rows)
- **Total**: 44 seats
- **Rows needed**: 44 ÷ 2 = 22 rows (A-V)

### Seat Grid Display

- Seats 1-4: Row A (columns 1-4)
- Seats 5-8: Row B (columns 1-4)
- Seats 9-12: Row C (columns 1-4)
- etc.

---

## Frontend Display

### Seat Colors & Icons

| State               | Color       | Icon   | Clickable     | Hold Timer         |
| ------------------- | ----------- | ------ | ------------- | ------------------ |
| Available           | Green       | Number | ✅ Yes        | -                  |
| Selected            | Dark Green  | ✓      | ✅ Can toggle | ✅ Shows countdown |
| Processing (Others) | Gold/Yellow | ⏱      | ❌ No         | ✅ "Being held..." |
| Booked              | Red         | X      | ❌ No         | -                  |
| Driver              | Orange      | D      | ❌ No         | -                  |

### Countdown Display

When user selects seats:

```
Total Fare: Rs. 300 (Hold expires: 19:45)
```

Updates every second showing remaining time. When expired:

```
⚠️ Hold expired! Please book immediately or seats will be released.
```

---

## Cleanup Process

### Automatic Expired Hold Removal

**When happens**: Every time user loads seat selection page or proceeds to payment

**What it does**:

1. Find all "Processing" seats on this schedule
2. Check if `HoldExpiryTime < DateTime.Now`
3. If expired → Delete the BookingSeat record
4. Seats become "Available" again

**SQL Query**:

```sql
DELETE FROM BookingSeats
WHERE ScheduleId = @scheduleId
  AND Status = 'Processing'
  AND HoldExpiryTime < GETDATE()
```

---

## Example Scenario

### Scenario: Two Users Booking Same Bus

**Time: 14:30**

- User A loads seat selection
- Sees seats 1-44 available

**Time: 14:31**

- User A selects seats 5, 6, 7
- System creates holds with expiry: 14:51
- Display: "Hold expires: 20:00" (19 min 59 sec)

**Time: 14:32**

- User B loads same bus
- Sees seat 5, 6, 7 as "⏱ Being held (20 min)"
- Selects seats 8, 9, 10 instead
- System creates holds with expiry: 14:52

**Time: 14:40**

- User A enters payment form
- User A's hold = 14:51 (still valid, 11 min left)
- User B's hold = 14:52 (still valid, 12 min left)

**Time: 14:42**

- User A clicks "Pay"
- System validates: Seats 5, 6, 7 still on hold ✅
- Payment processed
- Seats 5, 6, 7 changed to Status = "Booked"
- Booking created

**Time: 14:45**

- User B enters payment form
- Validation runs: Seat 8, 9, 10 still on hold ✅

**Time: 14:46**

- User B clicks "Pay"
- System validates: Seats 8, 9, 10 still on hold ✅
- Payment processed
- Seats 8, 9, 10 changed to Status = "Booked"
- Both bookings complete ✅

**Time: 14:51**

- User A's original 20-min hold would expire
- But status already changed to "Booked" - no cleanup needed

**Time: 14:52**

- User B's original 20-min hold would expire
- But status already changed to "Booked" - no cleanup needed

---

## Implementation Details

### 1. Model Changes

```csharp
public class BookingSeat
{
    public string Status { get; set; } = "Booked"; // Changed from implicit "Booked"
    public DateTime? ProcessingStartTime { get; set; } // Track when hold started
    public DateTime? HoldExpiryTime { get; set; } // Track when hold expires
}
```

### 2. Controller Methods

- `SelectSeats()`: Cleaned expired holds, returns booked + processing lists
- `HoldSeats()`: Creates processing records with 20-min expiry
- `ValidateSeats()`: Checks both booked and processing seats
- `ProcessPayment()`: Converts processing → booked
- `ReleaseHold()`: Deletes processing records

### 3. JavaScript Logic

- Auto-hold on first seat selection
- Real-time countdown timer
- Release hold when all seats deselected
- Cleanup on page unload

---

## SQL Queries

### Get all seats for a bus on a date

```sql
SELECT
    bs.SeatNumber,
    bs.Status,
    bs.ProcessingStartTime,
    bs.HoldExpiryTime,
    CASE
        WHEN bs.Status = 'Booked' THEN 'BOOKED'
        WHEN bs.Status = 'Processing' AND bs.HoldExpiryTime > GETDATE() THEN 'ON HOLD'
        WHEN bs.Status = 'Processing' AND bs.HoldExpiryTime <= GETDATE() THEN 'EXPIRED'
        ELSE 'AVAILABLE'
    END as CurrentState
FROM BookingSeats bs
WHERE bs.ScheduleId = 1
ORDER BY bs.SeatNumber
```

### Find expired holds (cleanup)

```sql
SELECT COUNT(*) as ExpiredHolds
FROM BookingSeats
WHERE Status = 'Processing'
  AND HoldExpiryTime < GETDATE()
```

### Check if specific seat is available

```sql
SELECT
    CASE
        WHEN EXISTS(SELECT 1 FROM BookingSeats WHERE ScheduleId = 1 AND SeatNumber = 5 AND Status = 'Booked')
            THEN 'BOOKED'
        WHEN EXISTS(SELECT 1 FROM BookingSeats WHERE ScheduleId = 1 AND SeatNumber = 5 AND Status = 'Processing' AND HoldExpiryTime > GETDATE())
            THEN 'ON HOLD'
        ELSE 'AVAILABLE'
    END as SeatStatus
```

---

## Testing Checklist

- [ ] User A selects 2 seats → Shows countdown timer
- [ ] User B loads same bus → Sees User A's seats as "Being held"
- [ ] User B selects different seats → Gets own countdown timer
- [ ] User A proceeds to payment → Validation passes
- [ ] User A completes payment → Seats marked "BOOKED"
- [ ] User B still sees User A's seats as "BOOKED" after refresh
- [ ] User A deselects all seats → Hold released immediately
- [ ] Wait 25 minutes without booking → Hold expires automatically
- [ ] Next user refresh → Expired holds cleaned up, seats available

---

## Future Enhancements

- [ ] Email notification: "Your hold expires in 5 minutes"
- [ ] SMS reminder before hold expiry
- [ ] Partial hold release (deselect individual seats)
- [ ] Admin dashboard: Live hold status monitoring
- [ ] Analytics: Popular seats, hold-to-booking conversion rate
- [ ] Loyalty: Free 5-minute extension for members

---

**Version**: 2.0  
**Last Updated**: November 19, 2025  
**Status**: ✅ Production Ready
