# Microservices Solution: API Gateway & AuthService

This solution demonstrates a simple microservices architecture using .NET 8/9, featuring an **API Gateway Service** and an **AuthService** for authentication and authorization using JWT tokens.

---

## Directory Structure

```
Compie/
│
├── APIGatewayService/
└── AuthService/
    
```
### APIGatewayService
```
├── APIGatewayService/
│   └── APIGatewayService/
│       ├── Controllers/
│       │   └── APIGatewayController.cs
│       ├── Middlewares/
│       │   └── RequestResponseLoggingMiddleware.cs
│       ├── Models/
│       │   └── UserDetailsDto.cs
│       ├── Services/
│       │   ├── AuthServiceClient.cs
│       │   └── IAuthenticationService.cs
│       ├── Program.cs
│       ├── appsettings.json
│       └── ...
```

### AuthService
```
└── AuthService/
    └── AuthService/
        ├── Controllers/
        │   └── AuthController.cs
        ├── Models/
        │   ├── LoginRequest.cs
        │   ├── LoginResponse.cs
        │   └── User.cs
        ├── Repositories/
        │   ├── IUserRepository.cs
        │   └── InMemoryUserRepository.cs
        ├── Services/
        │   ├── AuthService.cs
        │   └── IAuthService.cs
        ├── Program.cs
        ├── appsettings.json
        └── ...
```

---

## System Architecture & Decisions

### Overview

- **API Gateway Service**:  
  - Entry point for all client requests.
  - Forwards authentication requests to AuthService.
  - Validates JWT tokens by calling AuthService.
  - Logs all requests and responses.
  - Supports routing and access control.
- **AuthService**:  
  - Handles user login, logout, and token validation.
  - Issues JWT tokens with user roles.
  - Maintains in-memory user store (for demo purposes).
  - Supports token revocation (logout).

### Key Decisions

- **JWT Authentication**:  
  - Chosen for stateless, scalable authentication.
  - Tokens include user roles for role-based access.
- **Service Communication**:  
  - API Gateway communicates with AuthService via HTTP.
  - `AuthServiceClient` abstracts the HTTP calls.
- **Separation of Concerns**:  
  - Each service has clear responsibilities.
  - Dependency Injection is used throughout.
- **Logging**:  
  - Middleware logs all requests and responses in the gateway.
- **Extensibility**:  
  - Interfaces allow for easy swapping of implementations (e.g., real DB, distributed cache).

---

## How to Run the Solution

### Prerequisites

- [.NET 8 or 9 SDK](https://dotnet.microsoft.com/download)
- (Optional) [Postman](https://www.postman.com/) or similar tool for testing APIs

### 1. Start AuthService

```bash
cd AuthService/AuthService
dotnet run
```
- By default, runs on `http://localhost:5100` (check `launchSettings.json` or output).

### 2. Start APIGatewayService

```bash
cd APIGatewayService/APIGatewayService
dotnet run
```
- By default, runs on `http://localhost:5000`.

### 3. Try the APIs

- **Swagger UI** is available for both services at `/swagger`.
- **Login**:  
  `POST http://localhost:5100/api/Auth/login`  
  Body:  
  ```json
  {
    "userName": "admin",
    "password": "password"
  }
  ```
- **Access Protected Endpoint via Gateway**:  
  `GET http://localhost:5000/api/gateway/user/me`  
  - Add `Authorization: Bearer {token}` header (token from login).

---

## Known Limitations, Assumptions, and Shortcuts

- **In-Memory User Store**:  
  - Users are hardcoded for demo; no persistent storage.
- **Token Revocation**:  
  - Logout is implemented by tracking revoked tokens in memory (not scalable for production).
- **No HTTPS by default**:  
  - For demo simplicity; enable HTTPS for production.
- **No distributed cache/session**:  
  - All state is local to the service instance.
- **No refresh tokens**:  
  - JWTs expire after a set time; no refresh mechanism.
- **No email verification or password reset**.
- **No rate limiting or advanced security features**.

---

## Bonus Features

- **JWT Authentication**:  
  - Secure, stateless, includes user roles.
- **Role Support**:  
  - JWT tokens include user roles for future role-based access control.
- **Request/Response Logging**:  
  - All gateway requests and responses are logged.
- **Swagger/OpenAPI**:  
  - Both services expose Swagger UI for easy testing and documentation.
- **Extensible Architecture**:  
  - Easy to swap in real databases, distributed caches, or external identity providers.

---

## Contact

For questions or improvements, please open an issue or pull request on
