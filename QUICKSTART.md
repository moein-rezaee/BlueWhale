# 🐳 BlueWhale - Docker Registry Management Panel
## Quick Start Guide (راهنمای شروع سریع)

---

## 📋 پیش نیازها (Prerequisites)

- ✅ Docker (نسخه 20.10+)
- ✅ Docker Compose (نسخه 2.0+)
- ✅ 4GB RAM حداقل
- ✅ 2GB فضای دیسک

---

## 🚀 شروع سریع (5 دقیقه)

### گزینه 1: اجرای کل پروژه از Root

```bash
# 1. وارد دایرکتوری اصلی شوید
cd /workspaces/BlueWhale

# 2. تمام سرویس‌ها رو بالا بیارید
docker-compose up --build

# 3. صبر کنید تا تمام سرویس‌ها شروع شوند (1-2 دقیقه)
```

**سرویس‌ها:**
- 🌐 Frontend: http://localhost:3000
- 🔧 Backend API: http://localhost:5260
- 📚 API Swagger: http://localhost:5260/swagger
- 📦 Docker Registry: http://localhost:5000

---

## 🔓 ورود (Login)

**URL**: http://localhost:3000

**نام کاربری (Username)**: `admin`  
**رمز عبور (Password)**: `admin123`

---

## 📊 سرویس‌ها (Services)

### 1️⃣ Frontend (UI) - Nuxt 4
```
پورت: 3000
ساختار:
- صفحات (Pages): Dashboard, Repositories, Activity, Settings, Security
- Composables: useRegistryApi
- Layouts: دیفالت برای ناویگیشن
- Assets: استایل‌های Tailwind CSS

از کجا بالا می‌آید: BlueWhale.UI/Dockerfile
```

### 2️⃣ Backend API - .NET 9.0
```
پورت: 5260
ساختار:
- Controllers: Auth, Users, Activities, Settings, Repositories, Tags, Statistics, Health
- Services: DockerRegistryService، DatabaseContext
- Database: SQLite (data/api/registry.db)

از کجا بالا می‌آید: BlueWhale.Registry/BlueWhale.Registry.Api/Dockerfile
```

### 3️⃣ Docker Registry - Official Registry V2
```
پورت: 5000
نقش: ذخیره تصاویر Docker
حجم‌ها: data/registry

از کجا بالا می‌آید: registry:2.8.3 (رسمی)
```

---

## 🗂️ ساختار پروژه

```
/workspaces/BlueWhale/
├── docker-compose.yml          ⭐ ROOT - تمام سرویس‌ها اینجا
├── BlueWhale.Registry/
│   ├── BlueWhale.Registry.Api/
│   │   ├── Dockerfile          (Build شامل dotnet restore, build, publish)
│   │   ├── Program.cs          (JWT، Database، Logging)
│   │   ├── appsettings.json    (Configuration)
│   │   └── Controllers/        (5 controllers)
│   ├── BlueWhale.Registry.Domain/
│   ├── BlueWhale.Registry.Infrastructure/
│   └── BlueWhale.Registry.Application/
├── BlueWhale.UI/
│   ├── Dockerfile              (Nuxt Build)
│   ├── nuxt.config.ts
│   ├── app/
│   │   ├── pages/              (Login, Dashboard, Repositories, Activity, etc)
│   │   ├── layouts/
│   │   └── composables/
│   ├── assets/
│   └── package.json
└── data/                        (Persistence)
    ├── api/                     (SQLite database)
    └── registry/                (Docker images)
```

---

## 🛠️ دستورات مفید

### بالا بیاورن تمام سرویس‌ها
```bash
cd /workspaces/BlueWhale
docker-compose up -d
```

### پایین آوردن تمام سرویس‌ها
```bash
docker-compose down
```

### دیدن لاگ‌ها
```bash
# تمام سرویس‌ها
docker-compose logs -f

# فقط Backend
docker-compose logs -f registry-api

# فقط Frontend
docker-compose logs -f ui

# فقط Registry
docker-compose logs -f registry
```

### ریست‌کردن کامل (حذف تمام داده‌ها)
```bash
docker-compose down -v
rm -rf data/
docker-compose up -d
```

### بررسی وضعیت سرویس‌ها
```bash
docker-compose ps
```

### وارد شدن به سرویس
```bash
# Backend
docker exec -it bluewhale-registry-api bash

# Frontend
docker exec -it bluewhale-ui sh

# Registry
docker exec -it bluewhale-registry-core sh
```

---

## 🧪 تست API Endpoints

### لاگین
```bash
curl -X POST http://localhost:5260/v1/api/Auth/Login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "admin123"
  }'
```

**جواب:**
```json
{
  "accessToken": "eyJhbGc...",
  "refreshToken": "550e8400...",
  "expiresIn": 3600,
  "user": {
    "id": "...",
    "username": "admin",
    "email": "admin@bluewhale.local",
    "role": "Admin"
  }
}
```

