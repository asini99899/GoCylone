# 🚌 Seat Hold System - Quick Reference

## 📊 Seat States at a Glance

```
┌─────────────────────────────────────────────────────────────┐
│ AVAILABLE (Green)                                           │
│ ✓ Clickable                                                 │
│ ✓ No restrictions                                           │
│ Ready for booking                                           │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│ PROCESSING (Gold - ⏱)  [20 MINUTE HOLD]                    │
│ ✗ NOT Clickable                                             │
│ ⏱ Countdown: 19:45 (20 min timer)                          │
│ ⚠ Being held by another user                               │
│ ↳ Auto-expires after 20 min                                │
│ ↳ Becomes AVAILABLE if not confirmed                       │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│ BOOKED (Red - X)  [PERMANENT]                               │
│ ✗ NOT Clickable                                             │
│ 🔒 Locked forever                                           │
│ Confirmed booking - cannot be changed                       │
│ Only admin can cancel                                       │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│ DRIVER (Orange - D)                                         │
│ ✗ NOT Clickable                                             │
│ Reserved for bus driver                                     │
│ Never available for passengers                              │
└─────────────────────────────────────────────────────────────┘
```

---

## ⏱ 20-Minute Hold Timeline

```
Time 0:00 → User clicks seat 5, 6, 7
            ↓
            System creates PROCESSING records
            HoldExpiryTime = 0:20

Time 0:01  Display: "Hold expires: 19:59"

Time 0:10  Display: "Hold expires: 19:10"
           User fills locations

Time 0:15  User clicks "Proceed to Payment"
           Display: "Hold expires: 19:05"

Time 0:18  User enters card details
           Display: "Hold expires: 19:02"

Time 0:19  User clicks "Pay"
           ↓
           ✓ Validation: Seats still on hold ✅
           ✓ Payment processed ✅
           ✓ Status: PROCESSING → BOOKED ✅
           ✓ Booking confirmed ✅

Time 0:20  Original hold would expire
           But seats already BOOKED - no cleanup needed
           User sees success page ✅

---

Alternative: User Doesn't Confirm

Time 0:21  Background cleanup runs
           ↓
           Finds PROCESSING seats
           HoldExpiryTime < NOW
           ↓
           Deletes BookingSeat records
           ↓
           Seats revert to AVAILABLE
           ↓
           Next user can now select them ✅
```

---

## 🎯 User Scenarios

### Scenario 1: Fast Checkout (SUCCESS ✅)

```
0:00  Select seats 3, 4 (PROCESSING)
0:02  Enter locations
0:05  Click "Proceed"
0:07  Click "Pay"
0:08  ✓ Payment success → Seats BOOKED
```

### Scenario 2: Slow Checkout (SUCCESS ✅)

```
0:00  Select seats 5, 6 (PROCESSING)
0:03  Enter locations
0:05  Go make coffee ☕
0:15  Return, click "Proceed"
0:16  Enter card details
0:18  Click "Pay"
0:19  ✓ Payment success → Seats BOOKED
      (Within 20 min window)
```

### Scenario 3: Too Slow (FAILURE ❌)

```
0:00  Select seats 7, 8 (PROCESSING)
0:05  Enter locations
0:10  Go make lunch 🍽️
0:25  Return to browser
      ❌ Hold expired!
      "Hold expired! Seats released."
      ↓
      Must start over, select new seats
```

### Scenario 4: Other User Waiting (BLOCKED 🚫)

```
User A → 0:00  Select seats 9, 10 (PROCESSING)
             ↓
             Hold expires: 0:20

User B → 0:03  Loads same bus
             Sees seats 9, 10 as "⏱ Being held"
             ↓
             Selects different seats 11, 12
             ↓
             Gets own 0:20 hold

User A → 0:15  Completes payment
             Seats 9, 10 → BOOKED ✓

User B → 0:16  Enters payment
             Seats 11, 12 still on hold ✓

User B → 0:18  Completes payment
             Seats 11, 12 → BOOKED ✓

Result: Both users booked successfully ✅
```

---

## 🔄 System Flow Diagram

