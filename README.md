# RAFIQ API Documentation

## Base URLs
- **Development:** `http://localhost:5107`
- **Production:** `http://tourestaapi.runasp.net`
- **Swagger:** `/swagger` (grouped into 4 docs: admin, user, helper, system)

## Tech Stack
- ASP.NET Core (.NET 9.0)
- MySQL 8.0.36 (Pomelo EF Core)
- JWT Bearer Authentication (24h tokens)
- Cloudinary (file uploads)
- SMTP Email (OTP verification)

---

## Mobile App APIs

### Authentication (`/api/auth`)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register a new user account |
| POST | `/api/auth/check-email` | Check if email exists |
| POST | `/api/auth/verify-password` | Login with email & password |
| POST | `/api/auth/verify-code` | Verify email verification code |
| POST | `/api/auth/resend-verification-code` | Resend verification code |
| POST | `/api/auth/google-login` | Google login |
| POST | `/api/auth/google-verify-code` | Verify Google login code |
| POST | `/api/auth/google-register` | Register via Google |
| POST | `/api/auth/verify-google-token` | Verify Google ID token |
| POST | `/api/auth/forgot-password` | Request password reset code |
| POST | `/api/auth/reset-password` | Reset password with code |
| PUT | `/api/auth/update-profile` | Update user profile (multipart/form-data) |

### Helper Onboarding (`/api/helper`) — Requires Auth

#### Registration & Profile
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/helper/register` | Create helper profile linked to user |
| PUT | `/api/helper/profile` | Update helper profile & documents (multipart/form-data) |

#### Status & Eligibility
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/helper/status` | Get onboarding progress & missing steps |
| GET | `/api/helper/eligibility` | Check all 5 activation conditions |

#### Drug Test
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/helper/drug-test` | Upload drug test (expires in 6 months) |

#### Car Management
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/helper/car` | Add or update car info with license docs |
| DELETE | `/api/helper/car` | Remove car info |

#### Certificates
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/helper/certificates` | Upload professional certificate |
| DELETE | `/api/helper/certificates/{id}` | Remove a certificate |

#### Languages
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/helper/languages` | List supported languages |
| GET | `/api/helper/languages/my` | Get helper's registered languages |
| POST | `/api/helper/languages/{code}/test` | Take AI-evaluated language test (max 3/month, 24h cooldown) |

---

## Web Dashboard APIs — Requires Admin Auth

### Admin Authentication (`/api/admin-auth`)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/adminauth/check-email` | Check admin email |
| POST | `/api/adminauth/verify-password` | Verify admin password & send OTP |
| POST | `/api/adminauth/google-login` | Admin Google login |
| POST | `/api/adminauth/verify-otp` | Verify OTP & get JWT token |

### Helper Management (`/api/admin/helpers`) — AdminOnly

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/admin/helpers/pending` | Get helpers awaiting review |
| GET | `/api/admin/helpers` | List helpers with search, filter & pagination |
| GET | `/api/admin/helpers/{id}` | Get full helper details for review |
| POST | `/api/admin/helpers/{id}/mark-under-review` | Mark as under review |
| POST | `/api/admin/helpers/{id}/approve` | Approve helper |
| POST | `/api/admin/helpers/{id}/reject` | Reject helper (with reason) |
| POST | `/api/admin/helpers/{id}/request-changes` | Request changes (with description) |
| POST | `/api/admin/helpers/{id}/ban` | Ban helper (with reason) |
| POST | `/api/admin/helpers/{id}/unban` | Remove ban |
| POST | `/api/admin/helpers/{id}/suspend` | Suspend helper (with reason) |
| POST | `/api/admin/helpers/{id}/activate` | Activate helper |

### Admin Management (`/api/admin/admins`) — SuperAdminOnly

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/admin/admins` | List all admin accounts |
| POST | `/api/admin/admins` | Create new admin |
| PATCH | `/api/admin/admins/{id}/role` | Update admin role |
| PATCH | `/api/admin/admins/{id}/deactivate` | Deactivate admin |
| PATCH | `/api/admin/admins/{id}/activate` | Activate admin |

### Dashboard (`/api/admin/dashboard`) — AdminOnly

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/admin/dashboard/stats` | Dashboard statistics |
| GET | `/api/admin/dashboard/recent-helpers` | 10 most recent registrations |
| GET | `/api/admin/dashboard/recent-actions` | 10 most recent admin actions |

### Reports (`/api/admin/reports`) — AdminOnly

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/admin/reports` | Get helper reports (filter by resolved status) |
| PATCH | `/api/admin/reports/{id}/resolve` | Resolve a report |

### Notes (`/api/admin/helpers/{helperId}/notes`) — AdminOnly

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/admin/helpers/{helperId}/notes` | Get internal notes for a helper |
| POST | `/api/admin/helpers/{helperId}/notes` | Add internal note |

### Audit Logs (`/api/admin/audit`) — AdminOnly

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/admin/audit` | Get paged audit logs with filters |
| GET | `/api/admin/audit/helpers/{helperId}` | Get audit history for a helper |

---

## System (`/api/home`)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/home` | API version & status |
| GET | `/api/home/status` | Server status with timestamp |
| GET | `/api/home/info` | App name, version, environment |
| GET | `/api/home/health` | Deep health check (DB connectivity) |

---

## Authorization Policies

| Policy | Requirement | Used By |
|--------|-------------|---------|
| Default (`[Authorize]`) | Valid JWT token | Helper endpoints |
| `AdminOnly` | JWT with `type="admin"` | Dashboard, helpers, reports, notes, audit |
| `SuperAdminOnly` | `type="admin"` + `role="SuperAdmin"` | Admin management |

## Data Formats
- **Entity IDs:** 32-char string (`Guid.NewGuid().ToString("N")`, no dashes)
- **Language codes:** ISO 639-1 (e.g., `en`, `fr`, `ar`)
- **Dates:** ISO 8601 UTC
- **File size limits:** Profile images (5MB), documents (10MB)

## How to Run
1. Configure `appsettings.json` (DB connection, JWT, SMTP, Cloudinary, Google)
2. `dotnet ef database update`
3. `dotnet run`
4. Open `http://localhost:5107/swagger`
