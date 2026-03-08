# Touresta Helper Mobile App — Complete Flow Document

> **Audience**: Flutter mobile developers
> **Backend**: ASP.NET Core Web API (Touresta.API)
> **Base URL**: `https://your-domain.com` (use `https://localhost:7000` for dev)
> **Auth**: JWT Bearer token — store after login, send as `Authorization: Bearer <token>` on all `/api/helper/*` endpoints

---

## Authentication Header

All `/api/helper/*` endpoints require:

```
Authorization: Bearer <jwt_token>
```

The token is obtained from the login flow (Step 1). Store it securely using `flutter_secure_storage`.

---

## STEP 1 — Helper Login / Authentication

The helper uses the **same authentication system** as regular users. The app determines the user type after login.

---

### Screen 1.1: Email Entry

| Field | Value |
|-------|-------|
| **Screen Name** | `EmailScreen` |
| **What helper sees** | App logo, "Enter your email" text field, "Continue" button, "Sign in with Google" button |
| **User action** | Types email and taps "Continue" |
| **API endpoint** | `POST /api/Auth/check-email` |

**Request body:**
```json
{
  "email": "helper@example.com"
}
```

**Response (200 — email exists):**
```json
{
  "message": "Email exists",
  "action": "go_to_password_page",
  "email": "helper@example.com"
}
```

**Response (404 — email not found):**
```json
{
  "message": "This email doesn't exist",
  "action": "stay_on_email_page"
}
```

**Response (400 — validation error):**
```json
{
  "success": false,
  "message": "Validation failed",
  "errors": ["Email: Invalid email format."],
  "statusCode": 400
}
```

**What the app does next:**
- `action == "go_to_password_page"` → Navigate to **PasswordScreen**
- `action == "stay_on_email_page"` → Show error "Email not found", offer "Register" button
- 400 error → Show validation message below the text field

---

### Screen 1.2: Password Entry

| Field | Value |
|-------|-------|
| **Screen Name** | `PasswordScreen` |
| **What helper sees** | "Welcome back" header, email displayed (read-only), password field, "Login" button, "Forgot password?" link |
| **User action** | Types password and taps "Login" |
| **API endpoint** | `POST /api/Auth/verify-password` |

**Request body:**
```json
{
  "email": "helper@example.com",
  "password": "SecurePass@123"
}
```

**Response (200 — success):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "message": "Login successful",
  "action": "go_to_dashboard",
  "user": {
    "id": 5,
    "userId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "email": "helper@example.com",
    "userName": "AhmedHelper",
    "phoneNumber": "+201234567890",
    "gender": "Male",
    "country": "Egypt",
    "birthDate": "1995-06-15T00:00:00",
    "profileImageUrl": "https://res.cloudinary.com/..."
  }
}
```

**Response (400 — invalid password):**
```json
{
  "message": "Invalid password",
  "action": "email_not_verified",
  "email": "helper@example.com"
}
```

**What the app does next:**
- **200 + action `go_to_dashboard`**:
  1. Store `token` in `flutter_secure_storage`
  2. Store `user` object in local state (Provider/Riverpod/Bloc)
  3. Navigate to **HelperDashboardScreen** (Step 2)
- **400 + message `Invalid password`** → Show error under password field
- **400 + message contains `verify`** → Navigate to OTP screen for email verification

---

### Screen 1.3: Google Login (Alternative)

| Field | Value |
|-------|-------|
| **Screen Name** | Part of `EmailScreen` |
| **What helper sees** | "Sign in with Google" button |
| **User action** | Taps Google sign-in button |
| **API endpoint** | `POST /api/Auth/google-login` |

**Flow:**
1. Use `google_sign_in` Flutter package to get the user's Google email
2. Call the API:

**Request body:**
```json
{
  "email": "helper@gmail.com"
}
```

**Response (200):**
```json
{
  "message": "Verification code sent to email"
}
```

**What the app does next:**
- Navigate to **OTPScreen** with the email
- If 404 → Navigate to registration screen

---

### Screen 1.4: OTP Verification

| Field | Value |
|-------|-------|
| **Screen Name** | `OTPScreen` |
| **What helper sees** | "Enter verification code" header, 6-digit OTP input fields, "Verify" button, "Resend code" link |
| **User action** | Enters 6-digit code and taps "Verify" |
| **API endpoint** | `POST /api/Auth/verify-code` |

**Request body:**
```json
{
  "email": "helper@example.com",
  "code": "482935"
}
```

**Response (200):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "message": "Verification successful"
}
```