```
                    ┌─────────────────────┐
                    │  USER LOADS PAGE    │
                    └──────────┬──────────┘
                               │
                    ┌──────────▼──────────┐
                    │ CLEANUP EXPIRED     │
                    │ HOLDS (>20 min)     │
                    └──────────┬──────────┘
                               │
        ┌──────────────────────┼──────────────────────┐
        │                      │                      │
    ┌───▼──────┐   ┌──────┬───▼────┐    ┌──────┬───▼─────┐
    │ BOOKED   │   │      │        │    │      │         │
    │ (Red X)  │   │      │        │    │      │         │
    │ Locked   │   │   Processing  │    │  Available     │
    │ Forever  │   │   (Gold ⏱)    │    │  (Green)       │
    │          │   │   20 min hold │    │  Clickable     │
    └──────────┘   │                │    │                │
                   └────────────────┘    └────────────────┘
                        │ Expires              ▲
                        └──────────────────────┘


               BOOKING FLOW

    ┌─────────────┐
    │ USER CLICKS │
    │ SEAT 5, 6   │
    │ (Available) │
    └──────┬──────┘
           │
    ┌──────▼──────────────────┐
    │ HoldSeats API called     │
    │ Status: PROCESSING       │
    │ HoldExpiryTime = now+20m │
    │ Countdown starts         │
    └──────┬──────────────────┘
           │
    ┌──────▼──────────────────────────┐
    │ USER SELECTS LOCATIONS          │
    │ (Pickup, Drop)                  │
    │ Total Fare calculated           │
    └──────┬───────────────────────────┘
           │
    ┌──────▼──────────────────────────┐
    │ USER CLICKS                     │
    │ "PROCEED TO PAYMENT"            │
    └──────┬───────────────────────────┘
           │
    ┌──────▼──────────────────────────┐
    │ ValidateSeats checks:           │
    │ • Seats still on hold?          │
    │ • No new bookings?              │
    │ • Expired holds cleaned?        │
    └──────┬───────────────────────────┘
           │
    ┌──────▼──────────────────────────┐
    │ PAYMENT FORM LOADS              │
    │ Countdown still running         │
    └──────┬───────────────────────────┘
           │
    ┌──────▼──────────────────────────┐
    │ USER ENTERS CARD                │
    │ • Card holder name              │
    │ • Card number                   │
    │ • Expiry date                   │
    │ • CVV                           │
    └──────┬───────────────────────────┘
           │
    ┌──────▼──────────────────────────┐
    │ USER CLICKS "PAY"               │
    │ ├─ Validate card               │
    │ ├─ Check hold still valid       │
    │ ├─ Process payment              │
    │ ├─ Create Booking record        │
    │ ├─ Update Status: BOOKED        │
    │ ├─ Create PaymentInfo record    │
    │ └─ Redirect to Success          │
    └──────┬───────────────────────────┘
           │
    ┌──────▼──────────────────────────┐
    │ ✓ BOOKING CONFIRMED             │
    │ • Booking ID: 12                │
    │ • Reference: BK-12-251119       │
    │ • Seats 5, 6 → BOOKED (LOCKED) │
    │ • Other users see these as X    │
    └──────────────────────────────────┘
```

---

## 💾 Database Records

### When User Selects Seats

```
BookingSeats Table INSERT:
┌──────────────────────────────────┐
│ BookingSeatId: 101               │
│ BookingId: 0 (placeholder)       │
│ SeatNumber: 5                    │
│ ScheduleId: 1                    │
│ Status: "Processing"             │
│ ProcessingStartTime: 14:30:00    │
│ HoldExpiryTime: 14:50:00         │
│ BookedDate: NULL                 │
└──────────────────────────────────┘

BookingSeatId: 102
├─ SeatNumber: 6
├─ Status: "Processing"
├─ ProcessingStartTime: 14:30:00
├─ HoldExpiryTime: 14:50:00
└─ BookedDate: NULL
```

### When User Confirms Payment

```
BookingSeats UPDATE:
┌──────────────────────────────────┐
│ BookingSeatId: 101               │
│ BookingId: 5 (now set)           │
│ SeatNumber: 5                    │
│ ScheduleId: 1                    │
│ Status: "Booked" (changed)       │
│ ProcessingStartTime: 14:30:00    │
│ HoldExpiryTime: 14:50:00         │
│ BookedDate: 14:35:00 (set)       │
└──────────────────────────────────┘

Bookings INSERT:
┌──────────────────────────────────┐
│ BookingId: 5                     │
│ UserId: 1                        │
│ ScheduleId: 1                    │
│ NumberOfSeats: 2                 │
│ TotalFare: 200                   │
│ Status: "Confirmed"              │
│ PickupLocation: "Galewela"       │
│ DropLocation: "Matale"           │
│ BookingDate: 14:35:00            │
└──────────────────────────────────┘

PaymentInfos INSERT:
┌──────────────────────────────────┐
│ PaymentId: 5                     │
│ BookingId: 5                     │
│ CardHolderName: "John Doe"       │
│ CardNumber: "****1111"           │
│ Amount: 200                      │
│ PaymentStatus: "Completed"       │
│ TransactionId: UUID              │
│ PaymentDate: 14:35:00            │
└──────────────────────────────────┘
```

