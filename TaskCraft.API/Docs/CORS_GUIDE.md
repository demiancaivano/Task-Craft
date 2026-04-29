# 🌐 TaskCraft API - CORS Configuration Guide

## 📋 Table of Contents

- [What is CORS?](#what-is-cors)
- [Current Configuration](#current-configuration)
- [Configuration Options](#configuration-options)
- [Common Scenarios](#common-scenarios)
- [Troubleshooting](#troubleshooting)
- [Security Best Practices](#security-best-practices)

---

## 🔍 What is CORS?

**Cross-Origin Resource Sharing (CORS)** is a security feature implemented by web browsers that restricts web pages from making requests to a different domain than the one that served the web page.

### Why CORS Matters

When your frontend application (e.g., React app at `http://localhost:3000`) tries to communicate with your API (e.g., `https://localhost:5001`), the browser blocks the request by default for security reasons. CORS configuration tells the browser which origins are allowed to access your API.

### CORS Flow

```
┌─────────────────┐           ┌─────────────────┐
│  Frontend App   │           │   TaskCraft API │
│  localhost:3000 │           │  localhost:5001 │
└────────┬────────┘           └────────┬────────┘
         │                              │
         │  1. Request with Origin      │
         │ ─────────────────────────────>│
         │                              │
         │  2. Check CORS Policy        │
         │                              │
         │  3. Response with Headers    │
         │<─────────────────────────────│
         │  Access-Control-Allow-Origin │
         │                              │
```

---

## ⚙️ Current Configuration

### Code Location

**File:** `TaskCraft.API/Program.cs`

```csharp
// Lines 65-78
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
            ?? new[] { "http://localhost:3000", "http://localhost:5173" };

        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Line 143
app.UseCors("AllowReactApp");
```

### Configuration File

**File:** `TaskCraft.API/appsettings.json`

```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://localhost:5173"
    ]
  }
}
```

### Current Settings Explained

| Setting | Value | Description |
|---------|-------|-------------|
| **Policy Name** | `AllowReactApp` | The name of the CORS policy |
| **Origins** | `localhost:3000`, `localhost:5173` | Allowed frontend origins (React, Vite) |
| **Methods** | All (`*`) | Allows GET, POST, PUT, DELETE, PATCH, etc. |
| **Headers** | All (`*`) | Allows any header in requests |
| **Credentials** | Enabled | Allows cookies and authorization headers |

---

## 🛠️ Configuration Options

### 1. Allow Specific Origins (Current - Recommended)

✅ **Best for:** Production and security-conscious development

```csharp
policy.WithOrigins("http://localhost:3000", "https://app.taskcraft.com")
      .AllowAnyMethod()
      .AllowAnyHeader()
      .AllowCredentials();
```

**appsettings.json:**
```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "https://app.taskcraft.com"
    ]
  }
}
```

### 2. Allow Any Origin (Development Only)

⚠️ **Warning:** Never use in production!

```csharp
policy.AllowAnyOrigin()
      .AllowAnyMethod()
      .AllowAnyHeader();
      // Note: Cannot use .AllowCredentials() with AllowAnyOrigin()
```

### 3. Allow Specific Methods

```csharp
policy.WithOrigins("http://localhost:3000")
      .WithMethods("GET", "POST", "PUT", "DELETE")
      .AllowAnyHeader()
      .AllowCredentials();
```

### 4. Allow Specific Headers

```csharp
policy.WithOrigins("http://localhost:3000")
      .AllowAnyMethod()
      .WithHeaders("Content-Type", "Authorization")
      .AllowCredentials();
```

### 5. Disable Credentials

```csharp
policy.WithOrigins("http://localhost:3000")
      .AllowAnyMethod()
      .AllowAnyHeader();
      // No .AllowCredentials() - cookies/auth headers not allowed
```

### 6. Multiple Policies

```csharp
// Add multiple policies
builder.Services.AddCors(options =>
{
    // Policy for React app
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });

    // Policy for mobile app
    options.AddPolicy("AllowMobileApp", policy =>
    {
        policy.WithOrigins("capacitor://localhost", "ionic://localhost")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });

    // Public API (read-only)
    options.AddPolicy("PublicReadOnly", policy =>
    {
        policy.AllowAnyOrigin()
              .WithMethods("GET")
              .AllowAnyHeader();
    });
});

// Use different policies for different endpoints
app.MapControllers().RequireCors("AllowReactApp");
```

---

## 📝 Common Scenarios

### Scenario 1: Adding a New Frontend Port

**Use Case:** You're using Angular on port 4200

1. Open `appsettings.json`
2. Add the new origin:

```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://localhost:5173",
      "http://localhost:4200"
    ]
  }
}
```

3. Restart the API
4. Your Angular app can now communicate with the API

### Scenario 2: Deploying to Production

**Use Case:** Your frontend is deployed to `https://app.taskcraft.com`

**appsettings.Production.json:**
```json
{
  "Cors": {
    "AllowedOrigins": [
      "https://app.taskcraft.com",
      "https://www.taskcraft.com"
    ]
  }
}
```

**Important:**
- Use HTTPS in production
- Remove localhost origins
- Use exact domain names

### Scenario 3: Development + Production

**appsettings.Development.json:**
```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://localhost:5173",
      "http://localhost:4200"
    ]
  }
}
```

**appsettings.Production.json:**
```json
{
  "Cors": {
    "AllowedOrigins": [
      "https://app.taskcraft.com"
    ]
  }
}
```

### Scenario 4: Mobile Apps (Capacitor/Ionic)

```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "capacitor://localhost",
      "ionic://localhost"
    ]
  }
}
```

### Scenario 5: Subdomain Wildcard

```csharp
// Program.cs
policy.SetIsOriginAllowed(origin =>
{
    var uri = new Uri(origin);
    return uri.Host.EndsWith(".taskcraft.com") || uri.Host == "taskcraft.com";
})
.AllowAnyMethod()
.AllowAnyHeader()
.AllowCredentials();
```

This allows:
- `https://app.taskcraft.com`
- `https://admin.taskcraft.com`
- `https://api.taskcraft.com`

---

## 🐛 Troubleshooting

### Error: "CORS policy has blocked the request"

**Symptom:**
```
Access to fetch at 'https://localhost:5001/api/users' from origin 
'http://localhost:3000' has been blocked by CORS policy: 
No 'Access-Control-Allow-Origin' header is present.
```

**Solutions:**

1. **Check if origin is allowed:**
   - Verify `appsettings.json` includes your frontend origin
   - Ensure the origin is exact (including protocol and port)
   - Example: `http://localhost:3000` ≠ `https://localhost:3000`

2. **Verify middleware order:**
   ```csharp
   app.UseRouting();
   app.UseCors("AllowReactApp");  // CORS before Auth
   app.UseAuthentication();
   app.UseAuthorization();
   app.MapControllers();
   ```

3. **Restart the API:**
   - Configuration changes require restart
   - Stop debugging and run again

### Error: "Credentials flag is true, but Access-Control-Allow-Credentials is not"

**Symptom:**
```
CORS policy: The value of the 'Access-Control-Allow-Credentials' 
header in the response is '' which must be 'true' when the 
request's credentials mode is 'include'.
```

**Solution:**

Ensure `.AllowCredentials()` is in your policy:

```csharp
policy.WithOrigins("http://localhost:3000")
      .AllowAnyMethod()
      .AllowAnyHeader()
      .AllowCredentials();  // ← Add this
```

And in your frontend:

```javascript
// Fetch API
fetch('https://localhost:5001/api/users', {
  credentials: 'include'
});

// Axios
axios.get('https://localhost:5001/api/users', {
  withCredentials: true
});
```

### Error: "Preflight request doesn't pass"

**Symptom:**
```
OPTIONS request returns 404 or doesn't have CORS headers
```

**Solution:**

Ensure CORS middleware is added:

```csharp
app.UseCors("AllowReactApp");
app.MapControllers();
```

### Production: HTTPS Certificate Issues

**Symptom:**
```
Mixed Content: The page was loaded over HTTPS, but requested 
an insecure resource.
```

**Solution:**

- Ensure API uses HTTPS in production
- Update frontend to use `https://` URLs
- Configure proper SSL certificates

---

## 🔒 Security Best Practices

### ✅ DO

1. **Specify Exact Origins**
   ```json
   "AllowedOrigins": ["https://app.taskcraft.com"]
   ```

2. **Use HTTPS in Production**
   ```json
   "AllowedOrigins": ["https://app.taskcraft.com"]  // ✅
   ```

3. **Separate Dev/Prod Configs**
   - Use `appsettings.Development.json` for localhost
   - Use `appsettings.Production.json` for production domains

4. **Limit Methods if Possible**
   ```csharp
   policy.WithMethods("GET", "POST", "PUT", "DELETE")
   ```

5. **Monitor CORS Logs**
   - Log blocked requests
   - Review suspicious origins

### ❌ DON'T

1. **Never Use Wildcard in Production**
   ```csharp
   policy.AllowAnyOrigin()  // ❌ NEVER IN PRODUCTION!
   ```

2. **Don't Mix AllowAnyOrigin with AllowCredentials**
   ```csharp
   policy.AllowAnyOrigin()
         .AllowCredentials();  // ❌ This will throw an error!
   ```

3. **Don't Use HTTP in Production**
   ```json
   "AllowedOrigins": ["http://app.taskcraft.com"]  // ❌ Insecure!
   ```

4. **Don't Hardcode URLs in Code**
   ```csharp
   policy.WithOrigins("http://localhost:3000")  // ❌ Use config
   ```

   Use configuration instead:
   ```csharp
   var origins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
   policy.WithOrigins(origins)  // ✅
   ```

---

## 🧪 Testing CORS

### Using Browser DevTools

1. Open browser DevTools (F12)
2. Go to **Network** tab
3. Make a request to the API
4. Check the response headers:
   - `Access-Control-Allow-Origin`
   - `Access-Control-Allow-Methods`
   - `Access-Control-Allow-Headers`
   - `Access-Control-Allow-Credentials`

### Using cURL

```bash
# Test preflight request
curl -X OPTIONS \
  https://localhost:5001/api/users \
  -H "Origin: http://localhost:3000" \
  -H "Access-Control-Request-Method: GET" \
  -v

# Expected headers in response:
# Access-Control-Allow-Origin: http://localhost:3000
# Access-Control-Allow-Methods: GET, POST, PUT, DELETE, ...
# Access-Control-Allow-Credentials: true
```

### Using JavaScript

```javascript
// Test CORS from browser console
fetch('https://localhost:5001/api/users', {
  method: 'GET',
  credentials: 'include',
  headers: {
    'Content-Type': 'application/json'
  }
})
  .then(response => console.log('✅ CORS works!', response))
  .catch(error => console.error('❌ CORS blocked:', error));
```

---

## 📦 Complete Configuration Example

### appsettings.Development.json

```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",    // React (CRA)
      "http://localhost:5173",    // Vite
      "http://localhost:4200",    // Angular
      "http://localhost:8080"     // Vue
    ]
  }
}
```

### appsettings.Production.json

```json
{
  "Cors": {
    "AllowedOrigins": [
      "https://app.taskcraft.com",
      "https://www.taskcraft.com"
    ]
  },
  "JwtSettings": {
    "RequireHttpsMetadata": true
  }
}
```

### Program.cs (Complete)

```csharp
// ===== CORS Configuration =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
            ?? new[] { "http://localhost:3000", "http://localhost:5173" };

        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// ... build app ...

// ===== Middleware Pipeline =====
app.UseHttpsRedirection();
app.UseCors("AllowReactApp");  // Before Authentication
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
```

---

## 📚 Additional Resources

- [Microsoft Docs: Enable CORS](https://learn.microsoft.com/en-us/aspnet/core/security/cors)
- [MDN: CORS](https://developer.mozilla.org/en-US/docs/Web/HTTP/CORS)
- [API Guide](./API_GUIDE.md) - Complete API documentation

---

## 💡 Quick Reference

### Adding New Origin

1. Open `appsettings.json`
2. Add origin to `Cors:AllowedOrigins` array
3. Restart API
4. Test from frontend

### Middleware Order

```
1. UseErrorHandling()
2. UseHttpsRedirection()
3. UseCors()              ← BEFORE Auth
4. UseAuthentication()
5. UseAuthorization()
6. MapControllers()
```

### Common Ports

| Framework | Default Port |
|-----------|-------------|
| React (CRA) | 3000 |
| Vite | 5173 |
| Angular | 4200 |
| Vue | 8080 |
| Next.js | 3000 |

---

**Happy Coding! 🚀**