### دریافت لیست کاربران (نیاز به Token)
```bash
TOKEN="eyJhbGc..." # از login بالا

curl -X GET http://localhost:5260/v1/api/Users \
  -H "Authorization: Bearer $TOKEN"
```

### دریافت فعالیت‌ها
```bash
curl -X GET http://localhost:5260/v1/api/Activities \
  -H "Authorization: Bearer $TOKEN"
```

---

## 🔐 احراز هویت (Authentication)

**سیستم:** JWT (JSON Web Tokens)

**نحوه کار:**
1. لاگین با username و password
2. دریافت access token و refresh token
3. استفاده از access token در Authorization header
4. وقتی token expire شد، refresh token استفاده کنید

**هدر مثال:**
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## 📚 API مستندات

### Swagger UI
```
http://localhost:5260/swagger
```

تمام endpoints اینجا قابل مشاهده و تست هستند!

---

## 🐛 مشکل‌شناسی (Troubleshooting)

### ❌ خطای "Cannot connect to Docker daemon"
```bash
# Docker رو شروع کنید
sudo systemctl start docker

# یا (Mac)
open /Applications/Docker.app
```

### ❌ Port 5000/5260/3000 در حال استفاده
```bash
# ببینید کی استفاده می‌کنه
lsof -i :5000
lsof -i :5260
lsof -i :3000

# یا سرویس قدیمی رو حذف کنید
docker-compose down
```

### ❌ Database خراب است
```bash
# ریست‌کردن database
rm -rf data/api/registry.db
docker-compose restart registry-api
```

### ❌ Frontend نمی‌بندد
```bash
# صفحه رو Hard Refresh کنید
Ctrl+Shift+R (یا Cmd+Shift+R on Mac)

# یا Cache رو پاک کنید
docker-compose down
rm -rf BlueWhale.UI/.nuxt
docker-compose up -d
```

---

## 🔄 Workflow نرمال

### هروز استفاده:
```bash
# صبح
cd /workspaces/BlueWhale
docker-compose up -d

# تمام روز کار کنید
# http://localhost:3000

# شام
docker-compose down
```

### Development:
```bash
# Backend تغییر کرد
docker-compose up --build registry-api

# Frontend تغییر کرد
docker-compose up --build ui

# تمام چیز تغییر کرد
docker-compose up --build
```

---

## 📱 از بیرون دسترسی

اگر می‌خواهید از دیگر ماشین بتوانید دسترسی داشته باشید:

```bash
# سرویس‌ها رو روی 0.0.0.0 دنبال کنند
# (درست شده در docker-compose.yml)

# از ماشین دیگر:
http://YOUR_IP:3000      # Frontend
http://YOUR_IP:5260      # API
http://YOUR_IP:5000      # Registry
```

---

## 🎯 پایگاه داده (Database)

**نوع:** SQLite  
**محل:** `data/api/registry.db`  
**Backup:**
```bash
cp -r data/api/registry.db data/api/registry.db.backup
```

**Restore:**
```bash
cp data/api/registry.db.backup data/api/registry.db
docker-compose restart registry-api
```

---

## 🔒 امنیت

### مقادیر اولیه (Change in Production!)
```
Username: admin
Password: admin123
JWT Secret: bluewhale-docker-registry-secret-key-2025
```

### تغییر رمز:
1. وارد شوید: http://localhost:3000 (admin/admin123)
2. Security صفحه → تغییر رمز

### تغییر JWT Secret:
```bash
# docker-compose.yml رو ویرایش کنید
REGISTRY_API environment:
  JWT_SECRET: "your-secure-random-secret-here"

# سپس
docker-compose down
docker-compose up -d
```

---

## 📈 Performance

- **First Load:** ~30 seconds
- **Login:** ~500ms
- **List Repositories:** ~100ms
- **API Responses:** <100ms (usually)

---

## 🆘 کمک و پشتیبانی

### Logs بگیرید:
```bash
docker-compose logs > logs.txt
```

### اطلاعات سیستم:
```bash
docker-compose config > config.yml
docker ps -a
docker images
df -h /workspaces/BlueWhale/data/
```

---

## ✨ خلاصه

```
┌─────────────────────────────────────────┐
│     🐳 BlueWhale Docker Registry        │
├─────────────────────────────────────────┤
│  docker-compose up                      │
│           ↓                             │
│  ┌─────────┬──────────┬────────────┐   │
│  │ Frontend│ Backend  │ Registry   │   │
│  │ :3000   │ :5260    │ :5000      │   │
│  │ Nuxt    │ .NET 9   │ Official   │   │
│  └─────────┴──────────┴────────────┘   │
│           ↓                             │
│  🎉 Ready to use!                       │
└─────────────────────────────────────────┘
```

**حالا شماید آماده‌اید!**

---

**سوالات؟ مشکلات؟**  
Logs رو بگیرید و بگویید چه می‌بینید! 🔍
