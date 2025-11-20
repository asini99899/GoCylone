# Steps to Push to GitHub

Follow these steps to push your GoCylone project to GitHub:

## 1. Create a New Repository on GitHub

1. Go to https://github.com/asini99899 and sign in
2. Click the **"+"** icon in the top-right corner
3. Select **"New repository"**
4. Fill in the details:
   - **Repository name**: `GoCylone` (or any name you prefer)
   - **Description**: `Bus Booking and Management System with ASP.NET Core`
   - **Public** or **Private** (your choice)
   - Do NOT check "Initialize with README" (we already have files)
5. Click **"Create repository"**

## 2. Add Remote and Push

After creating the repository, GitHub will show you commands. Run these commands in your terminal:

```bash
cd "c:\Users\ccs\Desktop\projects for Job\GoCylone"

# Add the remote repository (replace YOUR_REPO_NAME if different)
git remote add origin https://github.com/asini99899/GoCylone.git

# Verify the remote was added
git remote -v

# Rename branch to main (optional but recommended)
git branch -M main

# Push the code to GitHub
git push -u origin main
```

## 3. Authenticate with GitHub

When you run `git push`, you may be prompted to authenticate. You have two options:

### Option A: Personal Access Token (Recommended)
1. Go to https://github.com/settings/tokens
2. Click **"Generate new token"** → **"Generate new token (classic)"**
3. Configure:
   - **Token name**: `GoCylone Push`
   - **Expiration**: 90 days (or as you prefer)
   - **Scopes**: Check `repo` (full control of private repositories)
4. Click **"Generate token"**
5. **Copy the token** (you won't see it again)
6. When Git asks for password, paste the token

### Option B: SSH Key
1. Generate SSH key: `ssh-keygen -t ed25519 -C "madubahashiniashini@gmail.com"`
2. Press Enter for all prompts to use default location
3. Add key to GitHub: https://github.com/settings/ssh/new
4. Change remote to SSH: 
   ```bash
   git remote set-url origin git@github.com:asini99899/GoCylone.git
   ```

## 4. Complete Commands (Copy & Paste)

```powershell
cd "c:\Users\ccs\Desktop\projects for Job\GoCylone"
git remote add origin https://github.com/asini99899/GoCylone.git
git branch -M main
git push -u origin main
```

## Verification

After pushing, visit https://github.com/asini99899/GoCylone to verify your code is there!

## Future Pushes

For future updates, simply run:
```bash
git add .
git commit -m "Your message here"
git push
```

---

**Need Help?**
- GitHub Docs: https://docs.github.com/
- Git Help: https://git-scm.com/doc