**Response (400):**
```json
{
  "message": "Invalid code"
}
```

**What the app does next:**
- **200** → Store token, navigate to **HelperDashboardScreen**
- **400 `Invalid code`** → Shake the OTP fields, show error
- **400 `expired`** → Show "Code expired" message, highlight "Resend code"

**Resend OTP:**
```
POST /api/Auth/resend-verification-code
Body: { "email": "helper@example.com" }
```

---

## STEP 2 — Helper Dashboard (Onboarding Hub)

This is the **central screen** of the helper app. It always shows current onboarding progress.

---

### Screen 2.1: Helper Dashboard

| Field | Value |
|-------|-------|
| **Screen Name** | `HelperDashboardScreen` |
| **What helper sees** | Status banner, onboarding checklist, action buttons |
| **User action** | Taps on incomplete steps to navigate to them |
| **API endpoint** | `GET /api/Helper/status` |

**Request:**
```
GET /api/Helper/status
Authorization: Bearer <token>
```

**Response (200):**
```json
{
  "message": "Onboarding status retrieved",
  "data": {
    "helperId": "h-a1b2c3d4",
    "computedStatus": "Pending",
    "profileComplete": true,
    "nationalIdUploaded": true,
    "criminalRecordUploaded": false,
    "drugTestUploaded": false,
    "drugTestValid": false,
    "drugTestExpiry": null,
    "languagesVerified": 1,
    "hasCar": false,
    "isApproved": false,
    "isActive": false,
    "approvalStatus": "Pending",
    "rejectionReason": null,
    "missingSteps": [
      "Upload criminal record certificate (Fish & Tashbih directed to Touresta)",
      "Upload drug test document"
    ]
  }
}
```

**Response (404 — not registered as helper yet):**
```json
{
  "message": "No helper profile found. Please register first."
}
```

**What the app does next:**

If **404** → Show "Become a Helper" button → Navigate to **HelperRegisterScreen** (Step 3)

If **200** → Build the dashboard checklist:

```
┌──────────────────────────────────────────┐
│  Status: Pending Review                  │
│  ────────────────────────────────────── │
│                                          │
│  ✅  Profile Complete                    │
│  ✅  National ID Uploaded                │
│  ❌  Criminal Record → [Upload]          │
│  ❌  Drug Test → [Upload]                │
│  ✅  Languages (1 verified)              │
│  ➖  Car (optional) → [Add]              │
│  ➖  Certificates (optional) → [Add]     │
│                                          │
│  ⚠️ Missing Steps:                      │
│  • Upload criminal record certificate    │
│  • Upload drug test document             │
│                                          │
└──────────────────────────────────────────┘
```

**Status banner color logic:**

| `computedStatus` | Banner Color | Banner Text |
|-------------------|-------------|-------------|
| `"Pending"` | Yellow | "Your application is under review" |
| `"ChangesRequested"` | Orange | "Admin requested changes — see details below" |
| `"Rejected"` | Red | "Your application was rejected" |
| `"Active"` | Green | "You are active and can receive bookings" |
| `"Suspended"` | Red | "Your account has been suspended" |
| `"Unverified"` | Gray | "Please verify your email first" |

**When `rejectionReason` is not null:**
Show a card below the banner with the admin's reason text.

**Polling strategy:**
Call `GET /api/Helper/status` every time the dashboard screen is opened (in `initState` or `onResume`). Do NOT poll continuously.

---

## STEP 3 — Register as Helper

First-time helpers must register before accessing onboarding features.

---

### Screen 3.1: Become a Helper

