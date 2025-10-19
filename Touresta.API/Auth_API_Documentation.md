# Touresta APIs Documentation

##  Base URLs
- **Development:** `http://localhost:5107`
- **Production:** `https://touresta.com`

##  Mobile App APIs

### Authentication
- `POST /api/Auth/check-email` - التحقق من الإيميل
- `POST /api/Auth/verify-password` - التحقق من كلمة المرور  
- `POST /api/Auth/register` - تسجيل مستخدم جديد
- `POST /api/Auth/google-login` - تسجيل الدخول بـ Google

##  Web Dashboard APIs

### Admin Authentication  
- `POST /api/AdminAuth/check-email` - التحقق من إيميل الادمن
- `POST /api/AdminAuth/verify-password` - التحقق من كلمة مرور الادمن
- `POST /api/AdminAuth/google-login` - تسجيل دخول الادمن بـ Google

## How to Test
1. شغل المشروع: `dotnet run`
2. افتحي: `http://localhost:5107/swagger`
3. جربي الـ APIs من هناك