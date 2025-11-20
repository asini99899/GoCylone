# 🎫 Booking System - Quick Start Guide

## ✅ What's New

Complete booking system implemented with:

- 🪑 Interactive seat selection grid
- 📍 Boarding and drop location selection
- 💳 Card payment processing
- ✓ Booking confirmation

---

## 🚀 How to Test

### 1. Ensure You Have Test Data

Run this SQL (if you haven't already):

```sql
-- Routes
INSERT INTO Routes VALUES ('Galewela', 'Matale', 45.50, '1h 15m', GETDATE());

-- Buses
INSERT INTO Buses VALUES ('NP-ABC-001', 44, '2*2', 'COND001', 'AC', GETDATE());

-- Schedules (TODAY)
DECLARE @Today DATE = CAST(GETDATE() AS DATE);
INSERT INTO Schedules VALUES (1, 1, @Today, '08:30:00', GETDATE());
```

### 2. Test the Complete Flow

**Step 1: Home Page**

- Go to http://localhost:5020
- Search: From="Galewela", To="Matale", Date=Today
- Click "Search Buses"

**Step 2: Book a Bus**

- Click "Book Now" on the bus
- Redirects to `/Booking/SelectSeats/1`

**Step 3: Select Seats**

- Click on available seats (green)
- You can select multiple seats
- See price update in real-time
- Enter pickup location: "Galewela"
- Enter drop location: "Matale"
- Click "Proceed to Payment"

**Step 4: Enter Payment Details**

```
Cardholder: Test User
Card Number: 4111 1111 1111 1111 (test number)
Expiry: 12/25
CVV: 123
```

- Click "Pay Rs. XXX"

**Step 5: Success Confirmation**

- See booking reference number
- View all booking details
- Option to print ticket
- Click "Back to Home" to search again

---

## 📊 Seat Grid Display

The system automatically generates seats based on bus configuration:

For a 2x2 structure with 44 seats:

```
  A       B
┌────────┬────────┐
│ D   1  │ 2   3  │
│ 4   5  │ 6   7  │
│ 8   9  │ 10  11 │
└────────┴────────┘
```

Legend:

- 🟩 Green = Available seat (clickable)
- 🟩✓ Green checkmark = Selected seat
- 🟥 Red X = Already booked
- 🟧 Orange = Driver seat
- 🟦 Blue = Window seat

---

## 💳 Payment Details

### Test Card Information

- **Number**: 4111 1111 1111 1111
- **Expiry**: Any future date (MM/YY)
- **CVV**: Any 3 digits

### Features

- Auto-formats card number with spaces
- Auto-formats expiry date
- CVV input limited to 3 digits
- All fields required

---

## 🗄️ What Was Created

### Database Tables

1. **Bookings** - Main booking records
2. **BookingSeats** - Individual seat assignments
3. **PaymentInfos** - Payment details

### New Pages

1. `/Booking/SelectSeats/{scheduleId}` - Seat selection
2. `/Booking/ConfirmBooking/{scheduleId}` - Payment form
3. `/Booking/Success/{bookingId}` - Confirmation

### Controller

- **BookingController.cs** - All booking logic (140+ lines)

---

## 📝 Example URLs

Once you've booked:

- Seat selection: `http://localhost:5020/Booking/SelectSeats/1`
- Confirm booking: `http://localhost:5020/Booking/ConfirmBooking/1?seats=1,2,3&pickup=Galewela&drop=Matale`
- Success page: `http://localhost:5020/Booking/Success/1`

---

## ❗ Important Notes

### Current Limitations

- ⚠️ UserId is hard-coded to 1 (needs login integration)
- ⚠️ Payment is simulated (not actually processing)
- ⚠️ Card details not encrypted (for testing only)

### Before Production

1. Integrate with login system
2. Add real payment gateway (Stripe/PayPal)
3. Encrypt card data
4. Add email notifications
5. Implement refund policy

---

## 🐛 Troubleshooting

**"No buses found"**

- Check if you added test data with today's date
- Verify locations match exactly (case-sensitive search)

**Seats not showing**

- Check if bus has NumberOfSeats > 0
- Verify SeatStructure is set (2*2, 2*3, etc.)

**Can't click "Proceed to Payment"**

- You must select at least one seat
- Pickup and drop locations must be filled

**Error on payment page**

- Check browser console (F12)
- Verify all card fields are filled

---

## ✨ Key Features

✅ **Responsive Design**

- Works on desktop, tablet, phone
- Mobile-optimized seat grid

✅ **Real-time Validation**

- Live seat selection feedback
- Price updates instantly
- Form field validation

✅ **User-Friendly**

- Color-coded seats
- Clear error messages
- Intuitive workflow

✅ **Secure Design**

- Masked card display
- CVV protection
- Transaction IDs

---

## 📞 For More Info

See `BOOKING_SYSTEM_GUIDE.md` for:

- Complete architecture
- Database schema
- Security notes
- Enhancement ideas
- Full code documentation

---

**Status**: ✅ **READY TO TEST**

Your booking system is fully implemented and running on http://localhost:5020
