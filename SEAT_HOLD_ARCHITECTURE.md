# 🎫 Seat Hold System - Visual Architecture

## 🏗️ System Architecture

```
┌─────────────────────────────────────────────────────────────────────┐
│                         FRONTEND (HTML/CSS/JS)                      │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  ┌──────────────────────────────────────────────────────────────┐ │
│  │ SelectSeats.cshtml                                           │ │
│  │                                                              │ │
│  │ ┌─ Seat Grid (Dynamic)         ┌─ Countdown Timer ────────┐ │ │
│  │ │ A │ D  │ 1  ║  2  │           │ Total: Rs. 300           │ │ │
│  │ │ B │ 3  │ 4  ║  5  │           │ Hold: 19:45              │ │ │
│  │ │ C │ 6✓ │ 7⏱ ║  8  │           └──────────────────────────┘ │ │
│  │ │ D │ 9  │ 10 ║ 11  │                                       │ │
│  │ └─ Colors: Green(Avail), Gold(Hold), Red(Booked), Orange(D)   │ │
│  │                                                              │ │
│  │ ┌─ Boarding Section ──────────────────────────────────────┐ │ │
│  │ │ From: [Galewela__________]   To: [Matale____________]  │ │ │
│  │ └────────────────────────────────────────────────────────┘ │ │
│  └──────────────────────────────────────────────────────────────┘ │
│                            ↓ JavaScript                           │
│                                                                     │
│                    generateSeats() → Renders grid                  │
│                    toggleSeat() → Calls HoldSeats API             │
│                    updateUI() → Updates display                    │
│                    countdown() → Updates timer                     │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
                                ↓ HTTP/FETCH
┌─────────────────────────────────────────────────────────────────────┐
│                      BACKEND (ASP.NET Core)                        │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  BookingController                                                 │
│  ├─ GET /Booking/SelectSeats/{id}                                 │
│  │  ├─ Cleanup expired holds                                      │
│  │  ├─ Query booked seats (Status="Booked")                       │
│  │  ├─ Query processing seats (Status="Processing", expiry>now)   │
│  │  └─ Return SelectSeatsViewModel                                │
│  │                                                                │
│  ├─ POST /Booking/HoldSeats                                       │
│  │  ├─ Validate selected seats                                    │
│  │  ├─ Create BookingSeat records with Status="Processing"        │
│  │  ├─ Set HoldExpiryTime = now + 20 min                         │
│  │  └─ Return success + expiryTime                                │
│  │                                                                │
│  ├─ POST /Booking/ValidateSeats                                   │
│  │  ├─ Cleanup expired holds                                      │
│  │  ├─ Check for booked conflicts                                 │
│  │  ├─ Check for processing conflicts                             │
│  │  └─ Return success or detailed error                           │
│  │                                                                │
│  ├─ POST /Booking/ProcessPayment                                  │
│  │  ├─ Cleanup expired holds                                      │
│  │  ├─ Final validation of seats                                  │
│  │  ├─ Create Booking record                                      │
│  │  ├─ Update BookingSeat Status: PROCESSING → BOOKED             │
│  │  ├─ Create PaymentInfo record                                  │
│  │  └─ Return bookingId + success                                 │
│  │                                                                │
│  └─ POST /Booking/ReleaseHold                                     │
│     ├─ Delete processing BookingSeat records                      │
│     └─ Return success                                             │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
                                ↓ Entity Framework Core
┌─────────────────────────────────────────────────────────────────────┐
│                    DATABASE (SQL Server)                           │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  ┌──── BookingSeats Table ──────────────────────────────────────┐ │
│  │ PK  BookingSeatId        INT                                │ │
│  │ FK  BookingId             INT                               │ │
│  │     SeatNumber            INT (1-44)                        │ │
│  │ FK  ScheduleId            INT                               │ │
│  │ NEW Status                NVARCHAR    ← Track state         │ │
│  │ NEW ProcessingStartTime   DATETIME2   ← When hold started   │ │
│  │ NEW HoldExpiryTime        DATETIME2   ← When hold expires   │ │
│  │     BookedDate            DATETIME2   ← When confirmed      │ │
│  └──────────────────────────────────────────────────────────────┘ │
│                                                                     │
│  ┌──── Bookings Table ──────────────────────────────────────────┐ │
│  │ PK BookingId                                                │ │
│  │ FK UserId, ScheduleId                                       │ │
│  │   NumberOfSeats, TotalFare, Status                          │ │
│  │   PickupLocation, DropLocation, BookingDate                 │ │
│  └──────────────────────────────────────────────────────────────┘ │
│                                                                     │
│  ┌──── PaymentInfos Table ──────────────────────────────────────┐ │
│  │ PK PaymentId                                                │ │
│  │ FK BookingId (1:1)                                          │ │
│  │   CardHolderName, CardNumber (masked), CVV (***)            │ │
│  │   Amount, PaymentStatus, TransactionId, PaymentDate         │ │
│  └──────────────────────────────────────────────────────────────┘ │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

---

## 🔄 Seat State Machine

```
                        ┌─────────────────────┐
                        │   APPLICATION       │
                        │   STARTS/LOADS      │
                        └──────────┬──────────┘
                                   │
                    ┌──────────────┼──────────────┐
                    │ CLEANUP      │              │
                    │ EXPIRED      │              │
                    │ HOLDS        │              │
                    └──────────────┼──────────────┘
                                   │
              ┌────────────────────┼────────────────────┐
              │                    │                    │
              ▼                    ▼                    ▼
    ┌──────────────────┐ ┌──────────────────┐ ┌──────────────────┐
    │   AVAILABLE      │ │  PROCESSING      │ │     BOOKED       │
    │   (Green)        │ │  (Gold - ⏱)      │ │    (Red - X)      │
    │                  │ │                  │ │                  │
    │ • Clickable      │ │ • NOT clickable  │ │ • NOT clickable  │
    │ • No hold        │ │ • 20-min hold    │ │ • Permanent lock │
    │ • Can select     │ │ • Others see it  │ │ • Cannot change  │
    │                  │ │ • Auto-expires   │ │ • Only admin can │
    │                  │ │                  │ │   cancel         │
    └─────────┬────────┘ └──────────┬───────┘ └──────────────────┘
              │                     │
              │ User selects        │ User confirms      Permanent
              │ seat               │ payment/pays       (end state)
              └────────────────────►│
                                    │
                        Expires after 20 min
                        without confirmation
                                    │
                                    ▼
                        Returns to AVAILABLE
