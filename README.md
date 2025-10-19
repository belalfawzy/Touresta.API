# 🏝️ Touresta APIs Documentation

## 📍 Base URLs
- **Development:** `http://localhost:5107`
- **Production:** `http://tourestaapi.runasp.net/swagger/index.html`

## 📱 Mobile App APIs

### 🔐 Authentication
## login as a Tourest
- `POST /api/Auth/check-email` - التحقق من الإيميل
- `POST /api/Auth/verify-password` - التحقق من كلمة المرور  
- `POST /api/Auth/register` - تسجيل مستخدم جديد
- `POST /api/Auth/google-login` - تسجيل الدخول بـ Google
## login as a Helper 
- Not implemented yet
## 💻 Web Dashboard APIs

### 🔐 Admin Authentication  
- `POST /api/AdminAuth/check-email` - التحقق من إيميل الادمن
- `POST /api/AdminAuth/verify-password` - التحقق من كلمة مرور الادمن
- `POST /api/AdminAuth/google-login` - تسجيل دخول الادمن بـ Google

## 🚀 How to Test
1. `dotnet run`
2. `http://localhost:5107/swagger`
3. Start Testing
