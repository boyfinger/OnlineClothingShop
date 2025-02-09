USE master
DROP DATABASE IF EXISTS ClothingShop_PRN222_G2;

CREATE DATABASE ClothingShop_PRN222_G2;
USE ClothingShop_PRN222_G2;

CREATE TABLE [user] (
  [id] bigint PRIMARY KEY NOT NULL,
  [user_name] nvarchar(64) DEFAULT (null),
  [email] varchar(64),
  [password] varchar(64) DEFAULT (null),
  [status] int DEFAULT (null),
  [created_at] datetime DEFAULT (null)
)
GO

CREATE TABLE [user_info] (
  [id] bigint PRIMARY KEY NOT NULL,
  [full_name] nvarchar(127) DEFAULT (null),
  [phone_number] varchar(25) DEFAULT (null),
  [avatar_url] varchar(255) DEFAULT (null),
  [gender] int DEFAULT (null),
  [date_of_birth] date DEFAULT (null),
  [address] varchar(255),
  [update_at] datetime
)
GO

CREATE TABLE [user_gender] (
  [id] int PRIMARY KEY NOT NULL,
  [name] varchar(25) DEFAULT (null)
)
GO

CREATE TABLE [user_status] (
  [id] int PRIMARY KEY NOT NULL,
  [status_name] varchar(15) DEFAULT (null)
)
GO

CREATE TABLE [roles] (
  [id] int PRIMARY KEY NOT NULL,
  [name] varchar(50) DEFAULT (null)
)
GO

CREATE TABLE [user_roles] (
  [id] bigint PRIMARY KEY NOT NULL,
  [user_id] bigint DEFAULT (null),
  [role_id] int DEFAULT (null)
)
GO

CREATE TABLE [product] (
  [id] bigint PRIMARY KEY,
  [seller_id] bigint,
  [name] nvarchar(255),
  [category_id] bigint,
  [thumbnail_url] varchar(255),
  [description] text,
  [price] int,
  [discount] int,
  [quantity] int,
  [status] int,
  [create_at] datetime,
  [update_at] datetime
)
GO

CREATE TABLE [image] (
  [id] bigint PRIMARY KEY,
  [product_id] bigint,
  [url] varchar(255)
)
GO

CREATE TABLE [product_status] (
  [id] int PRIMARY KEY,
  [name] varchar(25)
)
GO

CREATE TABLE [category] (
  [id] bigint PRIMARY KEY,
  [name] varchar(255)
)
GO

CREATE TABLE [order] (
  [id] bigint PRIMARY KEY,
  [customer_id] bigint,
  [seller_id] bigint,
  [voucher_id] bigint,
  [full_name] nvarchar(127) DEFAULT (null),
  [phone_number] nvarchar(25) DEFAULT (null),
  [address] nvarchar(255),
  [note] nvarchar(511),
  [order_date] datetime,
  [status] int,
  [total_amount] int,
  [create_at] datetime,
  [update_at] datetime
)
GO

CREATE TABLE [order_details] (
  [id] bigint PRIMARY KEY,
  [order_id] bigint,
  [product_id] bigint,
  [quantity] int,
  [unit_price] int,
  [discount] int,
  [total_price] int
)
GO

CREATE TABLE [order_status] (
  [id] int PRIMARY KEY,
  [name] varchar(25)
)
GO

CREATE TABLE [voucher] (
  [id] bigint PRIMARY KEY,
  [code] varchar(50) UNIQUE,
  [type] int,
  [value] DECIMAL(10,2),
  [quantity] int,
  [description] nvarchar(255),
  [start_date] datetime,
  [end_date] datetime,
  [status] int,
  [create_at] datetime,
  [update_at] datetime
)
GO

CREATE TABLE [voucher_type] (
  [id] int PRIMARY KEY,
  [name] varchar(25)
)
GO

CREATE TABLE [voucher_status] (
  [id] int PRIMARY KEY,
  [name] varchar(25)
)
GO

CREATE TABLE [feedback] (
  [id] bigint PRIMARY KEY,
  [user_id] bigint NOT NULL,
  [product_id] bigint,
  [order_id] bigint,
  [rating] tinyint,
  [commnet] nvarchar(511),
  [create_at] datetime
)
GO

CREATE TABLE [report] (
  [id] bigint PRIMARY KEY,
  [user_id] bigint NOT NULL,
  [product_id] bigint,
  [reason] nvarchar(511),
  [status] int,
  [create_at] datetime,
  [update_at] datetime
)
GO

CREATE TABLE [report_status] (
  [id] int PRIMARY KEY,
  [name] varchar(25)
)
GO

CREATE TABLE [wishlist] (
  [id] int PRIMARY KEY,
  [user_id] bigint NOT NULL,
  [product_id] bigint NOT NULL,
  [is_deleted] tinyint,
  [create_at] datetime,
  [update_at] datetime
)
GO

ALTER TABLE [user_info] ADD FOREIGN KEY ([id]) REFERENCES [user] ([id])
GO

ALTER TABLE [user_info] ADD FOREIGN KEY ([gender]) REFERENCES [user_gender] ([id])
GO

ALTER TABLE [user_roles] ADD FOREIGN KEY ([user_id]) REFERENCES [user] ([id])
GO

ALTER TABLE [image] ADD FOREIGN KEY ([product_id]) REFERENCES [product] ([id])
GO

ALTER TABLE [product] ADD FOREIGN KEY ([category_id]) REFERENCES [category] ([id])
GO

ALTER TABLE [product] ADD FOREIGN KEY ([seller_id]) REFERENCES [user] ([id])
GO

ALTER TABLE [order_details] ADD FOREIGN KEY ([product_id]) REFERENCES [product] ([id])
GO

ALTER TABLE [order_details] ADD FOREIGN KEY ([order_id]) REFERENCES [order] ([id])
GO

ALTER TABLE [order] ADD FOREIGN KEY ([status]) REFERENCES [order_status] ([id])
GO

ALTER TABLE [product] ADD FOREIGN KEY ([status]) REFERENCES [product_status] ([id])
GO

ALTER TABLE [user] ADD FOREIGN KEY ([status]) REFERENCES [user_status] ([id])
GO

ALTER TABLE [voucher] ADD FOREIGN KEY ([type]) REFERENCES [voucher_type] ([id])
GO

ALTER TABLE [voucher] ADD FOREIGN KEY ([status]) REFERENCES [voucher_status] ([id])
GO

ALTER TABLE [order] ADD FOREIGN KEY ([voucher_id]) REFERENCES [voucher] ([id])
GO

ALTER TABLE [feedback] ADD FOREIGN KEY ([user_id]) REFERENCES [user] ([id])
GO

ALTER TABLE [feedback] ADD FOREIGN KEY ([product_id]) REFERENCES [product] ([id])
GO

ALTER TABLE [feedback] ADD FOREIGN KEY ([order_id]) REFERENCES [order] ([id])
GO

ALTER TABLE [report] ADD FOREIGN KEY ([user_id]) REFERENCES [user] ([id])
GO

ALTER TABLE [report] ADD FOREIGN KEY ([product_id]) REFERENCES [product] ([id])
GO

ALTER TABLE [report] ADD FOREIGN KEY ([status]) REFERENCES [report_status] ([id])
GO

ALTER TABLE [wishlist] ADD FOREIGN KEY ([user_id]) REFERENCES [user] ([id])
GO

ALTER TABLE [wishlist] ADD FOREIGN KEY ([product_id]) REFERENCES [product] ([id])
GO

ALTER TABLE [user_roles] ADD FOREIGN KEY ([role_id]) REFERENCES [roles] ([id])
GO
