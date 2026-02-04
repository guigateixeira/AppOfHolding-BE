# Bag of Holding - Backend Implementation Plan

## Strategy Overview

We'll implement this in **6 phases**, building from foundation to features:

1. **Project Setup** - NuGet packages, folder structure, configuration
2. **Database & Models** - EF Core entities, DbContext, migrations
3. **Authentication** - User registration, login, JWT middleware
4. **Core Features** - Bags, items catalog, bag items CRUD
5. **Invitations** - Invite system with tokens and expiration
6. **Real-time** - SignalR for live updates

Each phase builds on the previous, so we can test as we go.

---

## Phase 1: Project Setup

- [x] Add NuGet packages (EF Core, Npgsql, JWT, SignalR, BCrypt)
- [x] Create folder structure (Models, Data, Controllers, Services, Hubs, DTOs)
- [x] Configure PostgreSQL connection string in appsettings (with env var support)
- [x] Set up dependency injection structure
- [x] Configure CORS for frontend

---

## Phase 2: Database & Models

### Entities
- [ ] Create `User` entity
- [ ] Create `BagOfHolding` entity
- [ ] Create `BagAccess` entity with Role enum (Owner, Member)
- [ ] Create `Invitation` entity with Status enum (Pending, Accepted, Expired)
- [ ] Create `Item` entity (catalog)
- [ ] Create `BagItem` entity (item instance in bag)
- [ ] Create `ItemHistory` entity with Action enum

### DbContext & Config
- [ ] Create `AppDbContext` with all DbSets
- [ ] Configure entity relationships (FKs, unique constraints)
- [ ] Configure indexes for performance
- [ ] Create initial migration
- [ ] Apply migration to database

### Seed Data
- [ ] Create seed data for global items (Potion of Healing, Longsword, etc.)
- [ ] Configure seeding in DbContext or migration

---

## Phase 3: Authentication

### Models & DTOs
- [ ] Create `RegisterRequest` DTO
- [ ] Create `LoginRequest` DTO
- [ ] Create `AuthResponse` DTO (with token)

### Services
- [ ] Create `IAuthService` interface
- [ ] Implement `AuthService` (register, login, password hashing)
- [ ] Create `IJwtService` interface
- [ ] Implement `JwtService` (token generation, validation)

### Configuration
- [ ] Add JWT settings to appsettings (Secret, Issuer, Audience, Expiry)
- [ ] Configure JWT authentication middleware
- [ ] Create `[Authorize]` policy

### Controller
- [ ] Create `AuthController`
- [ ] POST `/auth/register` - create user, return JWT
- [ ] POST `/auth/login` - validate credentials, return JWT

### Testing
- [ ] Test registration flow
- [ ] Test login flow
- [ ] Test protected endpoint access

---

## Phase 4: Core Features

### Bags

#### DTOs
- [ ] Create `CreateBagRequest` DTO
- [ ] Create `BagResponse` DTO
- [ ] Create `BagDetailResponse` DTO

#### Service
- [ ] Create `IBagService` interface
- [ ] Implement `BagService` (create, list, get, delete with access checks)

#### Controller
- [ ] Create `BagsController`
- [ ] POST `/bags` - create bag (auto-add owner to bag_access)
- [ ] GET `/bags` - list user's bags
- [ ] GET `/bags/{bagId}` - get bag details (access check)
- [ ] DELETE `/bags/{bagId}` - delete bag (owner only)

### Items Catalog

#### DTOs
- [ ] Create `CreateItemRequest` DTO
- [ ] Create `ItemResponse` DTO

#### Service
- [ ] Create `IItemService` interface
- [ ] Implement `ItemService` (list global + user items, create custom)

