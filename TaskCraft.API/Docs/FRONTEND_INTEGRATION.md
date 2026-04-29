# 🎨 Frontend Integration - CORS Examples

## 📋 Table of Contents

- [React Examples](#react-examples)
- [Vue.js Examples](#vuejs-examples)
- [Angular Examples](#angular-examples)
- [Fetch API](#fetch-api)
- [Axios](#axios)
- [Common Issues](#common-issues)

---

## ⚛️ React Examples

### Using Fetch API

```javascript
// src/services/api.js
const API_BASE_URL = 'https://localhost:5001/api';

// Login
export async function login(usernameOrEmail, password) {
  const response = await fetch(`${API_BASE_URL}/auth/login`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    credentials: 'include', // Important for cookies/credentials
    body: JSON.stringify({
      usernameOrEmail,
      password
    })
  });

  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.message);
  }

  return response.json();
}

// Get Users (with JWT)
export async function getUsers(token) {
  const response = await fetch(`${API_BASE_URL}/users`, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    },
    credentials: 'include'
  });

  if (!response.ok) {
    throw new Error('Failed to fetch users');
  }

  return response.json();
}
```

### Using Axios

```bash
npm install axios
```

```javascript
// src/services/api.js
import axios from 'axios';

const api = axios.create({
  baseURL: 'https://localhost:5001/api',
  withCredentials: true, // Important for cookies/credentials
  headers: {
    'Content-Type': 'application/json'
  }
});

// Request interceptor (add JWT token)
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('accessToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor (handle errors)
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    if (error.response?.status === 401) {
      // Token expired - try to refresh
      const refreshToken = localStorage.getItem('refreshToken');
      if (refreshToken) {
        try {
          const { data } = await axios.post(
            'https://localhost:5001/api/auth/refresh',
            { refreshToken }
          );
          localStorage.setItem('accessToken', data.accessToken);
          // Retry original request
          error.config.headers.Authorization = `Bearer ${data.accessToken}`;
          return api.request(error.config);
        } catch (refreshError) {
          // Refresh failed - logout
          localStorage.clear();
          window.location.href = '/login';
        }
      }
    }
    return Promise.reject(error);
  }
);

export default api;

// Usage in components
export const authService = {
  login: (usernameOrEmail, password) =>
    api.post('/auth/login', { usernameOrEmail, password }),

  register: (userData) =>
    api.post('/auth/register', userData),

  logout: (refreshToken) =>
    api.post('/auth/revoke', { refreshToken })
};

export const userService = {
  getAll: () => api.get('/users'),
  getById: (id) => api.get(`/users/${id}`),
  create: (userData) => api.post('/users', userData),
  update: (id, userData) => api.put(`/users/${id}`, userData),
  delete: (id) => api.delete(`/users/${id}`)
};
```

### React Component Example

```javascript
// src/components/UserList.jsx
import React, { useEffect, useState } from 'react';
import api, { userService } from '../services/api';

function UserList() {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    loadUsers();
  }, []);

  const loadUsers = async () => {
    try {
      setLoading(true);
      const response = await userService.getAll();
      setUsers(response.data);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  if (loading) return <div>Loading...</div>;
  if (error) return <div>Error: {error}</div>;

  return (
    <div>
      <h2>Users</h2>
      <ul>
        {users.map(user => (
          <li key={user.id}>
            {user.firstName} {user.lastName} - {user.email}
          </li>
        ))}
      </ul>
    </div>
  );
}

export default UserList;
```

### Environment Variables

```bash
# .env.development
REACT_APP_API_URL=http://localhost:5001/api

# .env.production
REACT_APP_API_URL=https://api.taskcraft.com/api
```

```javascript
// src/services/api.js
const api = axios.create({
  baseURL: process.env.REACT_APP_API_URL,
  withCredentials: true
});
```

---

## 🟢 Vue.js Examples

### Using Axios

```bash
npm install axios
```

```javascript
// src/services/api.js
import axios from 'axios';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5001/api',
  withCredentials: true,
  headers: {
    'Content-Type': 'application/json'
  }
});

// Request interceptor
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('accessToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

export default api;
```

### Vue 3 Composition API

```vue
<!-- src/components/UserList.vue -->
<template>
  <div>
    <h2>Users</h2>
    <div v-if="loading">Loading...</div>
    <div v-else-if="error">Error: {{ error }}</div>
    <ul v-else>
      <li v-for="user in users" :key="user.id">
        {{ user.firstName }} {{ user.lastName }} - {{ user.email }}
      </li>
    </ul>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import api from '../services/api';

const users = ref([]);
const loading = ref(true);
const error = ref(null);

const loadUsers = async () => {
  try {
    loading.value = true;
    const response = await api.get('/users');
    users.value = response.data;
  } catch (err) {
    error.value = err.message;
  } finally {
    loading.value = false;
  }
};

onMounted(() => {
  loadUsers();
});
</script>
```

### Environment Variables (Vite)

```bash
# .env.development
VITE_API_URL=http://localhost:5001/api

# .env.production
VITE_API_URL=https://api.taskcraft.com/api
```

---

## 🅰️ Angular Examples

### HTTP Client Service

```typescript
// src/app/services/api.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('accessToken');
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': token ? `Bearer ${token}` : ''
    });
  }

  // Users
  getUsers(): Observable<any> {
    return this.http.get(`${this.baseUrl}/users`, {
      headers: this.getHeaders(),
      withCredentials: true
    });
  }

  getUserById(id: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/users/${id}`, {
      headers: this.getHeaders(),
      withCredentials: true
    });
  }

  // Auth
  login(usernameOrEmail: string, password: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/auth/login`, 
      { usernameOrEmail, password },
      { withCredentials: true }
    );
  }
}
```

### HTTP Interceptor

```typescript
// src/app/interceptors/auth.interceptor.ts
import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = localStorage.getItem('accessToken');

    if (token) {
      req = req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        },
        withCredentials: true
      });
    }

    return next.handle(req);
  }
}
```

### Module Configuration

```typescript
// src/app/app.module.ts
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from './interceptors/auth.interceptor';

@NgModule({
  imports: [
    HttpClientModule,
    // ...
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    }
  ]
})
export class AppModule { }
```

### Environment Files

```typescript
// src/environments/environment.ts
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5001/api'
};

// src/environments/environment.prod.ts
export const environment = {
  production: true,
  apiUrl: 'https://api.taskcraft.com/api'
};
```

---

## 🌐 Fetch API

### Basic Example

```javascript
const API_URL = 'https://localhost:5001/api';

// GET request
async function getUsers() {
  const token = localStorage.getItem('accessToken');

  const response = await fetch(`${API_URL}/users`, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    },
    credentials: 'include'
  });

  if (!response.ok) {
    throw new Error(`HTTP error! status: ${response.status}`);
  }

  return await response.json();
}

// POST request
async function createUser(userData) {
  const token = localStorage.getItem('accessToken');

  const response = await fetch(`${API_URL}/users`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    },
    credentials: 'include',
    body: JSON.stringify(userData)
  });

  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.message);
  }

  return await response.json();
}

// PUT request
async function updateUser(id, userData) {
  const token = localStorage.getItem('accessToken');

  const response = await fetch(`${API_URL}/users/${id}`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    },
    credentials: 'include',
    body: JSON.stringify(userData)
  });

  if (!response.ok) {
    throw new Error('Failed to update user');
  }

  return await response.json();
}

// DELETE request
async function deleteUser(id) {
  const token = localStorage.getItem('accessToken');

  const response = await fetch(`${API_URL}/users/${id}`, {
    method: 'DELETE',
    headers: {
      'Authorization': `Bearer ${token}`
    },
    credentials: 'include'
  });

  if (!response.ok) {
    throw new Error('Failed to delete user');
  }
}
```

---

## 📦 Axios

### Configuration

```javascript
import axios from 'axios';

const api = axios.create({
  baseURL: 'https://localhost:5001/api',
  withCredentials: true,
  headers: {
    'Content-Type': 'application/json'
  }
});

// Add token to all requests
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('accessToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Handle errors globally
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.clear();
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export default api;
```

### Usage

```javascript
import api from './api';

// GET
const users = await api.get('/users');
console.log(users.data);

// POST
const newUser = await api.post('/users', {
  firstName: 'John',
  lastName: 'Doe',
  email: 'john@example.com'
});

// PUT
const updated = await api.put('/users/123', {
  firstName: 'Jane'
});

// DELETE
await api.delete('/users/123');
```

---

## 🐛 Common Issues

### Issue 1: CORS Error Despite Configuration

**Problem:**
```
Access to fetch at 'https://localhost:5001/api/users' from origin 
'http://localhost:3000' has been blocked by CORS policy
```

**Solution:**
1. Verify origin is in `appsettings.json`
2. Ensure exact match (protocol + domain + port)
3. Restart the API

### Issue 2: Missing Credentials

**Problem:**
```
CORS policy: Credentials flag is true, but the 
'Access-Control-Allow-Credentials' header is ''
```

**Solution:**

Backend:
```csharp
policy.AllowCredentials();
```

Frontend:
```javascript
// Fetch
credentials: 'include'

// Axios
withCredentials: true
```

### Issue 3: Preflight Request Fails

**Problem:**
```
OPTIONS request returns 404
```

**Solution:**

Ensure CORS middleware is added:
```csharp
app.UseCors("AllowReactApp");
```

### Issue 4: Token Not Sent

**Problem:**
Authorization header is missing in requests

**Solution:**

Always add the token:
```javascript
headers: {
  'Authorization': `Bearer ${token}`
}
```

### Issue 5: HTTPS Certificate Error (Development)

**Problem:**
```
NET::ERR_CERT_AUTHORITY_INVALID
```

**Solution:**

Trust the development certificate:
```bash
dotnet dev-certs https --trust
```

Or disable SSL verification (dev only):
```javascript
// NOT FOR PRODUCTION
process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';
```

---

## 📚 Additional Resources

- [CORS Guide](./CORS_GUIDE.md) - Backend CORS configuration
- [API Guide](./API_GUIDE.md) - Complete API documentation
- [Swagger Guide](./SWAGGER_GUIDE.md) - API testing

---

**Happy Coding! 🚀**