```

---

## ⏱️ Hold Duration Timeline

```
TIME 0:00
│
├─► User selects seats 5, 6
│   └─ BookingSeat.Status = "Processing"
│   └─ BookingSeat.ProcessingStartTime = NOW
│   └─ BookingSeat.HoldExpiryTime = NOW + 20 min
│   └─ Display countdown: "20:00"
│
├─► Other users refresh page
│   └─ See seats as "⏱" (being held)
│   └─ Cannot select them
│
├─► User enters boarding places
│   └─ No action needed, timer continues
│
├─► User clicks "Proceed to Payment"
│   └─ API: ValidateSeats
│   └─ Check: Still within 20-min window ✓
│   └─ Show payment form
│
├─► User enters card details
│   └─ Timer still running
│   └─ Display: "Hold expires: 18:45"
│
├─► User clicks "Pay"
│   └─ API: ProcessPayment
│   │
│   ├─ Cleanup expired holds (if any)
│   ├─ Validate seats still on hold
│   ├─ Create Booking record
│   ├─ UPDATE BookingSeat.Status = "Booked"
│   ├─ UPDATE BookingSeat.BookedDate = NOW
│   ├─ CREATE PaymentInfo record
│   └─ Return success
│
├─► Booking confirmed
│   └─ Seats now Status = "Booked" (PERMANENT)
│   └─ Other users see as "X" (locked forever)
│
└─► DONE
    (Original 20-min hold would expire at 0:20,
     but seats already BOOKED so no cleanup needed)


ALTERNATIVE PATH: User doesn't confirm

TIME 0:00 ─► Select seats (status = PROCESSING)
TIME 0:20 ─► Hold expiry reached
             Next page load/API call triggers cleanup
             │
             ├─ Query: Find PROCESSING seats
             ├─ Where: HoldExpiryTime < NOW
             ├─ Action: DELETE BookingSeat records
             └─ Result: Seats revert to AVAILABLE

TIME 0:21 ─► Other users can now select them ✓
```

---

## 👥 Multi-User Scenario

```
USER A                          SHARED STATE              USER B
─────────────────────────────────────────────────────────────────