#### Controller
- [ ] Create `ItemsController`
- [ ] GET `/items` - list all items (global + user's custom)
- [ ] POST `/items` - create custom item
- [ ] GET `/items/{itemId}` - get item details

### Bag Items

#### DTOs
- [ ] Create `AddBagItemRequest` DTO
- [ ] Create `UpdateBagItemRequest` DTO
- [ ] Create `BagItemResponse` DTO

#### Service
- [ ] Create `IBagItemService` interface
- [ ] Implement `BagItemService` (add, update quantity, remove, log history)

#### Controller
- [ ] Add bag items endpoints to `BagsController`
- [ ] GET `/bags/{bagId}/items` - list items in bag
- [ ] POST `/bags/{bagId}/items` - add item (increment if exists)
- [ ] PUT `/bags/{bagId}/items/{bagItemId}` - update quantity
- [ ] DELETE `/bags/{bagId}/items/{bagItemId}` - remove item

#### History Logging
- [ ] Implement `ItemHistory` logging in BagItemService
- [ ] Log `Added`, `Removed`, `Updated`, `QuantityChanged` actions

---

## Phase 5: Invitations

#### DTOs
- [ ] Create `CreateInvitationRequest` DTO (optional email)
- [ ] Create `InvitationResponse` DTO
- [ ] Create `InvitePreviewResponse` DTO

#### Service
- [ ] Create `IInvitationService` interface
- [ ] Implement `InvitationService`
  - [ ] Generate secure random tokens
  - [ ] Create invitation with expiration
  - [ ] Validate token (check expiry, return 410 if expired)
  - [ ] Accept invitation (update status, add to bag_access)

#### Controller
- [ ] Create invitation endpoints
- [ ] POST `/bags/{bagId}/invitations` - create invite (owner only)
- [ ] GET `/bags/{bagId}/invitations` - list invitations (owner only)
- [ ] GET `/bags/invite/{token}` - validate & preview
- [ ] POST `/bags/invite/{token}/accept` - accept invitation

#### Expiration Handling
- [ ] Return 410 Gone for expired tokens
- [ ] Consider background job to clean up expired invitations (optional)

---

## Phase 6: Real-time (SignalR)

#### Hub Setup
- [ ] Create `BagHub` SignalR hub
- [ ] Implement `JoinBag(bagId)` - add to group (with access check)
- [ ] Implement `LeaveBag(bagId)` - remove from group

#### Events
- [ ] Define `ItemAdded` event payload
- [ ] Define `ItemRemoved` event payload
- [ ] Define `ItemUpdated` event payload

#### Integration
- [ ] Inject `IHubContext<BagHub>` into `BagItemService`
- [ ] Broadcast `ItemAdded` when item added
- [ ] Broadcast `ItemRemoved` when item removed
- [ ] Broadcast `ItemUpdated` when quantity changed

#### Authentication
- [ ] Configure SignalR to use JWT authentication
- [ ] Validate user access before joining bag group

---

## Phase 7: Polish & Testing (Optional)

- [ ] Add global exception handling middleware
- [ ] Add request validation (FluentValidation or DataAnnotations)
- [ ] Add Swagger/OpenAPI documentation
- [ ] Write unit tests for services
- [ ] Write integration tests for API endpoints
- [ ] Add rate limiting (optional)
- [ ] Add logging with Serilog (optional)

---

## Quick Reference: Folder Structure

```
AppOfHolding/
├── Controllers/
│   ├── AuthController.cs
│   ├── BagsController.cs
│   └── ItemsController.cs
├── Data/
│   ├── AppDbContext.cs
│   └── Migrations/
├── DTOs/
│   ├── Auth/
│   ├── Bags/
│   ├── Items/
│   └── Invitations/
├── Hubs/
│   └── BagHub.cs
├── Models/
│   ├── User.cs
│   ├── BagOfHolding.cs
│   ├── BagAccess.cs
│   ├── Invitation.cs
│   ├── Item.cs
│   ├── BagItem.cs
│   ├── ItemHistory.cs
│   └── Enums/
├── Services/
│   ├── AuthService.cs
│   ├── JwtService.cs
│   ├── BagService.cs
│   ├── ItemService.cs
│   ├── BagItemService.cs
│   └── InvitationService.cs
└── Program.cs
```

---

## Notes

- Start with Phase 1-3 to get auth working first
- Test each phase before moving on
- SignalR can be added last since core CRUD works without it
- Consider using `DateTimeOffset` instead of `DateTime` for timestamps