| Field | Value |
|-------|-------|
| **Screen Name** | `HelperRegisterScreen` |
| **What helper sees** | "Become a Touresta Helper" title, form fields: Full Name, Gender dropdown, Birth Date picker, "Register" button |
| **User action** | Fills in all fields and taps "Register" |
| **API endpoint** | `POST /api/Helper/register` |

**Request body (JSON):**
```json
{
  "fullName": "Ahmed Mohamed Ali",
  "gender": 0,
  "birthDate": "1995-06-15"
}
```

> **Gender enum values:** `0 = Male`, `1 = Female`

**Response (200):**
```json
{
  "message": "Helper profile created successfully.",
  "action": "continue_onboarding",
  "data": {
    "helperId": "h-a1b2c3d4-e5f6-7890",
    "fullName": "Ahmed Mohamed Ali",
    "gender": "Male",
    "birthDate": "1995-06-15T00:00:00",
    "profileImageUrl": null,
    "nationalIdPhoto": null,
    "criminalRecordFile": null,
    "hasCar": false,
    "isActive": false,
    "isApproved": false,
    "approvalStatus": "Pending",
    "rejectionReason": null,
    "createdAt": "2026-03-08T14:30:00Z",
    "updatedAt": null
  }
}
```

**Response (400 — already registered):**
```json
{
  "message": "A helper profile already exists for this user."
}
```

**Response (400 — email not verified):**
```json
{
  "message": "User email must be verified before registering as a helper."
}
```

**What the app does next:**
- **200** → Show success animation, navigate to **HelperDashboardScreen**
- **400 already exists** → Navigate directly to dashboard
- **400 not verified** → Navigate back to email verification flow

**Important:** After registration, Arabic is **automatically added** as a verified native language. The helper does NOT need to test for Arabic.

---

## STEP 4 — Profile Completion

The helper uploads identity documents required for admin review.

---

### Screen 4.1: Helper Profile

| Field | Value |
|-------|-------|
| **Screen Name** | `HelperProfileScreen` |
| **What helper sees** | Form with: Full Name, Gender, Birth Date, National ID photo upload area, Criminal Record upload area, "Save" button |
| **User action** | Updates fields and/or uploads documents, taps "Save" |
| **API endpoint** | `PUT /api/Helper/profile` |

**Request: `multipart/form-data`**

```
Content-Type: multipart/form-data
Authorization: Bearer <token>

Fields:
  fullName: "Ahmed Mohamed Ali"              (optional, only if changed)
  gender: 0                                   (optional, only if changed)
  birthDate: "1995-06-15"                     (optional, only if changed)

Files:
  nationalIdPhoto: [image/jpeg file, max 5MB]    (optional)
  criminalRecordFile: [image/jpeg file, max 10MB] (optional)
```

**Flutter code example for multipart upload:**
```dart
var request = http.MultipartRequest('PUT', Uri.parse('$baseUrl/api/Helper/profile'));
request.headers['Authorization'] = 'Bearer $token';

// Text fields (only send changed fields)
request.fields['fullName'] = 'Ahmed Mohamed Ali';

// File uploads
if (nationalIdFile != null) {
  request.files.add(await http.MultipartFile.fromPath(
    'nationalIdPhoto',
    nationalIdFile.path,
  ));
}

if (criminalRecordFile != null) {
  request.files.add(await http.MultipartFile.fromPath(
    'criminalRecordFile',
    criminalRecordFile.path,
  ));
}

var response = await request.send();
```

**Response (200):**
```json
{
  "message": "Profile updated successfully.",
  "action": "profile_updated",
  "data": {
    "helperId": "h-a1b2c3d4",
    "fullName": "Ahmed Mohamed Ali",
    "gender": "Male",
    "birthDate": "1995-06-15T00:00:00",
    "nationalIdPhoto": "https://res.cloudinary.com/dyvwflsax/raw/upload/v.../helpers/5/national-id/abc123.jpg",
    "criminalRecordFile": "https://res.cloudinary.com/dyvwflsax/raw/upload/v.../helpers/5/criminal-record/def456.jpg",
    "hasCar": false,
    "isActive": false,
    "isApproved": false,
    "approvalStatus": "Pending",
    "rejectionReason": null,
    "createdAt": "2026-03-08T14:30:00Z",
    "updatedAt": "2026-03-08T15:00:00Z"
  }
}
```

