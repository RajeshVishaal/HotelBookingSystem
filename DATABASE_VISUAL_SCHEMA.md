# Hotel Booking System 

## Entity Relationship Diagram

```
╔═══════════════════════════════════════════════════════════════════════════════════╗
║                              USER SERVICE DATABASE                                ║
╚═══════════════════════════════════════════════════════════════════════════════════╝

                                ┌──────────────────┐
                                │      USERS       │
                                ├──────────────────┤
                                │ Id (PK)          │
                                │ FirstName        │
                                │ LastName         │
                                │ EmailAddress (U) │
                                │ PasswordHash     │
                                │ CreatedAt        │
                                │ UpdatedAt        │
                                └────────┬─────────┘
                                         │
                                         │
                                         │

╔═══════════════════════════════════════════════════════════════════════════════════╗
║                          INVENTORY SERVICE DATABASE                               ║
╚═══════════════════════════════════════════════════════════════════════════════════╝

                    ┌─────────────────────────────────────────┐
                    │              HOTELS                     │
                    ├─────────────────────────────────────────┤
                    │ Id (PK)                                 │
                    │ Name                                    │
                    │ City                                    │
                    │ Country                                 │
                    │ AddressLine                             │
                    │ ImageUrl                                │
                    │ AverageRating                           │
                    │ TotalReviews                            │
                    └───┬──────────────────────────────┬──────┘
                        │                              │
        ┌───────────────┼──────────────┬───────────────┼──────────────┐
        │               │              │               │              │
        │               │              │               │              │
        ▼               ▼              ▼               ▼              ▼
┌──────────────┐ ┌──────────────┐ ┌─────────────────────────┐ ┌──────────────┐
│   REVIEWS    │ │    HOTEL     │ │   ROOM_CATEGORIES       │ │     ROOM     │
├──────────────┤ │ FACILITIES   │ ├─────────────────────────┤ │ AVAILABILITY │
│ Id (PK)      │ ├──────────────┤ │ Id (PK)                 │ ├──────────────┤
│ HotelId (FK) │ │ Id (PK)      │ │ HotelId (FK) ───────────┤ │ Id (PK)      │
│ UserId       │ │ HotelId (FK) │ │ RoomTypeId (FK) ──┐     │ │ HotelId (FK) │
│ Rating       │ │ Name         │ │ MaximumGuests     │     │ │ RoomCatId(FK)│
│ Comment      │ │ Category     │ │ ImageUrls (JSON)  │     │ │ Date (U)     │
│ CreatedAt    │ │ IsAvailable  │ │ Facilities (JSON) │     │ │ TotalCount   │
└──────────────┘ └──────────────┘ └──┬──────────────┬─┘     │ │ TotalBooked  │
                                     │              │       │ │ RowVersion   │
                ┌────────────────────┤              │       │ └──────────────┘
                │                    │              │       │
                ▼                    ▼              ▼       │
        ┌──────────────┐     ┌──────────────┐  ┌─────────────┐
        │     ROOM     │     │     ROOM     │  │ BED_CONFIGS │
        │  FACILITIES  │     │   PRICING    │  ├─────────────┤
        ├──────────────┤     ├──────────────┤  │ Id (PK)     │
        │ Id (PK)      │     │ Id (PK)      │  │ RoomCatId   │
        │ RoomCatId    │     │ RoomCatId    │  │ BedTypeId◄──┤
        │ Name         │     │ Date         │  │ Quantity    │
        │ Category     │     │ BaseRate     │  └─────────────┘
        │ IsAvailable  │     │ CreatedAt    │         │
        └──────────────┘     └──────────────┘         │
                                                       ▼
        ┌─────────────────────────────┐       ┌──────────────┐
        │       ROOM_TYPES            │       │  BED_TYPES   │
        ├─────────────────────────────┤       ├──────────────┤
        │ Id (PK)                     │       │ Id (PK)      │
        │ Name (U) [Single, Double,   │       │ Name (U)     │
        │          Deluxe, Suite]     │       │ Description  │
        │ Description                 │       │ Capacity     │
        │ CreatedAt                   │       └──────────────┘
        └─────────────────────────────┘


╔═══════════════════════════════════════════════════════════════════════════════════╗
║                           BOOKING SERVICE DATABASE                                ║
╚═══════════════════════════════════════════════════════════════════════════════════╝

                        ┌────────────────────────────────┐
                        │         BOOKINGS               │
                        ├────────────────────────────────┤
                        │ Id (PK)                        │
                        │ BookingReference (U)           │
                        │ IdempotencyKey (U) ⚡           │
                        │ UserId (logical)               │
                        │ HotelId (logical)              │
                        │ CheckIn                        │
                        │ CheckOut                       │
                        │ Guests                         │
                        │ TotalCost                      │
                        │ Status                         │
                        │ CreatedAt                      │
                        └────────────┬───────────────────┘
                                     │
                                     │
                                     ▼
                        ┌────────────────────────────────┐
                        │     BOOKING_ROOM_INFO          │
                        ├────────────────────────────────┤
                        │ Id (PK)                        │
                        │ BookingId (FK)                 │
                        │ RoomCategoryId (logical)       │
                        │ Quantity                       │
                        │ BaseRate                       │
                        │ Subtotal                       │
                        └────────────────────────────────┘


╔═══════════════════════════════════════════════════════════════════════════════════╗
║                         CROSS-SERVICE RELATIONSHIPS                               ║
╚═══════════════════════════════════════════════════════════════════════════════════╝

    USERS                         ──────writes────────▶    REVIEWS
    (User Service)                                         (Inventory Service)


    USERS                         ──────makes─────────▶    BOOKINGS
    (User Service)                                         (Booking Service)


    BOOKINGS                      ────references─────▶     HOTELS
    (Booking Service)                                      (Inventory Service)


    BOOKING_ROOM_INFO             ────references─────▶     ROOM_CATEGORIES
    (Booking Service)                                      (Inventory Service)


═══════════════════════════════════════════════════════════════════════════════════════

LEGEND:
───────

  (PK)       Primary Key
  (FK)       Foreign Key
  (U)        Unique Constraint
  ⚡         Special Purpose Column
  (logical)  Cross-service reference (not DB-enforced)
  ────▶      Relationship/Connection

═══════════════════════════════════════════════════════════════════════════════════════
```

