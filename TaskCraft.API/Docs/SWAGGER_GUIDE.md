# 📘 TaskCraft API - Swagger Documentation Guide

## 🎯 Overview

This document provides a comprehensive guide to using and understanding the TaskCraft API through Swagger/OpenAPI documentation.

---

## 🚀 Quick Start

### Accessing Swagger UI

1. **Development Environment:**
   - URL: `https://localhost:{port}/`
   - Example: `https://localhost:5001/`

2. **Staging Environment:**
   - URL: `https://staging.taskcraft.com/`

> **Note:** Swagger is disabled in Production for security reasons.

---

## 🔐 Authentication

### How to Authenticate in Swagger UI

1. **Get JWT Token:**
   - Navigate to `/api/auth/login` endpoint
   - Click "Try it out"
   - Enter credentials:
     ```json
     {
       "usernameOrEmail": "your-email@example.com",
       "password": "your-password"
     }
     ```
   - Click "Execute"
   - Copy the `accessToken` from the response

2. **Authorize Swagger:**
   - Click the 🔓 **Authorize** button at the top right
   - In the dialog, enter: `Bearer {your-access-token}`
   - Example: `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`
   - Click "Authorize"
   - Click "Close"

3. **Make Authenticated Requests:**
   - All subsequent requests will now include your JWT token
   - Protected endpoints will now work properly

---

## 📚 API Endpoints Overview

### **Authentication** (`/api/auth`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/auth/login` | Login with credentials | ❌ No |
| POST | `/api/auth/register` | Register new account | ❌ No |
| POST | `/api/auth/refresh` | Refresh access token | ❌ No |
| POST | `/api/auth/revoke` | Revoke refresh token | ✅ Yes |
| POST | `/api/auth/validate` | Validate refresh token | ❌ No |

### **Users** (`/api/users`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/users` | Get all users | ✅ Yes |
| GET | `/api/users/{id}` | Get user by ID | ✅ Yes |
| GET | `/api/users/email/{email}` | Get user by email | ✅ Yes |
| GET | `/api/users/username/{username}` | Get user by username | ✅ Yes |
| POST | `/api/users` | Create new user | ✅ Yes |
| PUT | `/api/users/{id}` | Update user | ✅ Yes |
| DELETE | `/api/users/{id}` | Delete user (soft) | ✅ Yes |
| DELETE | `/api/users/{id}/permanent` | Delete user (permanent) | ✅ Yes |

### **Projects** (`/api/projects`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/projects` | Get all projects | ✅ Yes |
| GET | `/api/projects/{id}` | Get project by ID | ✅ Yes |
| POST | `/api/projects` | Create new project | ✅ Yes |
| PUT | `/api/projects/{id}` | Update project | ✅ Yes |
| DELETE | `/api/projects/{id}` | Delete project | ✅ Yes |
| POST | `/api/projects/{id}/members` | Add project member | ✅ Yes |
| DELETE | `/api/projects/{projectId}/members/{userId}` | Remove member | ✅ Yes |

### **Tasks** (`/api/tasks`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/tasks` | Get all tasks | ✅ Yes |
| GET | `/api/tasks/{id}` | Get task by ID | ✅ Yes |
| GET | `/api/tasks/project/{projectId}` | Get tasks by project | ✅ Yes |
| POST | `/api/tasks` | Create new task | ✅ Yes |
| PUT | `/api/tasks/{id}` | Update task | ✅ Yes |
| DELETE | `/api/tasks/{id}` | Delete task | ✅ Yes |
| POST | `/api/tasks/{id}/assign` | Assign task to user | ✅ Yes |
| DELETE | `/api/tasks/{taskId}/unassign/{userId}` | Unassign user | ✅ Yes |

---

## 📋 Request/Response Examples

### 1. **User Login**

**Request:**
```http
POST /api/auth/login
Content-Type: application/json

{
  "usernameOrEmail": "john.doe@example.com",
  "password": "SecurePassword123!"
}
```