**Response (400 — upload error):**
```json
{
  "message": "National ID upload failed: File type '.gif' is not allowed. Accepted: JPG, PNG, PDF."
}
```

**What the app does next:**
- **200** → Show success toast, navigate back to dashboard
- **400** → Show error message, keep the form open

**Accepted file types:** `.jpg`, `.jpeg`, `.png`, `.pdf`
**Max sizes:** National ID = 5MB, Criminal Record = 10MB

**UX Notes:**
- Show thumbnail preview after file selection
- Show upload progress indicator during submission
- If `approvalStatus` was `"ChangesRequested"`, re-submitting profile resets it to `"Pending"`

---

## STEP 5 — Drug Test Upload

A certified drug test is **required** for activation. Expiry is automatic (6 months).

---

### Screen 5.1: Drug Test

| Field | Value |
|-------|-------|
| **Screen Name** | `DrugTestScreen` |
| **What helper sees** | Current drug test status (if any), expiry date, "Upload New Drug Test" button |
| **User action** | Selects file and taps "Upload" |
| **API endpoint** | `POST /api/Helper/drug-test` |

**Request: `multipart/form-data`**
```
Content-Type: multipart/form-data
Authorization: Bearer <token>

Files:
  drugTestFile: [file, max 10MB, JPG/PNG/PDF]
```

**Response (200):**
```json
{
  "message": "Drug test uploaded successfully. Valid for 6 months.",
  "action": "drug_test_uploaded",
  "data": {
    "id": 12,
    "filePath": "https://res.cloudinary.com/.../helpers/5/drug-test/xyz789.pdf",
    "uploadedAt": "2026-03-08T15:30:00Z",
    "expiryDate": "2026-09-08T15:30:00Z",
    "isCurrent": true
  }
}
```

**What the app does next:**
- Show success with expiry date: "Valid until September 8, 2026"
- Navigate back to dashboard (the ❌ next to Drug Test becomes ✅)

**UX Notes:**
- If the helper already has a valid drug test, show its details and expiry countdown
- Show a warning banner when drug test expires within 30 days
- If the helper was deactivated due to drug test expiry, uploading a new one **auto-reactivates** them (if still admin-approved)

---

## STEP 6 — Car Information (Optional)

Car registration is optional but allows the helper to offer transport services.

---

### Screen 6.1: Add / Update Car

| Field | Value |
|-------|-------|
| **Screen Name** | `CarScreen` |
| **What helper sees** | Form: Brand, Model, Color dropdown, License Plate, Energy Type dropdown, Vehicle Type dropdown, Car License upload, Personal License upload |
| **User action** | Fills fields, selects files, taps "Save" |
| **API endpoint** | `POST /api/Helper/car` |

**Request: `multipart/form-data`**
```
Fields:
  brand: "Toyota"
  model: "Corolla"
  color: 0                    (enum: 0=White, 1=Black, 2=Red, ...)
  licensePlate: "ABC-1234"
  energyType: 0               (enum: 0=Gasoline, 1=Diesel, 2=Electric, 3=Hybrid)
  type: 0                     (enum: 0=Sedan, 1=SUV, 2=Hatchback, ...)

Files:
  carLicenseFile: [file, max 10MB]
  personalLicenseFile: [file, max 10MB]
```

**Enum Reference:**

| CarColor | Value |
|----------|-------|
| White | 0 |
| Black | 1 |
| Red | 2 |
| Blue | 3 |
| Silver | 4 |
| Gray | 5 |
| Brown | 6 |
| Green | 7 |

| CarEnergyType | Value |
|---------------|-------|
| Gasoline | 0 |
| Diesel | 1 |
| Electric | 2 |
| Hybrid | 3 |

| CarType | Value |
|---------|-------|
| Sedan | 0 |
| SUV | 1 |
| Hatchback | 2 |
| Van | 3 |

