# GoCylone Deployment Guide

## ⚠️ Important Note

**Vercel does NOT support .NET applications.** Vercel is designed for Node.js, Python, Go, and other serverless languages.

For your ASP.NET Core application, you have better options:

---

## Option 1: **Railway.app** (RECOMMENDED - Easiest for .NET)

Railway supports .NET applications natively and is very beginner-friendly.

### Prerequisites

- GitHub account
- Railway account (free at railway.app)
- Your code pushed to GitHub

### Step-by-Step Deployment

#### 1. **Prepare Your Code**

First, ensure your project is ready:

```bash
cd "c:\Users\ccs\Desktop\projects for Job\GoCylone"

# Test locally
dotnet build
dotnet run
```

#### 2. **Create Database Connection Config**

Create `appsettings.Production.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "${DATABASE_URL}"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

#### 3. **Push Code to GitHub**

```bash
git init
git add .
git commit -m "Initial commit"
git branch -M main
git remote add origin https://github.com/YOUR_USERNAME/GoCylone.git
git push -u origin main
```

#### 4. **Deploy on Railway.app**

1. Go to [railway.app](https://railway.app)
2. Sign in with GitHub
3. Click "Create New Project"
4. Select "Deploy from GitHub repo"
5. Select your GoCylone repository
6. Railway will auto-detect it's a .NET project

#### 5. **Add Database Service**

1. In Railway dashboard, click "Add Service"
2. Select "PostgreSQL" (or MySQL)
3. Railway will create the database automatically
4. Copy the `DATABASE_URL` connection string

#### 6. **Configure Environment Variables**

In Railway dashboard, add these variables:

```
DATABASE_URL = (your connection string from step 5)
ASPNETCORE_ENVIRONMENT = Production
```

#### 7. **Deploy**

Railway auto-deploys on every push to main branch!

---

## Option 2: **Azure App Service** (Enterprise-grade)

Microsoft Azure is perfect for ASP.NET Core apps.

### Prerequisites

- Microsoft Azure account
- Visual Studio or VS Code
- Azure CLI

### Step-by-Step Deployment

#### 1. **Create Azure Account**

- Go to [azure.microsoft.com](https://azure.microsoft.com)
- Sign up (free tier includes $200 credits)

#### 2. **Create Resource Group**

```bash
# Login to Azure
az login

# Create resource group
az group create --name GoCylone-RG --location "East US"
```

#### 3. **Create SQL Database**

```bash
# Create SQL Server
az sql server create --name gocylone-server --resource-group GoCylone-RG --admin-user azureuser --admin-password YourPassword123!

# Create Database
az sql db create --resource-group GoCylone-RG --server gocylone-server --name GoCyloneDB --edition Standard
```

#### 4. **Create App Service Plan**

```bash
# Create App Service Plan
az appservice plan create --name GoCylone-Plan --resource-group GoCylone-RG --sku B1 --is-linux

# Create Web App
az webapp create --resource-group GoCylone-RG --plan GoCylone-Plan --name gocylone-app --runtime "DOTNET|9.0"
```

#### 5. **Configure Connection String**

```bash
az webapp config appsettings set --name gocylone-app --resource-group GoCylone-RG --settings "ConnectionStrings:DefaultConnection=Server=tcp:gocylone-server.database.windows.net,1433;Initial Catalog=GoCyloneDB;Persist Security Info=False;User ID=azureuser;Password=YourPassword123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
```

#### 6. **Deploy Your Code**

```bash
# Create deployment credentials
az webapp deployment user set --user-name YOUR_USERNAME --password YOUR_PASSWORD

# Deploy from local Git
cd "c:\Users\ccs\Desktop\projects for Job\GoCylone"
git init
git add .
git commit -m "Initial commit"
az webapp deployment source config-local-git --name gocylone-app --resource-group GoCylone-RG
git remote add azure https://YOUR_USERNAME@gocylone-app.scm.azurewebsites.net/gocylone-app.git
git push azure main
```

---

## Option 3: **Heroku** (Simple but paid)

Heroku is simple but no longer has a free tier.

### Prerequisites

- Heroku account
- Heroku CLI
- Git

### Quick Steps

```bash
# Login to Heroku
heroku login

# Create app
heroku create gocylone-app

# Add PostgreSQL
heroku addons:create heroku-postgresql:hobby-dev

# Get connection string
heroku config:get DATABASE_URL

# Add to Program.cs
# Deploy
git push heroku main
```

---

## Option 4: **DigitalOcean App Platform**

Similar to Railway but with more control.

### Prerequisites

- DigitalOcean account
- GitHub repository

### Steps

1. Go to [digitalocean.com/app-platform](https://digitalocean.com/app-platform)
2. Click "Create App"
3. Select your GitHub repository
4. Set environment to ".NET"
5. Add database service
6. Deploy

---

## Database Migration Considerations

### Important: Update Your Code for PostgreSQL

Since Azure SQL Server and Railway use different databases, update `Program.cs`:

```csharp
var dbProvider = Environment.GetEnvironmentVariable("DB_PROVIDER") ?? "SqlServer";

if (dbProvider == "PostgreSQL")
{
    builder.Services.AddDbContext<GoCyloneDbContext>(options =>
        options.UseNpgsql(connectionString));
}
else
{
    builder.Services.AddDbContext<GoCyloneDbContext>(options =>
        options.UseSqlServer(connectionString));
}
```

### Add NuGet Package for PostgreSQL

```bash
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

### Update .csproj

```xml
<ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="9.0.0" />
    <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.0.0" />
</ItemGroup>
```

---

## Security Best Practices

1. **Never commit secrets** to Git
2. **Use environment variables** for:
   - Connection strings
   - API keys
   - Database passwords
3. **Enable HTTPS** (all options above handle this)
4. **Configure CORS** if needed
5. **Use strong passwords** for databases

---

## Recommended Path

**For beginners: Railway.app**

- ✅ Easiest setup
- ✅ Auto-deploys from GitHub
- ✅ Built-in database support
- ✅ Free tier available
- ✅ No configuration needed

**For production: Azure**

- ✅ Enterprise-grade reliability
- ✅ Free tier with credits
- ✅ Excellent Microsoft support
- ✅ Auto-scaling available

---

## Need Help?

1. Railway: https://docs.railway.app/
2. Azure: https://learn.microsoft.com/en-us/azure/app-service/
3. .NET Deployment: https://learn.microsoft.com/en-us/dotnet/core/deploying/