---

## 📊 Database Tables Summary

### User Service (1 table)
| Table | Purpose | Key Columns |
|-------|---------|-------------|
| **USERS** | User accounts & authentication | Id, EmailAddress, PasswordHash |

### Inventory Service (10 tables)
| Table | Purpose | Key Columns |
|-------|---------|-------------|
| **HOTELS** | Hotel properties | Id, Name, City, Country |
| **ROOM_TYPES** | Room type catalog | Id, Name (Single/Double/Deluxe) |
| **ROOM_CATEGORIES** | Hotel-specific room config | Id, HotelId, RoomTypeId |
| **ROOM_AVAILABILITY** | Daily room inventory | Id, Date, TotalCount, TotalBooked |
| **ROOM_PRICING** | Date-specific pricing | Id, RoomCategoryId, Date, BaseRate |
| **ROOM_FACILITIES** | Room-level amenities | Id, RoomCategoryId, Name |
| **HOTEL_FACILITIES** | Hotel-level amenities | Id, HotelId, Name |
| **BED_TYPES** | Bed type catalog | Id, Name (King/Queen/Single) |
| **BED_CONFIGS** | Beds per room category | Id, RoomCategoryId, BedTypeId |
| **REVIEWS** | Hotel reviews | Id, HotelId, UserId, Rating |

### Booking Service (2 tables)
| Table | Purpose | Key Columns |
|-------|---------|-------------|
| **BOOKINGS** | Booking records | Id, BookingReference, IdempotencyKey |
| **BOOKING_ROOM_INFO** | Rooms in each booking | Id, BookingId, RoomCategoryId |

---

## 🔑 Key Relationships

### Within Inventory Service
```
HOTELS
  ├── ROOM_CATEGORIES (which rooms the hotel has)
  ├── ROOM_AVAILABILITY (daily inventory per room type)
  ├── HOTEL_FACILITIES (amenities like pool, gym)
  └── REVIEWS (user reviews)

ROOM_CATEGORIES
  ├── ROOM_TYPES (what type: Single/Double/Deluxe)
  ├── ROOM_PRICING (daily rates)
  ├── ROOM_FACILITIES (amenities like WiFi, TV)
  ├── ROOM_AVAILABILITY (daily inventory)
  └── BED_CONFIGS (bed configuration)

BED_CONFIGS
  └── BED_TYPES (what beds: King/Queen/Single)
```

### Within Booking Service
```
BOOKINGS
  └── BOOKING_ROOM_INFO (what rooms were booked)
```