**Response (200):**
```json
{
  "message": "Car information saved successfully.",
  "action": "car_saved",
  "data": {
    "id": 3,
    "brand": "Toyota",
    "model": "Corolla",
    "color": "White",
    "licensePlate": "ABC-1234",
    "energyType": "Gasoline",
    "type": "Sedan",
    "carLicenseFile": "https://res.cloudinary.com/.../helpers/5/car/license.jpg",
    "personalLicenseFile": "https://res.cloudinary.com/.../helpers/5/car/personal.jpg"
  }
}
```

**Response (400 — duplicate plate):**
```json
{
  "message": "License plate already registered to another helper."
}
```

### Screen 6.2: Remove Car

| Field | Value |
|-------|-------|
| **User action** | Taps "Remove Car" button with confirmation dialog |
| **API endpoint** | `DELETE /api/Helper/car` |

**Response (200):**
```json
{
  "message": "Car removed successfully."
}
```

---

## STEP 7 — Certificates (Optional)

Certificates improve the helper's trust score and search ranking.

---

### Screen 7.1: Add Certificate

| Field | Value |
|-------|-------|
| **Screen Name** | `CertificateScreen` |
| **What helper sees** | List of uploaded certificates, "Add Certificate" button |
| **User action** | Taps "Add", fills Name, selects Type, uploads file |
| **API endpoint** | `POST /api/Helper/certificates` |

**Request: `multipart/form-data`**
```
Fields:
  name: "Tour Guide License - Cairo"
  type: 0                        (enum: 0=TourGuide, 1=Archaeology, 2=History, 3=Other)

Files:
  certificateFile: [file, max 10MB]
```

| CertificateType | Value |
|-----------------|-------|
| TourGuide | 0 |
| Archaeology | 1 |
| History | 2 |
| Other | 3 |

**Response (200):**
```json
{
  "message": "Certificate uploaded successfully.",
  "action": "certificate_uploaded",
  "data": {
    "id": 7,
    "name": "Tour Guide License - Cairo",
    "filePath": "https://res.cloudinary.com/.../helpers/5/certificates/cert.pdf",
    "type": "TourGuide",
    "isVerified": false,
    "uploadedAt": "2026-03-08T16:00:00Z"
  }
}
```

**Note:** `isVerified` starts as `false`. Admin sets it to `true` during approval.

### Screen 7.2: Remove Certificate

```
DELETE /api/Helper/certificates/{id}
Authorization: Bearer <token>
```

**Response (200):**
```json
{
  "message": "Certificate removed successfully."
}
```

**UX Notes:**
- Show certificates as cards with a thumbnail, name, type badge, and verified/unverified status
- Show a small "Verified ✓" badge on certificates after admin approval
- Helper can upload multiple certificates

---

## STEP 8 — Language Selection

Languages determine what tourists the helper can serve.

---

### Screen 8.1: Available Languages

| Field | Value |
|-------|-------|
| **Screen Name** | `LanguagesScreen` |
| **What helper sees** | Grid/list of 12 supported languages, each with "Added" or "Take Test" button |
| **User action** | Taps a language to take the proficiency test |
| **API endpoint** | `GET /api/Helper/languages` |

**Response (200):**
```json
{
  "message": "Available languages",
  "data": [
    { "code": "ar", "name": "Arabic",     "alreadyAdded": true  },
    { "code": "en", "name": "English",    "alreadyAdded": false },
    { "code": "fr", "name": "French",     "alreadyAdded": false },
    { "code": "de", "name": "German",     "alreadyAdded": false },
    { "code": "es", "name": "Spanish",    "alreadyAdded": false },
    { "code": "it", "name": "Italian",    "alreadyAdded": false },
    { "code": "pt", "name": "Portuguese", "alreadyAdded": false },
    { "code": "ru", "name": "Russian",    "alreadyAdded": false },
    { "code": "zh", "name": "Chinese",    "alreadyAdded": false },
    { "code": "ja", "name": "Japanese",   "alreadyAdded": false },
    { "code": "ko", "name": "Korean",     "alreadyAdded": false },
    { "code": "tr", "name": "Turkish",    "alreadyAdded": false }
  ]
}
```

