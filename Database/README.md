# Database Schema Documentation

## Overview
PostgreSQL database schema for **Thuê Gì v2** - Cosplay costume rental platform.

## Migration Files

### 001_initial_schema.sql
Core tables for basic platform functionality:
- **Users & Auth**: users, roles, user_roles
- **Shops**: shops, categories
- **Catalog**: costumes, costume_images, costume_categories, inventory
- **Bookings**: bookings, booking_items, booking_status_history
- **Payments**: payments
- **Reviews**: reviews
- **Messaging**: conversations, messages
- **Notifications**: notifications
- **Wishlist**: wishlists, wishlist_items

### 002_advanced_features.sql
Advanced features:
- **Identity Verification**: identity_verifications
- **Staff Management**: staff_profiles, staff_services, staff_bookings
- **Blog**: blog_posts, blog_categories, blog_post_categories, blog_comments
- **Community**: community_posts, community_comments, post_likes, comment_likes, user_follows
- **Marketing**: discounts, discount_usage, referrals, gift_cards, gift_card_transactions
- **Admin**: disputes, reports, wallets, wallet_transactions, system_settings

## Key Features

### Data Types
- **Primary Keys**: UUID (uuid_generate_v4())
- **Timestamps**: TIMESTAMP WITH TIME ZONE
- **Soft Delete**: deleted_at column
- **Arrays**: TEXT[] for images, attachments
- **JSON**: JSONB for flexible data

### Enums
- user_status: active, inactive, suspended, banned
- booking_status: pending, confirmed, in_progress, completed, cancelled, refunded
- payment_status: pending, processing, completed, failed, refunded
- payment_method: credit_card, debit_card, bank_transfer, e_wallet, cash
- verification_status: pending, in_review, approved, rejected
- dispute_status: open, in_review, resolved, closed
- notification_type: booking, payment, review, message, system, promotion
- staff_service_type: makeup, photography, both
- post_type: text, image, video, poll
- report_type: spam, inappropriate, harassment, fake, other
- report_status: pending, reviewing, resolved, dismissed

### Constraints
- Email format validation
- Price/amount positive checks
- Rating range (0-5)
- Date range validation
- Quantity validation
- Different users for follows/conversations

### Indexes
Optimized for:
- User lookups (email, status)
- Shop searches (slug, rating, location)
- Costume searches (price, rating, availability, character, series)
- Booking queries (customer, shop, status, dates)
- Payment tracking (booking, user, status)
- Review filtering (costume, shop, rating)
- Message history (conversation, sender, timestamp)
- Notification feeds (user, read status, timestamp)

### Triggers
Auto-update `updated_at` on:
- users, shops, costumes, bookings, payments, reviews
- identity_verifications, staff_profiles, blog_posts
- community_posts, discounts, wallets

## Relationships

### One-to-Many
- users → shops (owner)
- shops → costumes
- costumes → costume_images
- costumes → inventory
- bookings → booking_items
- bookings → booking_status_history
- bookings → payments
- users → reviews (reviewer)
- users → messages (sender)
- users → notifications
- users → wishlists

### Many-to-Many
- users ↔ roles (user_roles)
- costumes ↔ categories (costume_categories)
- blog_posts ↔ blog_categories (blog_post_categories)
- users ↔ users (user_follows)
- community_posts ↔ users (post_likes)
- community_comments ↔ users (comment_likes)

### Self-Referencing
- categories (parent_id)
- blog_comments (parent_id)
- community_comments (parent_id)

## Seed Data

### Roles
- admin: Full system access
- shop_owner: Manage shop and costumes
- customer: Rent costumes
- staff: Makeup artist or photographer

### Categories
- Anime, Game, Movie, Traditional, Fantasy, Historical

### Blog Categories
- Hướng dẫn, Tin tức, Review, Cộng đồng

### System Settings
- platform_commission_rate: 10%
- min_booking_days: 1
- max_booking_days: 30
- cancellation_window_hours: 24
- auto_complete_days: 3

## Running Migrations

```bash
# Connect to PostgreSQL
psql -U postgres -d thuegi_db

# Run migrations in order
\i thuegi-be/Database/Migrations/001_initial_schema.sql
\i thuegi-be/Database/Migrations/002_advanced_features.sql
```

## Connection String Example

```
Host=localhost;Port=5432;Database=thuegi_db;Username=postgres;Password=your_password
```

## Notes

- All tables use UUID primary keys for better distribution and security
- Soft delete implemented via `deleted_at` column
- Timestamps include timezone for global consistency
- Arrays used for flexible multi-value fields (images, attachments)
- JSONB for extensible data without schema changes
- Comprehensive indexes for query performance
- Foreign key constraints with appropriate CASCADE/SET NULL
- Check constraints enforce business rules at DB level
- Triggers auto-maintain `updated_at` timestamps