14:30:00
├─ Select seats 5, 6
│  └─ HoldSeats()
│
└─ BookingSeat(5):
   ├─ Status = PROCESSING
   ├─ HoldExpiryTime = 14:50:00
   └─ Display: "Hold expires: 19:59"

                          (Database)
                    BookingSeats:
                    ├─ SeatNo=5, Status=PROCESSING
                    │            HoldExpiry=14:50:00
                    └─ SeatNo=6, Status=PROCESSING
                                HoldExpiry=14:50:00


14:31:00
                                          ← Page loads
                                          ├─ SelectSeats()
                                          │
                                          └─ Query results:
                                             ├─ BookedSeats=[]
                                             └─ ProcessingSeats=[5,6]

                                          Display:
                                          ├─ Seat 5 = "⏱"
                                          ├─ Seat 6 = "⏱"
                                          └─ Message: "Being held by
                                             another user (20 min)"


14:32:00
├─ Click "Proceed to Payment"
│  └─ ValidateSeats([5,6])
│     ├─ Check booked: None ✓
│     ├─ Check processing: [5,6] still valid ✓
│     └─ Return: success
│
└─ Show payment form


14:33:00
                                          ← Select seats 8, 9
                                          ├─ HoldSeats()
                                          │
                                          └─ BookingSeat(8):
                                             ├─ Status = PROCESSING
                                             ├─ HoldExpiryTime = 14:53:00
                                             └─ Display: "Hold: 19:59"


14:35:00
├─ Enter card & click "Pay"
│  └─ ProcessPayment([5,6])
│     ├─ Cleanup expired holds: None
│     ├─ Validate [5,6]: Status still PROCESSING ✓
│     ├─ Create Booking(Id=5)
│     ├─ UPDATE [5,6]: Status=BOOKED, BookingId=5
│     ├─ CREATE PaymentInfo(5)
│     └─ Return bookingId=5
│
└─ BOOKING CREATED ✓
   Seats 5, 6 now LOCKED


14:36:00                                   ← Enter card & click "Pay"
└─ Success page                               └─ ProcessPayment([8,9])
   Booking Ref: BK-5-251119                    ├─ Cleanup expired: None
                                               ├─ Validate [8,9]: Still PROCESSING ✓
                                               ├─ Create Booking(Id=6)
                                               ├─ UPDATE [8,9]: Status=BOOKED, BookingId=6
                                               ├─ CREATE PaymentInfo(6)
                                               └─ Return bookingId=6

14:37:00                                   └─ Success page
                                              Booking Ref: BK-6-251119

FINAL STATE:
BookingSeats:
├─ Seat 5: Status=BOOKED, BookingId=5
├─ Seat 6: Status=BOOKED, BookingId=5
├─ Seat 8: Status=BOOKED, BookingId=6
└─ Seat 9: Status=BOOKED, BookingId=6

RESULT: Both users successfully booked ✓
```

---

## 🎯 API Request/Response Flow

```
1. PAGE LOAD
   ├─ Browser: GET /Booking/SelectSeats/1
   │
   └─ Server:
      ├─ SELECT FROM BookingSeats
      │  WHERE ScheduleId=1 AND Status='Booked'
      │
      ├─ SELECT FROM BookingSeats
      │  WHERE ScheduleId=1 AND Status='Processing'
      │  AND HoldExpiryTime > NOW
      │
      ├─ DELETE FROM BookingSeats
      │  WHERE ScheduleId=1 AND Status='Processing'
      │  AND HoldExpiryTime < NOW
      │
      └─ Return View with:
         ├─ BookedSeats = [2, 5, 8]
         ├─ ProcessingSeats = [3, 7]
         └─ TotalSeats = 44


2. USER SELECTS SEAT
   ├─ Browser: POST /Booking/HoldSeats
   │  Body: {
   │    scheduleId: 1,
   │    selectedSeats: [5, 6, 7]
   │  }
   │
   └─ Server:
      ├─ INSERT INTO BookingSeats
      │  (SeatNumber=5, Status='Processing',
      │   ProcessingStartTime=NOW,
      │   HoldExpiryTime=NOW+20min)
      │
      ├─ INSERT INTO BookingSeats
      │  (SeatNumber=6, Status='Processing', ...)
      │
      ├─ INSERT INTO BookingSeats
      │  (SeatNumber=7, Status='Processing', ...)
      │
      └─ Return Response: {
           success: true,
           expiryTime: "2025-11-19T14:50:00Z"
         }