**What the app does next:**
- Arabic (`alreadyAdded: true`) → Show as "Native ✓" (not tappable, no test needed)
- Other languages with `alreadyAdded: false` → Show "Take Test" button
- Other languages with `alreadyAdded: true` → Show level badge (from My Languages)

---

## STEP 9 — Language Test

AI-evaluated proficiency test with rate limiting.

---

### Screen 9.1: Language Test

| Field | Value |
|-------|-------|
| **Screen Name** | `LanguageTestScreen` |
| **What helper sees** | Language name header, series of questions, answer text fields, "Submit" button |
| **User action** | Answers all questions and taps "Submit" |
| **API endpoint** | `POST /api/Helper/languages/{code}/test` |

**Request body (JSON):**
```json
{
  "answers": [
    { "questionId": 1, "answer": "The Pyramids of Giza are located on the outskirts of Cairo." },
    { "questionId": 2, "answer": "The Nile River flows from south to north through Egypt." },
    { "questionId": 3, "answer": "Luxor Temple was built during the New Kingdom period." }
  ]
}
```

**Response (200 — passed):**
```json
{
  "message": "Congratulations! You passed the English test with a score of 82.",
  "data": {
    "languageCode": "en",
    "languageName": "English",
    "aiScore": 82,
    "level": "Advanced",
    "passed": true,
    "attemptsUsedThisMonth": 1,
    "nextRetryAvailableAt": null
  }
}
```

**Response (200 — failed):**
```json
{
  "message": "You did not pass the English test. Score: 45. You can retry after 24 hours.",
  "data": {
    "languageCode": "en",
    "languageName": "English",
    "aiScore": 45,
    "level": "Basic",
    "passed": false,
    "attemptsUsedThisMonth": 2,
    "nextRetryAvailableAt": "2026-03-09T16:30:00Z"
  }
}
```

**Response (400 — rate limited):**
```json
{
  "message": "Please wait until 2026-03-09 16:30 UTC before retrying.",
  "data": {
    "languageCode": "en",
    "languageName": "English",
    "attemptsUsedThisMonth": 2,
    "nextRetryAvailableAt": "2026-03-09T16:30:00Z"
  }
}
```

**Response (400 — monthly limit reached):**
```json
{
  "message": "Maximum 3 test attempts per language per month reached. Try again next month."
}
```

**Response (400 — Arabic):**
```json
{
  "message": "Arabic is auto-verified at Native level. No test required."
}
```

**Rate Limiting Rules:**
| Rule | Value |
|------|-------|
| Max attempts per language per month | 3 |
| Cooldown between attempts | 24 hours |
| Passing score | 60+ (Intermediate or above) |

**Score to Level Mapping:**

| Score Range | Level |
|------------|-------|
| 90-100 | Native |
| 80-89 | Fluent |
| 70-79 | Advanced |
| 60-69 | Intermediate |
| 40-59 | Basic |
| 0-39 | Beginner |

**What the app does next:**
- **Passed** → Show success animation with score and level badge, navigate back to Languages
- **Failed** → Show score, show countdown timer to next retry
- **Rate limited** → Show countdown timer, disable "Take Test" button

---

## STEP 10 — My Languages

Shows all languages the helper has been tested on.

---

### Screen 10.1: My Languages

| Field | Value |
|-------|-------|
| **Screen Name** | `MyLanguagesScreen` |
| **What helper sees** | List of languages with level badges, scores, verification status |
| **User action** | Views progress, taps language to retry test (if eligible) |
| **API endpoint** | `GET /api/Helper/languages/my` |

**Response (200):**
```json
{
  "message": "Your languages",
  "data": [
    {
      "id": 1,
      "languageCode": "ar",
      "languageName": "Arabic",
      "level": "Native",
      "aiScore": null,
      "testAttempts": 0,
      "lastTestedAt": null,
      "isVerified": true
    },
    {
      "id": 5,
      "languageCode": "en",
      "languageName": "English",
      "level": "Advanced",
      "aiScore": 82,
      "testAttempts": 1,
      "lastTestedAt": "2026-03-08T16:30:00Z",
      "isVerified": true
    },
    {
      "id": 8,
      "languageCode": "fr",
      "languageName": "French",
      "level": "Basic",
      "aiScore": 45,
      "testAttempts": 2,
      "lastTestedAt": "2026-03-07T10:00:00Z",
      "isVerified": false
    }
  ]
}
```