**Success Response (200):**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6...",
  "tokenType": "Bearer",
  "expiresIn": 3600,
  "user": {
    "id": "a1b2c3d4-e5f6-g7h8-i9j0-k1l2m3n4o5p6",
    "username": "johndoe",
    "email": "john.doe@example.com",
    "firstName": "John",
    "lastName": "Doe"
  }
}
```

**Error Response (401):**
```json
{
  "statusCode": 401,
  "errorCode": "UNAUTHORIZED",
  "message": "Invalid username or password",
  "timestamp": "2024-01-15T10:30:00.000Z",
  "path": "/api/auth/login"
}
```

### 2. **User Registration**

**Request:**
```http
POST /api/auth/register
Content-Type: application/json

{
  "username": "janedoe",
  "email": "jane.doe@example.com",
  "firstName": "Jane",
  "lastName": "Doe",
  "password": "SecurePassword123!"
}
```

**Success Response (200):**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "z9y8x7w6v5u4t3s2r1q0p9o8n7m6l5k4...",
  "tokenType": "Bearer",
  "expiresIn": 3600,
  "user": {
    "id": "b2c3d4e5-f6g7-h8i9-j0k1-l2m3n4o5p6q7",
    "username": "janedoe",
    "email": "jane.doe@example.com",
    "firstName": "Jane",
    "lastName": "Doe"
  }
}
```

**Error Response (409 - Conflict):**
```json
{
  "statusCode": 409,
  "errorCode": "CONFLICT",
  "message": "Email 'jane.doe@example.com' is already registered",
  "timestamp": "2024-01-15T10:30:00.000Z",
  "path": "/api/auth/register"
}
```

**Error Response (400 - Validation):**
```json
{
  "statusCode": 400,
  "errorCode": "VALIDATION_ERROR",
  "message": "One or more validation errors occurred.",
  "errors": {
    "Email": ["Email is required", "Email format is invalid"],
    "Password": ["Password must be at least 8 characters"]
  },
  "timestamp": "2024-01-15T10:30:00.000Z",
  "path": "/api/auth/register"
}
```

### 3. **Create Project**

**Request:**
```http
POST /api/projects
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "name": "Website Redesign",
  "description": "Complete overhaul of company website",
  "startDate": "2024-02-01T00:00:00Z",
  "endDate": "2024-06-30T00:00:00Z"
}
```

**Success Response (201):**
```json
{
  "id": "c3d4e5f6-g7h8-i9j0-k1l2-m3n4o5p6q7r8",
  "name": "Website Redesign",
  "description": "Complete overhaul of company website",
  "startDate": "2024-02-01T00:00:00Z",
  "endDate": "2024-06-30T00:00:00Z",
  "createdAt": "2024-01-15T10:30:00.000Z",
  "ownerId": "a1b2c3d4-e5f6-g7h8-i9j0-k1l2m3n4o5p6"
}
```

### 4. **Get User by ID**