3. USER PROCEEDS TO PAYMENT
   ├─ Browser: POST /Booking/ValidateSeats
   │  Body: {
   │    scheduleId: 1,
   │    selectedSeats: [5, 6, 7]
   │  }
   │
   └─ Server:
      ├─ DELETE expired holds
      │  WHERE HoldExpiryTime < NOW
      │
      ├─ SELECT booked=[5,6,7] where Status='Booked'
      │  → Result: None (good!)
      │
      ├─ SELECT processing=[5,6,7] where
      │  Status='Processing' AND HoldExpiryTime>NOW
      │  → Result: [5,6,7] (good!)
      │
      └─ Return Response: {
           success: true,
           message: "Seats are available"
         }


4. USER CONFIRMS PAYMENT
   ├─ Browser: POST /Booking/ProcessPayment
   │  Body: {
   │    scheduleId: 1,
   │    selectedSeats: [5, 6, 7],
   │    totalFare: 300,
   │    cardNumber: "4111111111111111",
   │    ...
   │  }
   │
   └─ Server:
      ├─ DELETE expired holds (cleanup)
      │
      ├─ SELECT booked where SeatNumber IN [5,6,7]
      │  AND Status='Booked'
      │  → Result: None (good!)
      │
      ├─ INSERT INTO Bookings
      │  (UserId=1, ScheduleId=1, NumberOfSeats=3,
      │   TotalFare=300, Status='Confirmed', ...)
      │  → BookingId = 5
      │
      ├─ UPDATE BookingSeats
      │  SET Status='Booked', BookingId=5, BookedDate=NOW
      │  WHERE SeatNumber IN [5,6,7]
      │
      ├─ INSERT INTO PaymentInfos
      │  (BookingId=5, CardNumber='****1111',
      │   Amount=300, PaymentStatus='Completed', ...)
      │
      └─ Return Response: {
           success: true,
           bookingId: 5,
           message: "Booking confirmed successfully!"
         }
```

---

## 📊 Query Patterns

### Get Current Seat States

```sql
SELECT
    bs.SeatNumber,
    bs.Status,
    CASE
        WHEN bs.Status = 'Booked' THEN 'LOCKED'
        WHEN bs.Status = 'Processing'
             AND bs.HoldExpiryTime > GETDATE()
             THEN 'HELD (' + CAST(DATEDIFF(MINUTE, GETDATE(),
                        bs.HoldExpiryTime) AS VARCHAR) + ' min)'
        WHEN bs.Status = 'Processing'
             THEN 'EXPIRED'
        ELSE 'AVAILABLE'
    END as CurrentState,
    bs.HoldExpiryTime
FROM BookingSeats bs
WHERE bs.ScheduleId = 1
ORDER BY bs.SeatNumber
```

### Cleanup Expired Holds

```sql
DELETE FROM BookingSeats
WHERE ScheduleId = 1
  AND Status = 'Processing'
  AND HoldExpiryTime < GETDATE()
```

### Find Booked Seats

```sql
SELECT bs.SeatNumber, b.BookingId
FROM BookingSeats bs
INNER JOIN Bookings b ON bs.BookingId = b.BookingId
WHERE bs.ScheduleId = 1
  AND bs.Status = 'Booked'
ORDER BY bs.SeatNumber
```

---

## 🎨 UI State Mapping

```
Database Status → Display Visual    → User Sees
────────────────────────────────────────────────────

BOOKED          → Red Box (X)       → "This seat is booked"
                                    → Cannot click

PROCESSING      → Gold Box (⏱)      → "Being held by another user"
(valid)         → Pulsing animation → "20 min" message
                                    → Cannot click

PROCESSING      → Will be cleaned   → Not shown (cleanup on load)
(expired)       → Removed from DB

AVAILABLE       → Green Box (#)     → "Select seat"
                                    → Clickable
                                    → Changes to gold on click

DRIVER          → Orange Box (D)    → "Driver seat - Not available"
                                    → Cannot click
```

---

**Version**: 2.0  
**Last Updated**: November 19, 2025  
**Status**: ✅ PRODUCTION READY
