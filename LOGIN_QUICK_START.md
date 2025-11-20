# GoCylone Login & Registration - Quick Start

## Application Status: ✅ RUNNING

**URL**: http://localhost:5020

---

## Initial Access

When you first access the application, you are redirected to the **Login Page**.

---

## Admin Login

**Username**: `admin`  
**Password**: `123456`

After login → **Home Page** with bus search functionality

---

## User Registration

1. Click **"Create Account"** button on Login page
2. Fill registration form:

   - Full Name
   - Phone Number
   - ID Number (NIC/Passport)
   - Username (unique)
   - Email (optional)
   - Password (min 4 characters)
   - Confirm Password

3. Click **"Create Account"**
4. See success message → Redirected to Login
5. Login with new username and password

---

## User Session

After successful login, session includes:

- User ID
- Username
- Full Name
- Role (admin/user)

**Stored for 8 hours** of inactivity

---

## Features After Login

✅ Search buses by route and date  
✅ Select seats and make bookings  
✅ Process payments  
✅ View user information in header  
✅ Logout functionality

---

## Test Credentials

**Admin Account**:

- Username: `admin`
- Password: `123456`

**Create test user** via registration to verify user login

---

## Logout

Click **"Logout"** button in header → Returns to Login Page

Session cleared immediately

---

## Session Timeout

- **Duration**: 8 hours of inactivity
- **Auto-logout**: Yes
- **Redirect**: Login page

---

## Database

✅ User table updated with authentication fields:

- UserName
- PhoneNumber
- IdNumber
- LastLogin
- IsActive

All migrations applied successfully.

---

**Status**: Production Ready ✅