**UI Layout:**
```
┌──────────────────────────────────────┐
│  🇸🇦 Arabic          Native  ✅     │
│     Auto-verified                    │
├──────────────────────────────────────┤
│  🇬🇧 English         Advanced ✅    │
│     Score: 82 | Attempts: 1/3       │
├──────────────────────────────────────┤
│  🇫🇷 French          Basic    ❌    │
│     Score: 45 | Attempts: 2/3       │
│     [Retry Test] (available in 18h) │
└──────────────────────────────────────┘
```

**Logic:**
- `isVerified == true` → Green "Verified ✓" badge
- `isVerified == false` → Red "Not Passed" badge with retry option
- Arabic always shows as "Native ✅ Auto-verified"

---

## STEP 11 — Eligibility Check

Before the helper can accept bookings, all 5 conditions must be met.

---

### Screen 11.1: Eligibility Status

| Field | Value |
|-------|-------|
| **Screen Name** | `EligibilityScreen` or part of `HelperDashboardScreen` |
| **What helper sees** | Checklist of 5 eligibility conditions with pass/fail indicators |
| **User action** | Views status, taps failing items to fix them |
| **API endpoint** | `GET /api/Helper/eligibility` |

**Response (200 — not eligible):**
```json
{
  "message": "Eligibility check complete",
  "data": {
    "isEligible": false,
    "userVerified": true,
    "isApproved": false,
    "isActive": false,
    "hasValidDrugTest": false,
    "hasVerifiedLanguage": true,
    "blockingReasons": [
      "Helper is not approved by admin",
      "Helper account is not active",
      "No valid drug test on file (expired or missing)"
    ]
  }
}
```

**Response (200 — eligible):**
```json
{
  "message": "Eligibility check complete",
  "data": {
    "isEligible": true,
    "userVerified": true,
    "isApproved": true,
    "isActive": true,
    "hasValidDrugTest": true,
    "hasVerifiedLanguage": true,
    "blockingReasons": []
  }
}
```

**UI Layout:**
```
┌──────────────────────────────────────┐
│  ⚡ ELIGIBILITY CHECK               │
│                                      │
│  ✅ Email Verified                   │
│  ❌ Admin Approved → Waiting...      │
│  ❌ Account Active → Pending approval│
│  ❌ Valid Drug Test → [Upload Now]    │
│  ✅ Verified Language (Arabic)       │
│                                      │
│  Status: NOT ELIGIBLE                │
│  Fix 3 issues to start receiving     │
│  booking requests.                   │
└──────────────────────────────────────┘
```

**What the app does next:**
- `isEligible == true` → Show green "You're Active!" banner, enable booking features
- `isEligible == false` → Show blocking reasons as actionable items:
  - "No valid drug test" → Link to DrugTestScreen
  - "No verified language" → Link to LanguagesScreen
  - "Not approved by admin" → Show "Waiting for review" (no action)
  - "Not active" → Usually tied to approval or drug test expiry

---

## STEP 12 — Admin Review Cycle

The helper cannot control this step. The admin reviews their application through the Admin Dashboard.

---

### What Happens on the Backend

| Admin Action | Effect on Helper |
|-------------|-----------------|
| **Approve** | `isApproved = true`, `isActive = true`, `approvalStatus = "Approved"`, all certificates marked as verified |
| **Reject** | `approvalStatus = "Rejected"`, `rejectionReason` is set |
| **Request Changes** | `approvalStatus = "ChangesRequested"`, `rejectionReason` contains the required changes |

### How the App Detects Changes

The app calls `GET /api/Helper/status` when the dashboard opens. There is no push notification — the app polls on screen load.

**Scenario A — Approved:**
```json
{
  "data": {
    "computedStatus": "Active",
    "isApproved": true,
    "isActive": true,
    "approvalStatus": "Approved",
    "rejectionReason": null,
    "missingSteps": []
  }
}
```
→ Show "Congratulations!" celebration screen
→ Enable booking acceptance features

