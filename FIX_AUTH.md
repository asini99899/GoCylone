# Fix Authentication Issue

You're seeing a permission error because Git is using cached credentials from a different GitHub account.

## Solution: Use GitHub Personal Access Token

Follow these steps:

### Step 1: Generate Personal Access Token on GitHub

1. Go to: https://github.com/settings/tokens
2. Click **"Generate new token"** → **"Generate new token (classic)"**
3. Configure it:
   - **Token name**: `GoCylone Push`
   - **Expiration**: Select "90 days" (or 365 days)
   - **Scopes**: Check the box for `repo` (gives full control of repositories)
4. Click **"Generate token"**
5. **COPY the token** (it looks like `ghp_xxxxxxxxxxxx...`)
   - ⚠️ You won't see it again, so copy it now!

### Step 2: Update Remote URL

Run this command in PowerShell:
```powershell
cd "c:\Users\ccs\Desktop\projects for Job\GoCylone"
git remote set-url origin https://asini99899:YOUR_TOKEN@github.com/asini99899/GoCylone.git
```

Replace `YOUR_TOKEN` with the token you copied from GitHub.

### Step 3: Push Code

```powershell
git push -u origin main
```

---

## Example (DO NOT USE - just for reference):

```
git remote set-url origin https://asini99899:ghp_1234567890abcdefghijklmnopqrstuvwxyz@github.com/asini99899/GoCylone.git
```

---

## Alternative: Clear & Re-authenticate

If you prefer to not put token in URL, clear credentials:

```powershell
# Clear stored credentials
cmdkey /delete:git:https://github.com

# Then push (you'll be prompted to authenticate via browser)
git push -u origin main
```

---

## Quick Steps Summary:

1. Go to https://github.com/settings/tokens/new
2. Create token with `repo` scope
3. Copy token
4. Run the commands I provided above
5. Tell me when done!
