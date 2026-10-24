# Technical Test: Logic & Algorithms and Dual-Stack RESTful API

**Author:** Fachriza (FR)  
**Solutions Included:**
1. **Logic and Algorithm Implementation (C#)** – Saved in `FR_solution.cs` (and `algorithms/FR_solution.cs`)
2. **User Management CRUD API (ASP.NET Core 8 & Entity Framework Core SQLite)**
3. **User Management CRUD API (Golang 1.27 & Gin Framework SQLite)**

---

## Repository Structure

```
.
├── FR_solution.cs             # Single standalone C# solution file for Part 1
├── algorithms/
│   ├── FR_solution.cs         # Algorithm implementations
│   ├── Program.cs             # Comprehensive test runner with edge cases & benchmarks
│   └── Algorithms.csproj      # .NET 8 Project file
├── aspnet-user-api/           # ASP.NET Core 8 Web API
│   ├── Controllers/
│   │   └── UsersController.cs # RESTful API endpoints
│   ├── Models/
│   │   └── User.cs            # Domain entity
│   ├── DTOs/
│   │   └── UserDtos.cs        # Input validation DTOs & response contracts
│   ├── Data/
│   │   └── AppDbContext.cs    # EF Core DbContext with SQLite
│   ├── Services/
│   │   ├── IUserService.cs    # Business service interface
│   │   └── UserService.cs     # Business logic & validations
│   ├── Program.cs             # Application configuration & Swagger
│   ├── appsettings.json       # SQLite connection string
│   └── AspNetUserApi.csproj
├── gin-user-api/              # Golang + Gin RESTful API
│   ├── cmd/server/
│   │   └── main.go            # Entry point & routing
│   ├── internal/
│   │   ├── models/user.go     # User model & request validation tags
│   │   ├── handlers/user.go   # Gin HTTP handlers
│   │   └── repository/db.go   # SQLite data access layer
│   ├── go.mod
│   └── go.sum
├── tests/
│   └── api-tests.http         # Executable REST client test suite
└── README.md                  # Project documentation & run guide
```

---

## Part 1: Logic and Algorithm Implementation (C#)

The solutions are contained in the standalone file [FR_solution.cs](file:///Users/fachriza/Dev/meditap-test/FR_solution.cs).

### 1. Problem 1: Reverse a String (`StringReverser`)
- **Method Signature**: `public static string? ReverseString(string? inputStr)`
- **Thought Process**:
  - In C#, `string` is immutable; constructing any reversed string requires allocating new character memory.
  - Converting the input to a `char[]` buffer and performing an in-place two-pointer swap (`left` moving right, `right` moving left) achieves $O(n)$ time complexity with only $n/2$ operations.
  - Short-circuit paths: If `inputStr` is `null`, returns `null`. If length is $\le 1$, returns the string itself without allocation.
- **Complexity**:
  - **Time Complexity**: $\mathcal{O}(n)$, single pass.
  - **Space Complexity**: $\mathcal{O}(n)$ (required for returning the new string; auxiliary space is $\mathcal{O}(1)$).
- **Trade-offs & Alternatives**:
  - *Naive Concatenation (`str += c`)*: $\mathcal{O}(n^2)$ due to re-allocating a string on every character. Avoided.
  - *LINQ (`inputStr.Reverse().ToArray()`)*: Idiomatic but incurs LINQ iterator state machine overhead and extra allocations.

---

### 2. Problem 2: Palindrome Check (`PalindromeChecker`)
- **Method Signature**: `public static bool IsPalindrome(string? inputStr)`
- **Thought Process**:
  - In real-world coding and technical tests, palindromes are evaluated either strictly or symmetrically ignoring casing and non-alphanumeric punctuation (e.g. `"A man, a plan, a canal: Panama"`, `"Was it a car or a cat I saw?"`).
  - We use a two-pointer approach starting from both ends (`left = 0`, `right = length - 1`).
  - Non-alphanumeric characters are bypassed without creating a sanitized copy of the string, keeping auxiliary memory to zero.
  - Characters are compared case-insensitively using `char.ToLowerInvariant`.
  - For strict exact character comparison, `IsExactPalindrome(string? inputStr)` is also provided.
- **Complexity**:
  - **Time Complexity**: $\mathcal{O}(n)$, single pass.
  - **Space Complexity**: $\mathcal{O}(1)$ auxiliary space. Unlike solutions that reverse the string and compare (`Reverse(s) == s`), this approach allocates $0$ additional heap memory.

---

### 3. Problem 3: Prime Number Generator (`PrimeNumberGenerator`)
- **Method Signature**: `public static List<int> GeneratePrimes(int limit)`
- **Thought Process**:
  - If `limit < 2`, returns an empty list `[]` (the smallest prime is 2).
  - Selected Algorithm: **Sieve of Eratosthenes** ($\mathcal{O}(N \log \log N)$).
  - Compared to trial division ($\mathcal{O}(N \sqrt{N})$), the Sieve is orders of magnitude faster (e.g., generating 1,000,000 primes takes under 5 ms).
  - **Optimizations Applied**:
    - Uses `bool[] isComposite` where default `false` represents prime, eliminating array initialization overhead.
    - Outer loop only iterates up to $\sqrt{limit}$.
    - Inner loop starts at $p \times p$ (multiples $< p^2$ are already marked).
    - 64-bit integer cast `(long)p * p` prevents 32-bit integer overflow when limit is large.
    - Capacity pre-allocation using the Prime Number Theorem ($\pi(N) \approx \frac{N}{\ln N}$) avoids dynamic list resizing.
- **Complexity**:
  - **Time Complexity**: $\mathcal{O}(N \log \log N)$.
  - **Space Complexity**: $\mathcal{O}(N)$ for the boolean sieve array.

---

### How to Run the Algorithm Tests

Execute the automated test runner in `algorithms/`:

```bash
dotnet run --project algorithms/Algorithms.csproj
```

**Output Preview:**
```
==================================================================
   TECHNICAL TEST: LOGIC AND ALGORITHM IMPLEMENTATION (C#)       
==================================================================

--- [Problem 1: Reverse a String] ---
  [PASS] Input: "hello"          -> Output: "olleh"          Expected: "olleh"
  [PASS] Input: "World!"         -> Output: "!dlroW"         Expected: "!dlroW"
  [PASS] Input: "1234567890"     -> Output: "0987654321"     Expected: "0987654321"
  Summary: 8/8 test cases passed.

--- [Problem 2: Palindrome Check] ---
  [PASS] Normalized: True  | Exact: False | Input: "A man, a plan, a canal: Panama"
  [PASS] Normalized: True  | Exact: False | Input: "Was it a car or a cat I saw?"
  [PASS] Normalized: True  | Exact: True  | Input: "racecar"
  Summary: Normalized: 12/12 passed | Exact: 12/12 passed.

--- [Problem 3: Prime Number Generator] ---
  [PASS] Limit:   10 -> Found  4 primes: [2, 3, 5, 7]
  [PASS] Limit:   30 -> Found 10 primes: [2, 3, 5, 7, 11, 13, 17, 19, 23, 29]
  [PASS] Benchmark Limit 1,000,000: Generated 78,498 primes in 4 ms (Expected 78,498)
  Summary: 7/7 edge cases passed + large sieve verified.
```

---

## Part 2: User Management CRUD API (Dual-Stack Architecture)

We have implemented two identical, production-ready RESTful APIs:
1. **ASP.NET Core 8 Web API** (`aspnet-user-api/`)
2. **Golang + Gin Framework** (`gin-user-api/`)

Both APIs follow consistent RESTful URL conventions, identical JSON schemas, identical validation rules, and proper HTTP response status codes.

### Unified RESTful Endpoints Specification

| HTTP Method | Route | Description | Status Code | Error Responses |
|---|---|---|---|---|
| `POST` | `/api/users` | Create a new user | `201 Created` | `400 Bad Request` (validation), `409 Conflict` (duplicate email) |
| `GET` | `/api/users` | Retrieve all users | `200 OK` | `500 Internal Server Error` |
| `GET` | `/api/users/{userId}` | Retrieve single user | `200 OK` | `400 Bad Request` (invalid ID), `404 Not Found` |
| `PUT` | `/api/users/{userId}` | Update existing user | `200 OK` | `400 Bad Request`, `404 Not Found`, `409 Conflict` |
| `DELETE` | `/api/users/{userId}` | Delete user | `204 No Content` | `400 Bad Request` (invalid ID), `404 Not Found` |

### User Model Schema

```json
{
  "userId": 1,
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "createdAt": "2026-10-24T04:23:26.135Z",
  "updatedAt": "2026-10-24T04:25:58.457Z"
}
```

### Input Validation Rules
- `firstName`: Required, 1-50 characters.
- `lastName`: Required, 1-50 characters.
- `email`: Required, valid email format (RFC-compliant), unique across all users.

---

## Running the APIs

### 1. ASP.NET Core API (`aspnet-user-api/`)

```bash
# Navigate to the project directory
cd aspnet-user-api

# Run the API (listens on http://localhost:5000)
dotnet run --urls "http://localhost:5000"
```

- **Swagger / OpenAPI Documentation**: Open `http://localhost:5000/swagger` in your browser.
- **Database**: Automatically initializes `users.db` SQLite database on startup.

---

### 2. Golang Gin API (`gin-user-api/`)

```bash
# Navigate to the project directory
cd gin-user-api

# Run the API (listens on http://localhost:8080)
go run ./cmd/server/main.go
```

- **Database**: Automatically initializes `users_gin.db` SQLite database with auto-migrations.
- **Port**: Default is `8080` (can be overridden via `PORT=8080`).

---

## Testing the APIs

### Automated HTTP Client Suite
Open [tests/api-tests.http](file:///Users/fachriza/Dev/meditap-test/tests/api-tests.http) in VS Code (with REST Client extension) or JetBrains IDE to execute all test requests sequentially against either server.

### Example curl Commands

#### 1. Create a User (`POST /api/users`)
```bash
curl -X POST http://localhost:5000/api/users \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com"
  }'
```
*Response: `201 Created` with `Location` header.*

#### 2. Get All Users (`GET /api/users`)
```bash
curl http://localhost:5000/api/users
```
*Response: `200 OK` `[ { "userId": 1, "firstName": "John", ... } ]`*

#### 3. Get User by ID (`GET /api/users/1`)
```bash
curl http://localhost:5000/api/users/1
```
*Response: `200 OK`*

#### 4. Update User (`PUT /api/users/1`)
```bash
curl -X PUT http://localhost:5000/api/users/1 \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Johnathan",
    "lastName": "Doe",
    "email": "johnathan.doe@example.com"
  }'
```
*Response: `200 OK`*

#### 5. Delete User (`DELETE /api/users/1`)
```bash
curl -i -X DELETE http://localhost:5000/api/users/1
```
*Response: `204 No Content`*

#### 6. Verify Deletion (`GET /api/users/1`)
```bash
curl -i http://localhost:5000/api/users/1
```
*Response: `404 Not Found`*

*(To test against the Golang Gin server, simply replace port `5000` with `8080`)*