**Scenario B — Rejected:**
```json
{
  "data": {
    "computedStatus": "Rejected",
    "isApproved": false,
    "isActive": false,
    "approvalStatus": "Rejected",
    "rejectionReason": "National ID photo is not clear. Criminal record document is missing."
  }
}
```
→ Show red banner: "Application Rejected"
→ Show reason in a card below
→ Show "Contact Support" button

**Scenario C — Changes Requested:**
```json
{
  "data": {
    "computedStatus": "ChangesRequested",
    "isApproved": false,
    "isActive": false,
    "approvalStatus": "ChangesRequested",
    "rejectionReason": "Please re-upload your national ID with all 4 corners visible."
  }
}
```
→ Show orange banner: "Changes Requested"
→ Show the admin's feedback in a card
→ Highlight the relevant section with an "Update" button
→ When the helper re-submits (via `PUT /api/Helper/profile`), the status automatically resets to `"Pending"`

---

## Complete Navigation Map

```
App Launch
    │
    ├── Not logged in ──→ EmailScreen
    │                         │
    │                    PasswordScreen
    │                         │
    │                    OTPScreen (if needed)
    │                         │
    └── Logged in ──────→ HelperDashboardScreen
                              │
                    ┌─────────┼──────────────┐
                    │         │              │
              (404: no     (200: has      (200: active)
              helper)      helper)           │
                    │         │          BookingsScreen
                    ▼         ▼          (future feature)
            HelperRegisterScreen
                    │
                    ▼
            HelperDashboardScreen
                    │
          ┌────┬────┼────┬──────┬──────────┐
          │    │    │    │      │           │
          ▼    ▼    ▼    ▼      ▼           ▼
       Profile Drug  Car  Certs Languages  Eligibility
       Screen  Test       Screen Screen    Screen
                                  │
                                  ▼
                          LanguageTestScreen
                                  │
                                  ▼
                          MyLanguagesScreen
```

---

## Error Handling Reference

All API errors follow one of these patterns:

**Validation Error (400) — from ModelValidationFilter:**
```json
{
  "success": false,
  "message": "Validation failed",
  "errors": [
    "FullName: Full name is required.",
    "Gender: Gender is required."
  ],
  "statusCode": 400
}
```
→ Parse the `errors` array and show each message next to the relevant form field.

**Business Logic Error (400) — from controller:**
```json
{
  "message": "A helper profile already exists for this user."
}
```
→ Show as a toast/snackbar message.

**Not Found (404):**
```json
{
  "message": "No helper profile found. Please register first."
}
```
→ Navigate to registration screen.

**Unauthorized (401):**
→ Empty body with `www-authenticate: Bearer` header
→ Token expired — redirect to login screen, clear stored token.

**Server Error (500) — from GlobalExceptionMiddleware:**
```json
{
  "success": false,
  "message": "An unexpected error occurred. Please try again later.",
  "errors": null,
  "statusCode": 500
}
```
→ Show generic error dialog with "Retry" button.

---

## Recommended Flutter Packages

| Package | Purpose |
|---------|---------|
| `dio` or `http` | HTTP client |
| `flutter_secure_storage` | Store JWT token securely |
| `image_picker` | Camera/gallery file selection |
| `file_picker` | PDF document selection |
| `google_sign_in` | Google authentication |
| `riverpod` or `bloc` | State management |
| `go_router` | Navigation |
| `cached_network_image` | Display Cloudinary images |
| `intl` | Date formatting |
| `pin_code_fields` | OTP input UI |

---

## JWT Token Structure

The JWT token contains these claims (decode with `jwt_decoder` package):

```json
{
  "sub": "helper@example.com",
  "jti": "unique-id",
  "id": "5",
  "username": "AhmedHelper",
  "type": "user",
  "exp": 1741500000
}
```

- `id` — The user's database ID (used internally by the API)
- `type` — Always `"user"` for helpers (helpers ARE users with a linked Helper profile)
- `exp` — Token expiry (24 hours from login)

The app should check `exp` before making API calls and redirect to login if expired.