---

## 🛠 API Requests & Responses

### Request 1: HoldSeats (Auto-called when user selects)

```
POST /Booking/HoldSeats
Content-Type: application/json

{
  "scheduleId": 1,
  "selectedSeats": [5, 6]
}

RESPONSE (Success):
{
  "success": true,
  "message": "Seats held for 20 minutes",
  "expiryTime": "2025-11-19T14:50:00Z"
}

RESPONSE (Conflict):
{
  "success": false,
  "message": "Seats [5] are already booked"
}
```

### Request 2: ValidateSeats (Before payment)

```
POST /Booking/ValidateSeats
Content-Type: application/json

{
  "scheduleId": 1,
  "selectedSeats": [5, 6]
}

RESPONSE (Success):
{
  "success": true,
  "message": "Seats are available"
}

RESPONSE (Failure):
{
  "success": false,
  "message": "Seats [3] are being held by another user (expires in 5 minutes)."
}
```

### Request 3: ProcessPayment

```
POST /Booking/ProcessPayment
Content-Type: application/json

{
  "scheduleId": 1,
  "selectedSeats": [5, 6],
  "pickupLocation": "Galewela",
  "dropLocation": "Matale",
  "totalFare": 200.00,
  "cardHolderName": "John Doe",
  "cardNumber": "4111111111111111",
  "expiryDate": "12/25",
  "cvv": "123"
}

RESPONSE (Success):
{
  "success": true,
  "bookingId": 5,
  "message": "Booking confirmed successfully!"
}

RESPONSE (Hold expired):
{
  "success": false,
  "message": "Your hold has expired. Please select seats again."
}
```

---

## 🧪 Testing Quick Check

```
TEST 1: Seat Selection Creates Hold
□ Select seat 5
□ Verify countdown timer shows
□ Wait 1 second
□ Verify countdown decremented
✓ PASS: Hold created, timer working

TEST 2: Processing Seats Block Others
□ User A: Select seats 5, 6
□ User B: Refresh page
□ Verify seats 5, 6 show as "⏱"
□ Verify User B cannot click them
✓ PASS: Other users see holds

TEST 3: Payment Within 20 Min
□ Select seats
□ Proceed to payment (<10 min elapsed)
□ Enter card details
□ Click Pay
□ Verify booking succeeds
✓ PASS: Payment works within window

TEST 4: Expired Hold Cleanup
□ Select seats
□ Wait 21 minutes
□ Refresh page
□ Verify seats now AVAILABLE
✓ PASS: Automatic cleanup works

TEST 5: Booked Seats Locked
□ Complete a booking
□ Refresh page
□ Verify those seats show as "X"
□ Verify cannot click them
✓ PASS: Booked seats permanently locked
```

---

## 🎨 Visual Seat Layout Example

```
For Bus with SeatStructure: "2*2", TotalSeats: 44

          LEFT     RIGHT
        ┌──────────────┐
    Row A │ D    1  ║  2  │
        │ 3    4  ║  5  │
        ├──────────────┤
    Row B │ 6    7  ║  8  │
        │ 9   10  ║ 11  │
        ├──────────────┤
    Row C │12   13  ║ 14  │
        │15   16  ║ 17  │
        ├──────────────┤
    ... (22 rows total A-V)

Color Legend:
• D (Orange)  = Driver - not selectable
• 1-44 (Green) = Available - clickable
• ⏱ (Gold)    = Processing - not clickable
• X (Red)     = Booked - not clickable
```

---

## 📋 Checklist for Deployment

- [ ] BookingSeat model updated with Status, ProcessingStartTime, HoldExpiryTime
- [ ] Database migration created and applied
- [ ] HoldSeats endpoint working
- [ ] ReleaseHold endpoint working
- [ ] ValidateSeats distinguishes Processing vs Booked
- [ ] SelectSeats shows countdown timer
- [ ] ProcessingSeats displayed as "⏱" (gold)
- [ ] ProcessPayment converts Processing → Booked
- [ ] Expired holds cleaned up on page load
- [ ] Expired holds cleaned up on payment submission
- [ ] Test with 2+ simultaneous users ✓
- [ ] Test with various SeatStructures (2*2, 2*3) ✓
- [ ] Test hold expiry cleanup ✓
- [ ] Test permanent booking lock ✓

---

**Version**: 2.0  
**Last Updated**: November 19, 2025  
**Status**: ✅ READY TO USE
