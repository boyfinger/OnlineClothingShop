USE [master]
GO
/****** Object:  Database [ClothingShop_PRN222_G2]    Script Date: 2/25/2025 4:49:48 PM ******/
DROP DATABASE IF EXISTS ClothingShop_PRN222_G2;
CREATE DATABASE [ClothingShop_PRN222_G2]
USE [ClothingShop_PRN222_G2]
GO
/****** Object:  Table [dbo].[category]    Script Date: 2/25/2025 4:49:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[category](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[feedback]    Script Date: 2/25/2025 4:49:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[feedback](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [uniqueidentifier] NOT NULL,
	[product_id] [bigint] NULL,
	[order_id] [uniqueidentifier] NOT NULL,
	[rating] [tinyint] NULL,
	[comment] [nvarchar](511) NULL,
	[create_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[image]    Script Date: 2/25/2025 4:49:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[image](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[product_id] [bigint] NULL,
	[url] [varchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[order]    Script Date: 2/25/2025 4:49:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[order](
	[id] [uniqueidentifier] NOT NULL,
	[customer_id] [uniqueidentifier] NULL,
	[voucher_id] [bigint] NULL,
	[full_name] [nvarchar](127) NULL,
	[phone_number] [nvarchar](25) NULL,
	[address] [nvarchar](255) NULL,
	[note] [nvarchar](511) NULL,
	[order_date] [datetime] NULL,
	[status] [int] NULL,
	[total_amount] [int] NULL,
	[create_at] [datetime] NULL,
	[update_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[order_details]    Script Date: 2/25/2025 4:49:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[order_details](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[order_id] [uniqueidentifier] NOT NULL,
	[product_id] [bigint] NULL,
	[quantity] [int] NULL,
	[unit_price] [int] NULL,
	[discount] [int] NULL,
	[total_price] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[order_status]    Script Date: 2/25/2025 4:49:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[order_status](
	[id] [int] NOT NULL,
	[name] [varchar](25) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[product]    Script Date: 2/25/2025 4:49:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[product](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[seller_id] [uniqueidentifier] NULL,
	[name] [nvarchar](255) NULL,
	[category_id] [int] NULL,
	[thumbnail_url] [varchar](255) NULL,
	[description] [nvarchar](max) NULL,
	[price] [int] NULL,
	[discount] [int] NULL,
	[quantity] [int] NULL,
	[status] [int] NULL,
	[create_at] [datetime] NULL,
	[update_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[product_status]    Script Date: 2/25/2025 4:49:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[product_status](
	[id] [int] NOT NULL,
	[name] [varchar](25) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[report]    Script Date: 2/25/2025 4:49:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[report](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[user_id] [uniqueidentifier] NOT NULL,
	[product_id] [bigint] NULL,
	[reason] [nvarchar](511) NULL,
	[status] [int] NULL,
	[create_at] [datetime] NULL,
	[update_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[report_status]    Script Date: 2/25/2025 4:49:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[report_status](
	[id] [int] NOT NULL,
	[name] [varchar](25) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[roles]    Script Date: 2/25/2025 4:49:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[roles](
	[id] [int] NOT NULL,
	[name] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[user]    Script Date: 2/25/2025 4:49:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[user](
	[id] [uniqueidentifier] NOT NULL,
	[user_name] [nvarchar](64) NULL,
	[email] [varchar](64) NULL,
	[password] [varchar](64) NULL,
	[status] [int] NULL,
	[created_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

-- CART TABLE
CREATE TABLE [dbo].[cart](
	[id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
	[user_id] UNIQUEIDENTIFIER FOREIGN KEY REFERENCES [user](id),
	[total_amount] INT DEFAULT 0,
	[create_at] DATETIME DEFAULT GETDATE(),
	[update_at] DATETIME DEFAULT GETDATE()
)

CREATE TABLE [dbo].[cart_detail](
	[cart_id] UNIQUEIDENTIFIER FOREIGN KEY REFERENCES [cart](id),
	[product_id] BIGINT FOREIGN KEY REFERENCES [product](id),
	PRIMARY KEY ([cart_id], [product_id]),
	[quantity] INT DEFAULT 0,
	[total_price] INT DEFAULT 0,
	[create_at] DATETIME DEFAULT GETDATE(),
	[update_at] DATETIME DEFAULT GETDATE()
)

/****** Object:  Table [dbo].[user_gender]    Script Date: 2/25/2025 4:49:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[user_gender](
	[id] [int] NOT NULL,
	[name] [varchar](25) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[user_roles]    Script Date: 2/25/2025 4:49:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[user_roles](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [uniqueidentifier] NULL,
	[role_id] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[user_status]    Script Date: 2/25/2025 4:49:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[user_status](
	[id] [int] NOT NULL,
	[status_name] [varchar](15) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[user_voucher]    Script Date: 2/25/2025 4:49:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[user_voucher](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[user_id] [uniqueidentifier] NOT NULL,
	[voucher_id] [bigint] NULL,
	[quantity] [int] NULL,
	[start_date] [datetime] NULL,
	[end_date] [datetime] NULL,
	[status] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[userinfo]    Script Date: 2/25/2025 4:49:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[userinfo](
	[id] [uniqueidentifier] NOT NULL,
	[full_name] [nvarchar](127) NULL,
	[phone_number] [varchar](25) NULL,
	[avatar_url] [varchar](255) NULL,
	[gender] [int] NULL,
	[date_of_birth] [date] NULL,
	[address] [varchar](255) NULL,
	[update_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[voucher]    Script Date: 2/25/2025 4:49:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[voucher](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[code] [varchar](50) NULL,
	[type] [int] NULL,
	[value] [decimal](10, 2) NULL,
	[description] [nvarchar](255) NULL,
	[start_date] [datetime] NULL,
	[end_date] [datetime] NULL,
	[status] [int] NULL,
	[create_at] [datetime] NULL,
	[update_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[voucher_status]    Script Date: 2/25/2025 4:49:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[voucher_status](
	[id] [int] NOT NULL,
	[name] [varchar](25) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[voucher_type]    Script Date: 2/25/2025 4:49:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[voucher_type](
	[id] [int] NOT NULL,
	[name] [varchar](25) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[wishlist]    Script Date: 2/25/2025 4:49:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[wishlist](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[user_id] [uniqueidentifier] NOT NULL,
	[product_id] [bigint] NOT NULL,
	[is_deleted] [tinyint] NULL,
	[create_at] [datetime] NULL,
	[update_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[category] ON 

INSERT [dbo].[category] ([id], [name]) VALUES (1, N'Quần')
INSERT [dbo].[category] ([id], [name]) VALUES (2, N'Áo')
INSERT [dbo].[category] ([id], [name]) VALUES (3, N'Giày dép')
INSERT [dbo].[category] ([id], [name]) VALUES (4, N'Phụ kiện')
INSERT [dbo].[category] ([id], [name]) VALUES (5, N'Túi xách')
SET IDENTITY_INSERT [dbo].[category] OFF
GO
SET IDENTITY_INSERT [dbo].[feedback] ON 

INSERT [dbo].[feedback] ([id], [user_id], [product_id], [order_id], [rating], [comment], [create_at]) VALUES (1, N'680fefa1-9463-40a3-9d48-594529ab96cf', 1, N'680fefa1-9463-40a3-9d48-594529ab16cf', 5, N'Sản phẩm chất lượng, rất đáng tiền!', CAST(N'2025-02-26T10:00:00.000' AS DateTime))
INSERT [dbo].[feedback] ([id], [user_id], [product_id], [order_id], [rating], [comment], [create_at]) VALUES (2, N'680fefa1-9463-40a3-9d48-594529ab96cf', 5, N'680fefa1-9463-40a3-9d48-594529ab16cf', 4, N'Giao hàng nhanh nhưng đóng gói hơi sơ sài.', CAST(N'2025-02-26T10:05:00.000' AS DateTime))
INSERT [dbo].[feedback] ([id], [user_id], [product_id], [order_id], [rating], [comment], [create_at]) VALUES (3, N'dde923de-6b2a-4104-a293-6da7aaa68ef3', 2, N'680fefa1-9463-40a3-9d48-594529ab26cf', 5, N'Quần jeans rất đẹp, vừa vặn.', CAST(N'2025-02-26T11:00:00.000' AS DateTime))
INSERT [dbo].[feedback] ([id], [user_id], [product_id], [order_id], [rating], [comment], [create_at]) VALUES (4, N'dde923de-6b2a-4104-a293-6da7aaa68ef3', 4, N'680fefa1-9463-40a3-9d48-594529ab26cf', 3, N'Sản phẩm đẹp nhưng giao hàng hơi chậm.', CAST(N'2025-02-26T11:15:00.000' AS DateTime))
INSERT [dbo].[feedback] ([id], [user_id], [product_id], [order_id], [rating], [comment], [create_at]) VALUES (5, N'042d839d-8c72-49ba-8d6b-95b1d5a026a9', 3, N'680fefa1-9463-40a3-9d48-594529ab36cf', 4, N'Áo sơ mi vải mịn, mặc rất thích.', CAST(N'2025-02-26T12:30:00.000' AS DateTime))
INSERT [dbo].[feedback] ([id], [user_id], [product_id], [order_id], [rating], [comment], [create_at]) VALUES (6, N'1d809d9a-4780-48b4-8d2e-a1f3aad9faf6', 6, N'680fefa1-9463-40a3-9d48-594529ab46cf', 5, N'Túi xách rất đẹp, phù hợp với giá tiền.', CAST(N'2025-02-26T13:45:00.000' AS DateTime))
INSERT [dbo].[feedback] ([id], [user_id], [product_id], [order_id], [rating], [comment], [create_at]) VALUES (7, N'1d809d9a-4780-48b4-8d2e-a1f3aad9faf6', 10, N'680fefa1-9463-40a3-9d48-594529ab46cf', 4, N'Đồng hồ khá đẹp nhưng dây hơi cứng.', CAST(N'2025-02-26T14:00:00.000' AS DateTime))
SET IDENTITY_INSERT [dbo].[feedback] OFF
GO
SET IDENTITY_INSERT [dbo].[image] ON 

INSERT [dbo].[image] ([id], [product_id], [url]) VALUES (1, 1, N'https://4menshop.com/cache/image/300x400/images/thumbs/2025/02/ao-thun-co-tron-in-chu-dream-form-regular-at163_small-19113.jpg')
INSERT [dbo].[image] ([id], [product_id], [url]) VALUES (2, 1, N'https://4menshop.com/images/thumbs/2025/02/ao-thun-co-tron-in-chu-eclectic-prep-form-regular-at164-19114-slide-products-67ab1908933e6.jpg')
INSERT [dbo].[image] ([id], [product_id], [url]) VALUES (3, 1, N'https://4menshop.com/images/thumbs/2025/02/ao-thun-co-tron-in-chu-eclectic-prep-form-regular-at164-19114-slide-products-67ab1908d58d1.jpg')
SET IDENTITY_INSERT [dbo].[image] OFF
GO

INSERT [dbo].[order] ([id], [customer_id], [voucher_id], [full_name], [phone_number], [address], [note], [order_date], [status], [total_amount], [create_at], [update_at]) VALUES (N'680fefa1-9463-40a3-9d48-594529ab16cf', N'680fefa1-9463-40a3-9d48-594529ab96cf', NULL, N'Đặng Văn E', N'0987654321', N'Hà Nội', N'', CAST(N'2025-02-25T16:31:25.130' AS DateTime), 4, 950000, CAST(N'2025-02-25T16:31:25.130' AS DateTime), NULL)
INSERT [dbo].[order] ([id], [customer_id], [voucher_id], [full_name], [phone_number], [address], [note], [order_date], [status], [total_amount], [create_at], [update_at]) VALUES (N'680fefa1-9463-40a3-9d48-594529ab26cf', N'dde923de-6b2a-4104-a293-6da7aaa68ef3', NULL, N'Trần Thị B', N'0977654321', N'TP.HCM', N'', CAST(N'2025-02-25T16:31:25.130' AS DateTime), 4, 1260000, CAST(N'2025-02-25T16:31:25.130' AS DateTime), NULL)
INSERT [dbo].[order] ([id], [customer_id], [voucher_id], [full_name], [phone_number], [address], [note], [order_date], [status], [total_amount], [create_at], [update_at]) VALUES (N'680fefa1-9463-40a3-9d48-594529ab36cf', N'042d839d-8c72-49ba-8d6b-95b1d5a026a9', NULL, N'Phạm Văn C', N'0967654321', N'Đà Nẵng', N'', CAST(N'2025-02-25T16:31:25.130' AS DateTime), 4, 425000, CAST(N'2025-02-25T16:31:25.130' AS DateTime), NULL)
INSERT [dbo].[order] ([id], [customer_id], [voucher_id], [full_name], [phone_number], [address], [note], [order_date], [status], [total_amount], [create_at], [update_at]) VALUES (N'680fefa1-9463-40a3-9d48-594529ab46cf', N'1d809d9a-4780-48b4-8d2e-a1f3aad9faf6', NULL, N'Nguyễn Văn A', N'0957654321', N'Cần Thơ', N'', CAST(N'2025-02-25T16:31:25.130' AS DateTime), 4, 1738000, CAST(N'2025-02-25T16:31:25.130' AS DateTime), NULL)
INSERT [dbo].[order] ([id], [customer_id], [voucher_id], [full_name], [phone_number], [address], [note], [order_date], [status], [total_amount], [create_at], [update_at]) VALUES (N'680fefa1-9463-40a3-9d48-594529ab56cf', N'5400c558-7741-4b47-93c6-f5c23add1559', NULL, N'Trần Tuấn Anh', N'0344191620', N'Hà Nội', N'', GETDATE(), 4, 1738000,  GETDATE(), NULL)
GO
SET IDENTITY_INSERT [dbo].[order_details] ON 

INSERT [dbo].[order_details] ([id], [order_id], [product_id], [quantity], [unit_price], [discount], [total_price]) VALUES (1, N'680fefa1-9463-40a3-9d48-594529ab16cf', 1, 1, 250000, 0, 250000)
INSERT [dbo].[order_details] ([id], [order_id], [product_id], [quantity], [unit_price], [discount], [total_price]) VALUES (2, N'680fefa1-9463-40a3-9d48-594529ab16cf', 5, 1, 700000, 0, 700000)
INSERT [dbo].[order_details] ([id], [order_id], [product_id], [quantity], [unit_price], [discount], [total_price]) VALUES (3, N'680fefa1-9463-40a3-9d48-594529ab26cf', 4, 1, 450000, 20, 360000)
INSERT [dbo].[order_details] ([id], [order_id], [product_id], [quantity], [unit_price], [discount], [total_price]) VALUES (4, N'680fefa1-9463-40a3-9d48-594529ab26cf', 6, 1, 900000, 0, 900000)
INSERT [dbo].[order_details] ([id], [order_id], [product_id], [quantity], [unit_price], [discount], [total_price]) VALUES (5, N'680fefa1-9463-40a3-9d48-594529ab36cf', 2, 1, 500000, 15, 425000)
INSERT [dbo].[order_details] ([id], [order_id], [product_id], [quantity], [unit_price], [discount], [total_price]) VALUES (6, N'680fefa1-9463-40a3-9d48-594529ab46cf', 7, 1, 400000, 10, 360000)
INSERT [dbo].[order_details] ([id], [order_id], [product_id], [quantity], [unit_price], [discount], [total_price]) VALUES (7, N'680fefa1-9463-40a3-9d48-594529ab46cf', 9, 1, 350000, 8, 322000)
INSERT [dbo].[order_details] ([id], [order_id], [product_id], [quantity], [unit_price], [discount], [total_price]) VALUES (8, N'680fefa1-9463-40a3-9d48-594529ab46cf', 10, 1, 1200000, 12, 1056000)
INSERT [dbo].[order_details] ([id], [order_id], [product_id], [quantity], [unit_price], [discount], [total_price]) VALUES (9, N'680fefa1-9463-40a3-9d48-594529ab56cf', 7, 1, 400000, 10, 360000)
INSERT [dbo].[order_details] ([id], [order_id], [product_id], [quantity], [unit_price], [discount], [total_price]) VALUES (10, N'680fefa1-9463-40a3-9d48-594529ab56cf', 9, 1, 350000, 8, 322000)
INSERT [dbo].[order_details] ([id], [order_id], [product_id], [quantity], [unit_price], [discount], [total_price]) VALUES (11, N'680fefa1-9463-40a3-9d48-594529ab56cf', 10, 1, 1200000, 12, 1056000)
SET IDENTITY_INSERT [dbo].[order_details] OFF
GO
INSERT [dbo].[order_status] ([id], [name]) VALUES (1, N'PENDING CONFIRMATION')
INSERT [dbo].[order_status] ([id], [name]) VALUES (2, N'CONFIRMED')
INSERT [dbo].[order_status] ([id], [name]) VALUES (3, N'SHIPPING')
INSERT [dbo].[order_status] ([id], [name]) VALUES (4, N'DELIVERED')
INSERT [dbo].[order_status] ([id], [name]) VALUES (5, N'CANCELLED')
INSERT [dbo].[order_status] ([id], [name]) VALUES (6, N'RETURNED')
GO
SET IDENTITY_INSERT [dbo].[product] ON 

INSERT [dbo].[product] ([id], [seller_id], [name], [category_id], [thumbnail_url], [description], [price], [discount], [quantity], [status], [create_at], [update_at]) VALUES (1, N'1d809d9a-4780-48b4-8d2e-a1f3aad9faf6', N'Áo Thun Nam GENTO', 2, N'https://4menshop.com/cache/image/300x400/images/thumbs/2025/02/ao-thun-co-tron-in-chu-dream-form-regular-at163_small-19113.jpg', N'Áo thun nam cao cấp', 250000, 0, 100, 1, CAST(N'2025-02-19T17:01:27.673' AS DateTime), NULL)
INSERT [dbo].[product] ([id], [seller_id], [name], [category_id], [thumbnail_url], [description], [price], [discount], [quantity], [status], [create_at], [update_at]) VALUES (2, N'dde923de-6b2a-4104-a293-6da7aaa68ef3', N'Quần Jeans Nam Devis', 1, N'https://cdn.boo.vn/media/catalog/product/1/_/1.2.21.2.23.001.124.01.60600034_1__4.jpg', N'Quần jeans nam trẻ trung', 500000, 15, 50, 1, CAST(N'2025-02-19T17:01:27.673' AS DateTime), NULL)
INSERT [dbo].[product] ([id], [seller_id], [name], [category_id], [thumbnail_url], [description], [price], [discount], [quantity], [status], [create_at], [update_at]) VALUES (3, N'1d809d9a-4780-48b4-8d2e-a1f3aad9faf6', N'Áo Sơ Mi Nữ GENTO', 2, N'https://cdn.kkfashion.vn/25220-large_default/ao-so-mi-nu-cong-so-mau-trang-tay-dai-asm15-12.jpg', N'Áo sơ mi nữ thanh lịch', 300000, 0, 70, 1, CAST(N'2025-02-19T17:01:27.673' AS DateTime), NULL)
INSERT [dbo].[product] ([id], [seller_id], [name], [category_id], [thumbnail_url], [description], [price], [discount], [quantity], [status], [create_at], [update_at]) VALUES (4, N'dde923de-6b2a-4104-a293-6da7aaa68ef3', N'Váy Đầm Nữ GENTO', 1, N'https://pos.nvncdn.com/80a557-93682/ps/20230710_8xEuqh8NRG.png', N'Váy đầm nữ thời trang', 450000, 20, 30, 1, CAST(N'2025-02-19T17:01:27.673' AS DateTime), NULL)
INSERT [dbo].[product] ([id], [seller_id], [name], [category_id], [thumbnail_url], [description], [price], [discount], [quantity], [status], [create_at], [update_at]) VALUES (5, N'1d809d9a-4780-48b4-8d2e-a1f3aad9faf6', N'Giày Sneaker NIKE', 3, N'https://myshoes.vn/image/cache/catalog/2025/nike/giay-nike-pegasus-41-nu-phantom-01-500x500.jpg', N'Giày sneaker unisex', 700000, 0, 40, 1, CAST(N'2025-02-19T17:01:27.673' AS DateTime), NULL)
INSERT [dbo].[product] ([id], [seller_id], [name], [category_id], [thumbnail_url], [description], [price], [discount], [quantity], [status], [create_at], [update_at]) VALUES (6, N'dde923de-6b2a-4104-a293-6da7aaa68ef3', N'Túi Xách Nữ GENTO', 5, N'https://www.vascara.com/uploads/cms_productmedia/2025/January/23/tui-xach-tay-quai-doi-van-da-phoi-tuong-phan---tot-0193---mau-be__80307__1737606720-medium.jpg', N'Túi xách nữ hàng hiệu', 900000, 0, 20, 1, CAST(N'2025-02-19T17:01:27.673' AS DateTime), NULL)
INSERT [dbo].[product] ([id], [seller_id], [name], [category_id], [thumbnail_url], [description], [price], [discount], [quantity], [status], [create_at], [update_at]) VALUES (7, N'1d809d9a-4780-48b4-8d2e-a1f3aad9faf6', N'Áo Hoodie Nam Devis', 2, N'https://product.hstatic.net/1000360022/product/id-005769a_9d320dcfce3f4e5bb993a1ac13304c4f_master.jpg', N'Áo hoodie ấm áp', 400000, 10, 60, 1, CAST(N'2025-02-19T17:01:27.673' AS DateTime), NULL)
INSERT [dbo].[product] ([id], [seller_id], [name], [category_id], [thumbnail_url], [description], [price], [discount], [quantity], [status], [create_at], [update_at]) VALUES (8, N'dde923de-6b2a-4104-a293-6da7aaa68ef3', N'Mũ Lưỡi Trai Devis', 4, N'https://bizweb.dktcdn.net/100/287/440/products/mu-luoi-trai-local-brand-dep-mau-be-1.jpg?v=1644822065327', N'Mũ lưỡi trai phong cách', 150000, 5, 90, 1, CAST(N'2025-02-19T17:01:27.673' AS DateTime), NULL)
INSERT [dbo].[product] ([id], [seller_id], [name], [category_id], [thumbnail_url], [description], [price], [discount], [quantity], [status], [create_at], [update_at]) VALUES (9, N'dde923de-6b2a-4104-a293-6da7aaa68ef3', N'Kính Râm VSP303', 4, N'https://salt.tikicdn.com/cache/280x280/ts/product/9a/7c/6f/9edffc4f2ccd5be435fd2a0a784eeaa8.JPG', N'Kính râm chống UV', 350000, 8, 35, 1, CAST(N'2025-02-19T17:01:27.673' AS DateTime), NULL)
INSERT [dbo].[product] ([id], [seller_id], [name], [category_id], [thumbnail_url], [description], [price], [discount], [quantity], [status], [create_at], [update_at]) VALUES (10, N'1d809d9a-4780-48b4-8d2e-a1f3aad9faf6', N'Đồng Hồ Nam Citizen AN8201-57L', 4, N'https://donghoduyanh.com/images/products/2023/07/31/large/an8201-57l_1690775073.jpg', N'Đồng hồ nam thời trang', 1200000, 12, 25, 1, CAST(N'2025-02-19T17:01:27.673' AS DateTime), NULL)
SET IDENTITY_INSERT [dbo].[product] OFF
GO
INSERT [dbo].[product_status] ([id], [name]) VALUES (1, N'APPROVED')
INSERT [dbo].[product_status] ([id], [name]) VALUES (2, N'UNAPPROVED')
INSERT [dbo].[product_status] ([id], [name]) VALUES (3, N'REJECTED')

GO
INSERT [dbo].[report_status] ([id], [name]) VALUES (1, N'PENDING')
INSERT [dbo].[report_status] ([id], [name]) VALUES (2, N'IN_PROGRESS')
INSERT [dbo].[report_status] ([id], [name]) VALUES (3, N'RESOLVED')
INSERT [dbo].[report_status] ([id], [name]) VALUES (4, N'REJECTED')
GO
INSERT [dbo].[roles] ([id], [name]) VALUES (1, N'ADMIN')
INSERT [dbo].[roles] ([id], [name]) VALUES (2, N'SELLER')
INSERT [dbo].[roles] ([id], [name]) VALUES (3, N'CUSTOMER')
GO
INSERT [dbo].[user] ([id], [user_name], [email], [password], [status], [created_at]) VALUES (N'680fefa1-9463-40a3-9d48-594529ab96cf', N'dangvan_e', N'dangvane@example.com', N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 1, CAST(N'2025-02-19T16:32:47.613' AS DateTime))
INSERT [dbo].[user] ([id], [user_name], [email], [password], [status], [created_at]) VALUES (N'dde923de-6b2a-4104-a293-6da7aaa68ef3', N'tranthib', N'tranthib@example.com', N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 1, CAST(N'2025-02-19T16:32:47.613' AS DateTime))
INSERT [dbo].[user] ([id], [user_name], [email], [password], [status], [created_at]) VALUES (N'042d839d-8c72-49ba-8d6b-95b1d5a026a9', N'phamvan_c', N'phamvanc@example.com', N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 1, CAST(N'2025-02-19T16:32:47.613' AS DateTime))
INSERT [dbo].[user] ([id], [user_name], [email], [password], [status], [created_at]) VALUES (N'1d809d9a-4780-48b4-8d2e-a1f3aad9faf6', N'nguyenvan_a', N'nguyenvana@example.com', N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 1, CAST(N'2025-02-19T16:32:47.613' AS DateTime))
INSERT [dbo].[user] ([id], [user_name], [email], [password], [status], [created_at]) VALUES (N'6fc34f42-46e4-4355-8dad-b17da99885ae', N'admin', N'admin@example.com', N'240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 1, CAST(N'2025-02-19T16:32:47.613' AS DateTime))
INSERT [dbo].[user] ([id], [user_name], [email], [password], [status], [created_at]) VALUES (N'5400c558-7741-4b47-93c6-f5c23add1558', N'hoangthid', N'hoangthid@example.com', N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 1, CAST(N'2025-02-19T16:32:47.613' AS DateTime))
INSERT [dbo].[user] ([id], [user_name], [email], [password], [status], [created_at]) VALUES (N'5400c558-7741-4b47-93c6-f5c23add1559', N'tuananh', N'tuananh@gmail.com', N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 1, CAST(N'2025-02-19T16:32:47.613' AS DateTime))
GO

-- tuananh
INSERT [dbo].[cart] ([user_id]) VALUES ('5400c558-7741-4b47-93c6-f5c23add1559')
GO

INSERT [dbo].[userinfo] ([id]) VALUES ('5400c558-7741-4b47-93c6-f5c23add1559')
GO
----- het

INSERT [dbo].[user_gender] ([id], [name]) VALUES (1, N'MALE')
INSERT [dbo].[user_gender] ([id], [name]) VALUES (2, N'FEMALE')
INSERT [dbo].[user_gender] ([id], [name]) VALUES (3, N'OTHER')
GO
SET IDENTITY_INSERT [dbo].[user_roles] ON 

INSERT [dbo].[user_roles] ([id], [user_id], [role_id]) VALUES (1, N'6fc34f42-46e4-4355-8dad-b17da99885ae', 1)
INSERT [dbo].[user_roles] ([id], [user_id], [role_id]) VALUES (2, N'1d809d9a-4780-48b4-8d2e-a1f3aad9faf6', 2)
INSERT [dbo].[user_roles] ([id], [user_id], [role_id]) VALUES (3, N'dde923de-6b2a-4104-a293-6da7aaa68ef3', 2)
INSERT [dbo].[user_roles] ([id], [user_id], [role_id]) VALUES (4, N'042d839d-8c72-49ba-8d6b-95b1d5a026a9', 3)
INSERT [dbo].[user_roles] ([id], [user_id], [role_id]) VALUES (5, N'680fefa1-9463-40a3-9d48-594529ab96cf', 3)
INSERT [dbo].[user_roles] ([id], [user_id], [role_id]) VALUES (6, N'5400c558-7741-4b47-93c6-f5c23add1559', 3)
SET IDENTITY_INSERT [dbo].[user_roles] OFF
GO
INSERT [dbo].[user_status] ([id], [status_name]) VALUES (1, N'ACTIVE')
INSERT [dbo].[user_status] ([id], [status_name]) VALUES (2, N'INACTIVE')
INSERT [dbo].[user_status] ([id], [status_name]) VALUES (3, N'BANNED')
GO
INSERT [dbo].[voucher_status] ([id], [name]) VALUES (1, N'ACTIVE')
INSERT [dbo].[voucher_status] ([id], [name]) VALUES (2, N'EXPIRED')
INSERT [dbo].[voucher_status] ([id], [name]) VALUES (3, N'USED')
INSERT [dbo].[voucher_status] ([id], [name]) VALUES (4, N'DISABLED')
INSERT [dbo].[voucher_status] ([id], [name]) VALUES (5, N'PENDING')
GO
INSERT [dbo].[voucher_type] ([id], [name]) VALUES (1, N'DISCOUNT PERCENTAGE')
INSERT [dbo].[voucher_type] ([id], [name]) VALUES (2, N'DISCOUNT AMOUNT')
INSERT [dbo].[voucher_type] ([id], [name]) VALUES (3, N'FREE SHIPPING')
GO
ALTER TABLE [dbo].[order] ADD  DEFAULT (NULL) FOR [full_name]
GO
ALTER TABLE [dbo].[order] ADD  DEFAULT (NULL) FOR [phone_number]
GO
ALTER TABLE [dbo].[roles] ADD  DEFAULT (NULL) FOR [name]
GO
ALTER TABLE [dbo].[user] ADD  DEFAULT (newid()) FOR [id]
GO
ALTER TABLE [dbo].[user] ADD  DEFAULT (NULL) FOR [user_name]
GO
ALTER TABLE [dbo].[user] ADD  DEFAULT (NULL) FOR [password]
GO
ALTER TABLE [dbo].[user] ADD  DEFAULT (NULL) FOR [status]
GO
ALTER TABLE [dbo].[user] ADD  DEFAULT (NULL) FOR [created_at]
GO
ALTER TABLE [dbo].[user_gender] ADD  DEFAULT (NULL) FOR [name]
GO
ALTER TABLE [dbo].[user_roles] ADD  DEFAULT (NULL) FOR [user_id]
GO
ALTER TABLE [dbo].[user_roles] ADD  DEFAULT (NULL) FOR [role_id]
GO
ALTER TABLE [dbo].[user_status] ADD  DEFAULT (NULL) FOR [status_name]
GO
ALTER TABLE [dbo].[userinfo] ADD  DEFAULT (NULL) FOR [full_name]
GO
ALTER TABLE [dbo].[userinfo] ADD  DEFAULT (NULL) FOR [phone_number]
GO
ALTER TABLE [dbo].[userinfo] ADD  DEFAULT (NULL) FOR [avatar_url]
GO
ALTER TABLE [dbo].[userinfo] ADD  DEFAULT (NULL) FOR [gender]
GO
ALTER TABLE [dbo].[userinfo] ADD  DEFAULT (NULL) FOR [date_of_birth]
GO
ALTER TABLE [dbo].[feedback]  WITH CHECK ADD FOREIGN KEY([order_id])
REFERENCES [dbo].[order] ([id])
GO
ALTER TABLE [dbo].[feedback]  WITH CHECK ADD FOREIGN KEY([product_id])
REFERENCES [dbo].[product] ([id])
GO
ALTER TABLE [dbo].[feedback]  WITH CHECK ADD FOREIGN KEY([user_id])
REFERENCES [dbo].[user] ([id])
GO
ALTER TABLE [dbo].[image]  WITH CHECK ADD FOREIGN KEY([product_id])
REFERENCES [dbo].[product] ([id])
GO
ALTER TABLE [dbo].[order]  WITH CHECK ADD FOREIGN KEY([customer_id])
REFERENCES [dbo].[user] ([id])
GO
ALTER TABLE [dbo].[order]  WITH CHECK ADD FOREIGN KEY([status])
REFERENCES [dbo].[order_status] ([id])
GO
ALTER TABLE [dbo].[order]  WITH CHECK ADD FOREIGN KEY([voucher_id])
REFERENCES [dbo].[voucher] ([id])
GO
ALTER TABLE [dbo].[order_details]  WITH CHECK ADD FOREIGN KEY([order_id])
REFERENCES [dbo].[order] ([id])
GO
ALTER TABLE [dbo].[order_details]  WITH CHECK ADD FOREIGN KEY([product_id])
REFERENCES [dbo].[product] ([id])
GO
ALTER TABLE [dbo].[product]  WITH CHECK ADD FOREIGN KEY([category_id])
REFERENCES [dbo].[category] ([id])
GO
ALTER TABLE [dbo].[product]  WITH CHECK ADD FOREIGN KEY([seller_id])
REFERENCES [dbo].[user] ([id])
GO
ALTER TABLE [dbo].[product]  WITH CHECK ADD FOREIGN KEY([status])
REFERENCES [dbo].[product_status] ([id])
GO
ALTER TABLE [dbo].[report]  WITH CHECK ADD FOREIGN KEY([product_id])
REFERENCES [dbo].[product] ([id])
GO
ALTER TABLE [dbo].[report]  WITH CHECK ADD FOREIGN KEY([status])
REFERENCES [dbo].[report_status] ([id])
GO
ALTER TABLE [dbo].[report]  WITH CHECK ADD FOREIGN KEY([user_id])
REFERENCES [dbo].[user] ([id])
GO
ALTER TABLE [dbo].[user]  WITH CHECK ADD FOREIGN KEY([status])
REFERENCES [dbo].[user_status] ([id])
GO
ALTER TABLE [dbo].[user_roles]  WITH CHECK ADD FOREIGN KEY([role_id])
REFERENCES [dbo].[roles] ([id])
GO
ALTER TABLE [dbo].[user_roles]  WITH CHECK ADD FOREIGN KEY([user_id])
REFERENCES [dbo].[user] ([id])
GO
ALTER TABLE [dbo].[user_voucher]  WITH CHECK ADD FOREIGN KEY([user_id])
REFERENCES [dbo].[user] ([id])
GO
ALTER TABLE [dbo].[user_voucher]  WITH CHECK ADD FOREIGN KEY([voucher_id])
REFERENCES [dbo].[voucher] ([id])
GO
ALTER TABLE [dbo].[userinfo]  WITH CHECK ADD FOREIGN KEY([gender])
REFERENCES [dbo].[user_gender] ([id])
GO
ALTER TABLE [dbo].[userinfo]  WITH CHECK ADD FOREIGN KEY([id])
REFERENCES [dbo].[user] ([id])
GO
ALTER TABLE [dbo].[voucher]  WITH CHECK ADD FOREIGN KEY([status])
REFERENCES [dbo].[voucher_status] ([id])
GO
ALTER TABLE [dbo].[voucher]  WITH CHECK ADD FOREIGN KEY([type])
REFERENCES [dbo].[voucher_type] ([id])
GO
ALTER TABLE [dbo].[wishlist]  WITH CHECK ADD FOREIGN KEY([product_id])
REFERENCES [dbo].[product] ([id])
GO
ALTER TABLE [dbo].[wishlist]  WITH CHECK ADD FOREIGN KEY([user_id])
REFERENCES [dbo].[user] ([id])
GO
USE [master]
GO
ALTER DATABASE [ClothingShop_PRN222_G2] SET  READ_WRITE 
GO
