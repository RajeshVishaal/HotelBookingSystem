# Hotel Booking System - Microservices Platform

> **Deployed on Azure, built with .NET 9 and microservices architecture**

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-336791?style=for-the-badge&logo=postgresql)](https://www.postgresql.org/)
[![Azure](https://img.shields.io/badge/Azure-Container_Apps-0078D4?style=for-the-badge&logo=microsoftazure)](https://azure.microsoft.com/)
[![Docker](https://img.shields.io/badge/Docker-Containerized-2496ED?style=for-the-badge&logo=docker)](https://www.docker.com/)

🖥️ Frontend : https://overbooked-ui.vercel.app

⚙️ Backend (API Gateway): https://api-gateway.happyforest-8a2b009f.uksouth.azurecontainerapps.io

### Interactive API Documentation (Swagger UI)

| Service | Swagger UI | Status |
|---------|-----------|--------|
| **Inventory API** | [Open Swagger →](https://api-gateway.happyforest-8a2b009f.uksouth.azurecontainerapps.io/inventory-service/swagger/index.html) | 🟢 Online |
| **User API** | [Open Swagger →](https://api-gateway.happyforest-8a2b009f.uksouth.azurecontainerapps.io/user-service/swagger/index.html) | 🟢 Online |
| **Booking API** | [Open Swagger →](https://api-gateway.happyforest-8a2b009f.uksouth.azurecontainerapps.io/booking-service/swagger/index.html) | 🟢 Online |


### Core Features
- ✅ **User Management** - Registration, authentication with BCrypt password hashing
- ✅ **Hotel Search** - Advanced filtering by location, dates, capacity, and price
- ✅ **Booking Management** - Idempotent reservations with unique reference codes
- ✅ **Enterprise Error Handling** - Standardized responses, global exception middleware
- ✅ **API Gateway** - Unified entry point with intelligent routing (Ocelot)
- ✅ **Swagger Documentation** - Interactive API testing for all services
- ✅ **Cloud Deployment** - Production deployment on Azure Container Apps

### Technical Highlights
- 🔹 **Microservices Architecture** - Independently deployable services
- 🔹 **Clean Architecture** - Domain, Application, Infrastructure, API layers
- 🔹 **Repository Pattern** - Abstracted data access layer
- 🔹 **CQRS Pattern** - Command Query Responsibility Segregation
- 🔹 **Docker Containerization** - Consistent environments


## 🏗️ Architecture

```
                        ☁️  Azure Cloud
                              │
                              ▼
                    ┌─────────────────────┐
                    │   Load Balancer     │
                    │   (Azure Managed)   │
                    └──────────┬──────────┘
                               │ HTTPS (443)
                               ▼
                    ┌─────────────────────┐
                    │   API Gateway       │
                    │   Port: 8080        │
                    │                     │
                    │  • Route Management │
                    │  • Load Balancing   │
                    └──────────┬──────────┘
                               │
          ┌────────────────────┼────────────────────┐
          │                    │                    │
          ▼                    ▼                    ▼
  ┌───────────────┐    ┌──────────────┐    ┌─────────────┐
  │ User Service  │    │  Inventory   │    │  Booking    │
  │               │    │   Service    │    │  Service    │
  │ Port: 8080    │    │ Port: 8080   │    │ Port: 8080  │
  │               │    │              │    │             │
  │ • Auth        │    │ • Hotels     │    │ • Bookings  │
  │ • Users       │    │ • Rooms      │    │ • Payments  │
  │ • Profiles    │    │ • Search     │    │ • References│
  └───────┬───────┘    └──────┬───────┘    └──────┬──────┘
          │                   │                    │
          └───────────────────┼────────────────────┘
                              │ 
                              ▼
                  ┌─────────────────────────┐
                  │ PostgreSQL              │       
                  │ • User DB               │
                  │ • Inventory DB          │
                  │ • Booking DB            │    
                  └─────────────────────────┘
```
----
### Service Boundaries

| Service | Domain | Responsibilities | Database Tables |
|---------|--------|------------------|-----------------|
| **User Service** | Identity & Access | Authentication, User management | Users |
| **Inventory Service** | Hotel Catalog | Hotel search, Room availability | Hotels, RoomCategories, RoomTypes, RoomPricing, RoomAvailability |
| **Booking Service** | Reservations | Booking creation, Reference management | Bookings, BookingRoomInfo |
| **API Gateway** | Routing | Request routing, Load balancing |

## Run Locally

```bash
# Clone the repository
git clone https://github.com/RajeshVishaal/HotelBookingSystem.git
cd HotelBookingSystem

# Start all services (builds automatically)
docker compose up -d --build

# Wait for services to be ready (~30 seconds)
docker compose ps

# View logs
docker compose logs -f

# Stop services
docker compose down
```

## Few Live API Endpoints

All endpoints tested with real production data.

All endpoints tested with real production data from Azure Container Apps**

## Hotel Search & Discovery

### 1. Search Hotels

**Endpoint:** `GET /hotels/search`

**Query Parameters:**
- `name` - Search by hotel name or city (optional)
- `checkIn` - Check-in date in YYYY-MM-DD format (optional)
- `checkOut` - Check-out date in YYYY-MM-DD format (optional)
- `guests` - Number of guests (optional)
- `page` - Page number (default: 1)
- `pageSize` - Results per page (default: 10, max: 100)

**Request Example:**
```bash
curl "https://api-gateway.happyforest-8a2b009f.uksouth.azurecontainerapps.io/hotels/search?name=London&page=1&pageSize=2"
```

**Real Response:**
```json
{
  "success": true,
  "message": "Search completed successfully.",
  "data": {
    "items": [
      {
        "hotelId": "0166e8a9-cf42-4e90-8a56-01f93dde0c3b",
        "name": "London Central Luxe",
        "city": "London",
        "country": "United Kingdom",
        "addressLine": "476 Reina Village, Westminster",
        "imageUrl": "https://i.ibb.co/DgGRkhMd/poster.jpg",
        "averageRating": 6.8,
        "totalReviews": 1818,
        "comment": "Decent budget option with basic amenities.",
        "roomCategories": [
          {
            "id": "90292515-a4e8-4381-9eda-544d6e85c95c",
            "roomTypeName": "Single Room",
            "maximumGuests": 1,
            "baseRate": 176.45,
            "info": "1 guests"
          },
          {
            "id": "0ace965a-e496-4af7-8b4c-a2d80af9e37c",
            "roomTypeName": "Double Room",
            "maximumGuests": 2,
            "baseRate": 261.46,
            "info": "2 guests"
          },
          {
            "id": "04f5f0a3-d3b3-4427-b75a-649526770c1c",
            "roomTypeName": "Deluxe Room",
            "maximumGuests": 3,
            "baseRate": 210.09,
            "info": "3 guests"
          }
        ]
      }
    ],
    "totalRecords": 2,
    "pageNo": 1,
    "pageSize": 2,
    "hasNext": false,
    "hasPrevious": false
  }
}
```

---

### 2. Get Hotel Details

**Endpoint:** `GET /hotels/{hotelId}`

**Path Parameters:**
- `hotelId` - Hotel UUID

**Query Parameters (Optional):**
- `checkIn` - Check-in date (YYYY-MM-DD)
- `checkOut` - Check-out date (YYYY-MM-DD)
- `guests` - Number of guests

**Request Example:**
```bash
curl "https://api-gateway.happyforest-8a2b009f.uksouth.azurecontainerapps.io/hotels/0166e8a9-cf42-4e90-8a56-01f93dde0c3b"
```

**Real Response:**
```json
{
  "success": true,
  "message": "Hotel details retrieved successfully.",
  "data": {
    "hotelId": "0166e8a9-cf42-4e90-8a56-01f93dde0c3b",
    "hotelName": "London Central Luxe",
    "city": "London",
    "country": "United Kingdom",
    "addressLine": "476 Reina Village, Westminster",
    "imageUrl": "https://i.ibb.co/DgGRkhMd/poster.jpg",
    "averageRating": 6.8,
    "totalReviews": 1818,
    "comment": "Decent budget option with basic amenities.",
    "facilities": [
      "Free WiFi",
      "Gym",
      "24-Hour Reception",
      "Air Conditioning"
    ],
    "rooms": [
      {
        "id": "90292515-a4e8-4381-9eda-544d6e85c95c",
        "roomTypeName": "Single Room",
        "maximumGuests": 1,
        "baseRate": 176.45,
        "imageUrls": [
          "https://i.ibb.co/mptHVZG/295688351.jpg",
          "https://i.ibb.co/Nnxmzxwc/295688415.jpg"
        ],
        "facilities": [
          "Air Conditioning",
          "Free WiFi",
          "Mini Bar"
        ]
      },
      {
        "id": "0ace965a-e496-4af7-8b4c-a2d80af9e37c",
        "roomTypeName": "Double Room",
        "maximumGuests": 2,
        "baseRate": 261.46,
        "imageUrls": [
          "https://i.ibb.co/q1kbqmyp/295688386.jpg",
          "https://i.ibb.co/XbkXMb89/295688359.jpg"
        ],
        "facilities": [
          "Air Conditioning",
          "Free WiFi",
          "Mini Bar",
          "Coffee Maker"
        ]
      },
      {
        "id": "04f5f0a3-d3b3-4427-b75a-649526770c1c",
        "roomTypeName": "Deluxe Room",
        "maximumGuests": 3,
        "baseRate": 210.09,
        "imageUrls": [
          "https://i.ibb.co/DgGRkhMd/poster.jpg"
        ],
        "facilities": [
          "Air Conditioning",
          "Free WiFi",
          "Mini Bar",
          "Smart TV",
          "Balcony"
        ]
      }
    ]
  }
}
```

---

### 3. Get Hotel Summary (Basic Info)

**Endpoint:** `GET /hotels/{hotelId}/info`

**Path Parameters:**
- `hotelId` - Hotel UUID

**Request Example:**
```bash
curl "https://api-gateway.happyforest-8a2b009f.uksouth.azurecontainerapps.io/hotels/0166e8a9-cf42-4e90-8a56-01f93dde0c3b/info"
```

**Real Response:**
```json
{
  "success": true,
  "message": "Hotel summary retrieved successfully.",
  "data": {
    "hotelId": "0166e8a9-cf42-4e90-8a56-01f93dde0c3b",
    "id": "0166e8a9-cf42-4e90-8a56-01f93dde0c3b",
    "name": "London Central Luxe",
    "city": "London",
    "country": "United Kingdom",
    "addressLine": "476 Reina Village, Westminster",
    "imageUrl": "https://i.ibb.co/DgGRkhMd/poster.jpg",
    "averageRating": 6.8,
    "totalReviews": 0,
    "comment": "Decent budget option with basic amenities."
  }
}
```

---

## User Management

### 4. User Registration (Signup)

**Endpoint:** `POST /auth/signup`

**Request Headers:**
- `Content-Type: application/json`

**Request Body:**
```json
{
  "emailAddress": "user@example.com",
  "password": "SecurePass123!",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+44 20 1234 5678"
}
```

**Request Example:**
```bash
curl -X POST "https://api-gateway.happyforest-8a2b009f.uksouth.azurecontainerapps.io/auth/signup" \
-H "Content-Type: application/json" \
-d '{
  "emailAddress": "john.doe@example.com",
  "password": "SecurePass123!",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+44 20 1234 5678"
}'
```

**Real Response:**
```json
{
  "success": true,
  "message": "User registered successfully",
  "data": {
    "userId": "ab362be4-1951-4a3f-aabf-3bf4490d69f5",
    "firstName": "John",
    "lastName": "Doe",
    "emailAddress": "john.doe@example.com",
    "message": "User registered successfully"
  }
}
```

---

### 5. User Login

**Endpoint:** `POST /auth/login`

**Request Headers:**
- `Content-Type: application/json`

**Request Body:**
```json
{
  "emailAddress": "user@example.com",
  "password": "SecurePass123!"
}
```

**Request Example:**
```bash
curl -X POST "https://api-gateway.happyforest-8a2b009f.uksouth.azurecontainerapps.io/auth/login" \
-H "Content-Type: application/json" \
-d '{
  "emailAddress": "john.doe@example.com",
  "password": "SecurePass123!"
}'
```

**Response Structure:**
```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "userId": "ab362be4-1951-4a3f-aabf-3bf4490d69f5",
    "emailAddress": "john.doe@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
  }
}
```

---

## Booking Management

### 6. Create Booking (Reserve Room)

**Endpoint:** `POST /bookings/reserve`

**Request Headers:**
- `Content-Type: application/json`
- `Idempotency-Key: {unique-uuid}` ⚠️ **REQUIRED**

**Request Body:**
```json
{
  "userId": "ab362be4-1951-4a3f-aabf-3bf4490d69f5",
  "hotelId": "0166e8a9-cf42-4e90-8a56-01f93dde0c3b",
  "checkIn": "2026-02-15",
  "checkOut": "2026-02-20",
  "guests": 2,
  "rooms": [
    {
      "roomCategoryId": "0ace965a-e496-4af7-8b4c-a2d80af9e37c",
      "quantity": 1
    }
  ]
}
```

**Request Example:**
```bash
curl -X POST "https://api-gateway.happyforest-8a2b009f.uksouth.azurecontainerapps.io/bookings/reserve" \
-H "Content-Type: application/json" \
-H "Idempotency-Key: $(uuidgen)" \
-d '{
  "userId": "ab362be4-1951-4a3f-aabf-3bf4490d69f5",
  "hotelId": "0166e8a9-cf42-4e90-8a56-01f93dde0c3b",
  "checkIn": "2026-02-15",
  "checkOut": "2026-02-20",
  "guests": 2,
  "rooms": [
    {
      "roomCategoryId": "0ace965a-e496-4af7-8b4c-a2d80af9e37c",
      "quantity": 1
    }
  ]
}'
```

**Real Response:**
```json
{
  "success": true,
  "message": "Booking created successfully.",
  "data": {
    "hotelId": "0166e8a9-cf42-4e90-8a56-01f93dde0c3b",
    "checkIn": "2026-02-15",
    "checkOut": "2026-02-20",
    "totalCost": 1307.30,
    "rooms": [
      {
        "roomCategoryId": "0ace965a-e496-4af7-8b4c-a2d80af9e37c",
        "quantity": 1,
        "pricePerNight": 261.46,
        "subtotal": 1307.30
      }
    ]
  }
}
```

**Calculation:** 5 nights × £261.46/night = £1,307.30

---

### 7. Get Booking by Reference

**Endpoint:** `GET /bookings/ref/{bookingReference}`

**Path Parameters:**
- `bookingReference` - Unique booking reference (e.g., `BK-01K8M3GZX9HF`)

**Request Example:**
```bash
curl "https://api-gateway.happyforest-8a2b009f.uksouth.azurecontainerapps.io/bookings/ref/BK-01K8M3GZX9HF"
```

**Response Structure:**
```json
{
  "success": true,
  "message": "Booking details retrieved successfully.",
  "data": {
    "bookingReference": "BK-01K8M3GZX9HF",
    "hotelId": "0166e8a9-cf42-4e90-8a56-01f93dde0c3b",
    "hotelName": "London Central Luxe",
    "hotelImageUrl": "https://i.ibb.co/DgGRkhMd/poster.jpg",
    "userId": "ab362be4-1951-4a3f-aabf-3bf4490d69f5",
    "checkIn": "2026-02-15",
    "checkOut": "2026-02-20",
    "guests": 2,
    "totalCost": 1307.30,
    "createdAt": "2025-10-28T15:45:00Z",
    "rooms": [
      {
        "roomCategoryId": "0ace965a-e496-4af7-8b4c-a2d80af9e37c",
        "quantity": 1,
        "baseRate": 261.46,
        "subtotal": 1307.30
      }
    ]
  }
}
```

---

## Response Patterns

### Standard Success Response
```json
{
  "success": true,
  "message": "Operation completed successfully",
  "data": { /* Response payload */ }
}
```

### Standard Error Response
```json
{
  "success": false,
  "message": "Human-readable error description",
  "error": {
    "type": "ValidationException",
    "field": "email",
    "details": "Additional error context"
  }
}
```

### Error Response Examples

**404 Not Found:**
```json
{
  "success": false,
  "message": "Hotel with ID '123e4567-e89b-12d3-a456-426614174000' was not found.",
  "error": {
    "type": "NotFoundException",
    "resourceName": "Hotel",
    "resourceId": "123e4567-e89b-12d3-a456-426614174000"
  }
}
```

**400 Validation Error:**
```json
{
  "success": false,
  "message": "Email address cannot be empty.",
  "error": {
    "type": "ValidationException",
    "parameter": "email"
  }
}
```

**409 Conflict:**
```json
{
  "success": false,
  "message": "User with this email already exists",
  "error": {
    "type": "ConflictException",
    "conflictingField": "emailAddress"
  }
}
```

### Pagination Response
```json
{
  "success": true,
  "message": "Results retrieved successfully",
  "data": {
    "items": [ /* Array of results */ ],
    "totalRecords": 120,
    "pageNo": 1,
    "pageSize": 10,
    "hasNext": true,
    "hasPrevious": false
  }
}
```
---