**Request:**
```http
GET /api/users/a1b2c3d4-e5f6-g7h8-i9j0-k1l2m3n4o5p6
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Success Response (200):**
```json
{
  "id": "a1b2c3d4-e5f6-g7h8-i9j0-k1l2m3n4o5p6",
  "username": "johndoe",
  "email": "john.doe@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "isActive": true,
  "createdAt": "2023-07-15T08:00:00.000Z",
  "updatedAt": "2024-01-10T14:30:00.000Z"
}
```

**Error Response (404):**
```json
{
  "statusCode": 404,
  "errorCode": "NOT_FOUND",
  "message": "User with identifier 'a1b2c3d4-e5f6-g7h8-i9j0-k1l2m3n4o5p6' was not found.",
  "timestamp": "2024-01-15T10:30:00.000Z",
  "path": "/api/users/a1b2c3d4-e5f6-g7h8-i9j0-k1l2m3n4o5p6"
}
```

---

## 🎨 Swagger UI Features

### **1. Try It Out**
- Click "Try it out" on any endpoint
- Fill in required parameters
- Click "Execute" to make a real API call
- View response in real-time

### **2. Request/Response Schema**
- Click on "Schema" tab to view data structure
- Shows all available fields and their types
- Displays validation rules and constraints

### **3. Example Values**
- Click "Example Value" to see sample data
- Use as template for your requests
- Copy and modify as needed

### **4. Response Codes**
- View all possible HTTP status codes
- See example responses for each code
- Understand error scenarios

### **5. Models**
- Scroll to bottom to view all DTOs
- Click to expand and see structure
- Reference for request/response formats

---

## 🔍 Common HTTP Status Codes

| Code | Name | Description |
|------|------|-------------|
| 200 | OK | Request succeeded |
| 201 | Created | Resource created successfully |
| 204 | No Content | Request succeeded, no content to return |
| 400 | Bad Request | Invalid request format or validation errors |
| 401 | Unauthorized | Authentication required or invalid token |
| 403 | Forbidden | Authenticated but not authorized |
| 404 | Not Found | Resource not found |
| 409 | Conflict | Resource already exists |
| 500 | Internal Server Error | Server error occurred |

---

## 🎯 Best Practices

### **1. Always Test with Swagger First**
- Validate your request/response formats
- Test error scenarios
- Verify authentication flow

### **2. Use Example Values**
- Modify example data for testing
- Test edge cases
- Validate field requirements

### **3. Check Response Schemas**
- Review all available fields
- Understand data types
- Note optional vs required fields

### **4. Test Error Handling**
- Intentionally trigger errors
- Verify error response format
- Test validation rules

### **5. Document Your Findings**
- Save example requests
- Note any issues or bugs
- Share with team members

---

## 🛠️ Swagger Configuration

### **Enabled Environments:**
- ✅ Development
- ✅ Staging
- ❌ Production (disabled for security)

### **Features Enabled:**
- ✅ JWT Bearer Authentication
- ✅ XML Documentation Comments
- ✅ Request/Response Examples
- ✅ Schema Validation
- ✅ Deep Linking
- ✅ Filter Support
- ✅ Request Duration Display

### **OpenAPI Specification:**
- Version: 3.0
- Format: JSON
- Endpoint: `/swagger/v1/swagger.json`

---

## 📦 Integration

### **Import to Postman:**
1. Open Postman
2. Click "Import"
3. Enter URL: `https://localhost:{port}/swagger/v1/swagger.json`
4. Click "Import"
5. All endpoints will be added to Postman

### **Generate Client Code:**
1. Visit Swagger UI
2. Download OpenAPI spec
3. Use code generators:
   - Swagger Codegen
   - OpenAPI Generator
   - NSwag
4. Generate client libraries for your language

---

## 🔒 Security Notes

### **Token Management:**
- Access tokens expire after 1 hour
- Refresh tokens expire after 7 days
- Always store tokens securely
- Never expose tokens in URLs

### **Rate Limiting:**
- 100 requests per minute per IP
- 1000 requests per hour per user
- Excessive requests may be throttled

### **CORS Policy:**
- Allowed origins configured in `appsettings.json`
- Default: `http://localhost:3000`, `http://localhost:5173`
- Credentials allowed for authenticated requests

---

## 📞 Support

### **Issues or Questions:**
- Email: support@taskcraft.com
- GitHub: https://github.com/taskcraft/api
- Documentation: https://docs.taskcraft.com

### **API Version:**
- Current Version: v1
- Last Updated: January 2024
- Changelog: See `/api/changelog`

---

## 🎉 Happy Testing!

Use Swagger UI to explore and test the TaskCraft API. All endpoints are documented with examples, schemas, and error responses.

**Pro Tip:** Use the "Authorize" button at the top to authenticate once, then test all protected endpoints without re-entering tokens!

---

**Version**: 1.0  
**Last Updated**: January 2024  
**Status**: ✅ Active
