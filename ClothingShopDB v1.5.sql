USE [master]
GO
/****** Object:  Database [ClothingShop_PRN222_G2]    Script Date: 3/21/2025 10:08:34 AM ******/
CREATE DATABASE [ClothingShop_PRN222_G2]
GO
USE [ClothingShop_PRN222_G2]
GO
/****** Object:  Table [dbo].[cart]    Script Date: 3/21/2025 10:08:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[cart](
	[id] [uniqueidentifier] NOT NULL,
	[user_id] [uniqueidentifier] NULL,
	[total_amount] [int] NULL,
	[create_at] [datetime] NULL,
	[update_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[cart_detail]    Script Date: 3/21/2025 10:08:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[cart_detail](
	[cart_id] [uniqueidentifier] NOT NULL,
	[product_id] [bigint] NOT NULL,
	[quantity] [int] NULL,
	[total_price] [int] NULL,
	[create_at] [datetime] NULL,
	[update_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[cart_id] ASC,
	[product_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[category]    Script Date: 3/21/2025 10:08:34 AM ******/
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
/****** Object:  Table [dbo].[feedback]    Script Date: 3/21/2025 10:08:34 AM ******/
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
/****** Object:  Table [dbo].[image]    Script Date: 3/21/2025 10:08:34 AM ******/
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
/****** Object:  Table [dbo].[order]    Script Date: 3/21/2025 10:08:34 AM ******/
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
/****** Object:  Table [dbo].[order_details]    Script Date: 3/21/2025 10:08:34 AM ******/
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
	[status] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[order_status]    Script Date: 3/21/2025 10:08:34 AM ******/
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
/****** Object:  Table [dbo].[orderdetail_status]    Script Date: 3/21/2025 10:08:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[orderdetail_status](
	[id] [int] NOT NULL,
	[name] [varchar](25) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[product]    Script Date: 3/21/2025 10:08:34 AM ******/
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
/****** Object:  Table [dbo].[product_status]    Script Date: 3/21/2025 10:08:34 AM ******/
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
/****** Object:  Table [dbo].[report]    Script Date: 3/21/2025 10:08:34 AM ******/
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
/****** Object:  Table [dbo].[report_status]    Script Date: 3/21/2025 10:08:34 AM ******/
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
/****** Object:  Table [dbo].[roles]    Script Date: 3/21/2025 10:08:34 AM ******/
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
/****** Object:  Table [dbo].[user]    Script Date: 3/21/2025 10:08:34 AM ******/
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
/****** Object:  Table [dbo].[user_gender]    Script Date: 3/21/2025 10:08:34 AM ******/
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
/****** Object:  Table [dbo].[user_roles]    Script Date: 3/21/2025 10:08:34 AM ******/
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
/****** Object:  Table [dbo].[user_status]    Script Date: 3/21/2025 10:08:34 AM ******/
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
/****** Object:  Table [dbo].[userinfo]    Script Date: 3/21/2025 10:08:34 AM ******/
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
/****** Object:  Table [dbo].[voucher]    Script Date: 3/21/2025 10:08:34 AM ******/
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
	[usage_limit] [int] NULL,
	[usage_count] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[voucher_status]    Script Date: 3/21/2025 10:08:34 AM ******/
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
/****** Object:  Table [dbo].[voucher_type]    Script Date: 3/21/2025 10:08:34 AM ******/
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
/****** Object:  Table [dbo].[VoucherUsage]    Script Date: 3/21/2025 10:08:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VoucherUsage](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[user_id] [uniqueidentifier] NOT NULL,
	[voucher_id] [bigint] NOT NULL,
	[used_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[wishlist]    Script Date: 3/21/2025 10:08:34 AM ******/
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
INSERT [dbo].[cart] ([id], [user_id], [total_amount], [create_at], [update_at]) VALUES (N'601909a7-3794-4c80-87a4-9e0b69d22e45', N'5400c558-7741-4b47-93c6-f5c23add1559', 0, CAST(N'2025-03-11T10:37:56.060' AS DateTime), CAST(N'2025-03-11T10:37:56.060' AS DateTime))
INSERT [dbo].[cart] ([id], [user_id], [total_amount], [create_at], [update_at]) VALUES (N'b6347503-f2e0-4dc0-89e5-aa3898a7ba1c', N'680fefa1-9463-40a3-9d48-594529ab96cf', 300000, CAST(N'2025-03-12T09:03:50.583' AS DateTime), CAST(N'2025-03-20T15:30:36.237' AS DateTime))
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
INSERT [dbo].[feedback] ([id], [user_id], [product_id], [order_id], [rating], [comment], [create_at]) VALUES (8, N'dde923de-6b2a-4104-a293-6da7aaa68ef3', 1, N'680fefa1-9463-40a3-9d48-594529ab26cf', 5, N'Good!', CAST(N'2025-03-12T11:00:00.000' AS DateTime))
INSERT [dbo].[feedback] ([id], [user_id], [product_id], [order_id], [rating], [comment], [create_at]) VALUES (9, N'1d809d9a-4780-48b4-8d2e-a1f3aad9faf6', 1, N'680fefa1-9463-40a3-9d48-594529ab46cf', 4, N'Sản phẩm rất tốt nhưng giá thành cao!', CAST(N'2025-03-11T13:20:00.000' AS DateTime))
INSERT [dbo].[feedback] ([id], [user_id], [product_id], [order_id], [rating], [comment], [create_at]) VALUES (10, N'042d839d-8c72-49ba-8d6b-95b1d5a026a9', 1, N'680fefa1-9463-40a3-9d48-594529ab36cf', 3, N'Sản phẩm bị lỗi nhẹ', CAST(N'2025-03-11T13:20:00.000' AS DateTime))
INSERT [dbo].[feedback] ([id], [user_id], [product_id], [order_id], [rating], [comment], [create_at]) VALUES (11, N'680fefa1-9463-40a3-9d48-594529ab96cf', 10, N'4397c55c-14ca-46f8-9cb6-7cd0c56ddc00', 5, N'Good', CAST(N'2025-03-14T23:52:54.723' AS DateTime))
INSERT [dbo].[feedback] ([id], [user_id], [product_id], [order_id], [rating], [comment], [create_at]) VALUES (12, N'680fefa1-9463-40a3-9d48-594529ab96cf', 1, N'1174f72a-9024-4cce-97ab-e52973c76604', 1, NULL, CAST(N'2025-03-15T00:33:54.650' AS DateTime))
SET IDENTITY_INSERT [dbo].[feedback] OFF
GO
SET IDENTITY_INSERT [dbo].[image] ON 

INSERT [dbo].[image] ([id], [product_id], [url]) VALUES (1, 1, N'https://4menshop.com/cache/image/300x400/images/thumbs/2025/02/ao-thun-co-tron-in-chu-dream-form-regular-at163_small-19113.jpg')
INSERT [dbo].[image] ([id], [product_id], [url]) VALUES (2, 1, N'https://4menshop.com/images/thumbs/2025/02/ao-thun-co-tron-in-chu-eclectic-prep-form-regular-at164-19114-slide-products-67ab1908933e6.jpg')
INSERT [dbo].[image] ([id], [product_id], [url]) VALUES (3, 1, N'https://4menshop.com/images/thumbs/2025/02/ao-thun-co-tron-in-chu-eclectic-prep-form-regular-at164-19114-slide-products-67ab1908d58d1.jpg')
INSERT [dbo].[image] ([id], [product_id], [url]) VALUES (4, 2, N'https://product.hstatic.net/200000886795/product/quan-jeans-nam-insidemen-cropped-ijn0410z__12__490cc37aed984869ad5c5c09217dbe91.jpg')
INSERT [dbo].[image] ([id], [product_id], [url]) VALUES (5, 2, N'https://product.hstatic.net/200000886795/product/quan-jeans-nam-insidemen-cropped-ijn0410z__11__1e033bd7f5a446f9b6aba9377455e13f.jpg')
INSERT [dbo].[image] ([id], [product_id], [url]) VALUES (6, 2, N'https://product.hstatic.net/200000886795/product/quan-jeans-nam-insidemen-cropped-ijn0410z__10__bc0f4c3311474c19b239f1c71ff2b57c.jpg')
INSERT [dbo].[image] ([id], [product_id], [url]) VALUES (7, 2, N'https://product.hstatic.net/200000886795/product/quan-jeans-nam-insidemen-cropped-ijn0410z__9__8253d53d148746a8afdf49452fa34ac7.jpg')
INSERT [dbo].[image] ([id], [product_id], [url]) VALUES (8, 3, N'https://cdn.kkfashion.vn/25220-large_default/ao-so-mi-nu-cong-so-mau-trang-tay-dai-asm15-12.jpg')
INSERT [dbo].[image] ([id], [product_id], [url]) VALUES (9, 4, N'https://pos.nvncdn.com/80a557-93682/ps/20230710_8xEuqh8NRG.png')
INSERT [dbo].[image] ([id], [product_id], [url]) VALUES (10, 5, N'https://myshoes.vn/image/cache/catalog/2025/nike/giay-nike-pegasus-41-nu-phantom-01-500x500.jpg')
INSERT [dbo].[image] ([id], [product_id], [url]) VALUES (11, 5, N'https://myshoes.vn/image/cache/catalog/2025/nike/giay-nike-pegasus-41-nu-phantom-02-800x800.jpg')
INSERT [dbo].[image] ([id], [product_id], [url]) VALUES (12, 5, N'https://myshoes.vn/image/cache/catalog/2025/nike/giay-nike-pegasus-41-nu-phantom-04-800x800.jpg')
INSERT [dbo].[image] ([id], [product_id], [url]) VALUES (13, 5, N'https://myshoes.vn/image/cache/catalog/2025/nike/giay-nike-pegasus-41-nu-phantom-04-800x800.jpg')
INSERT [dbo].[image] ([id], [product_id], [url]) VALUES (14, 6, N'https://www.vascara.com/uploads/cms_productmedia/2025/January/23/tui-xach-tay-quai-doi-van-da-phoi-tuong-phan---tot-0193---mau-be__80308__1737606729.jpg')
INSERT [dbo].[image] ([id], [product_id], [url]) VALUES (15, 6, N'https://www.vascara.com/uploads/cms_productmedia/2025/January/23/tui-xach-tay-quai-doi-van-da-phoi-tuong-phan---tot-0193---mau-be__80310__1737606751.jpg')
INSERT [dbo].[image] ([id], [product_id], [url]) VALUES (16, 6, N'https://www.vascara.com/uploads/cms_productmedia/2025/January/23/tui-xach-tay-quai-doi-van-da-phoi-tuong-phan---tot-0193---mau-be__80309__1737606739.jpg')
INSERT [dbo].[image] ([id], [product_id], [url]) VALUES (17, 7, N'https://product.hstatic.net/1000360022/product/id-005769a_9d320dcfce3f4e5bb993a1ac13304c4f_master.jpg')
INSERT [dbo].[image] ([id], [product_id], [url]) VALUES (18, 8, N'https://bizweb.dktcdn.net/100/287/440/products/mu-luoi-trai-local-brand-dep-mau-be-1.jpg?v=1644822065327')
INSERT [dbo].[image] ([id], [product_id], [url]) VALUES (19, 9, N'https://salt.tikicdn.com/cache/280x280/ts/product/9a/7c/6f/9edffc4f2ccd5be435fd2a0a784eeaa8.JPG')
INSERT [dbo].[image] ([id], [product_id], [url]) VALUES (20, 10, N'https://donghoduyanh.com/images/products/2023/07/31/large/an8201-57l_1690775073.jpg')
INSERT [dbo].[image] ([id], [product_id], [url]) VALUES (21, 12, N'/images/9a87194d-7368-499c-b237-d501dc8556d5_image.jpg')
INSERT [dbo].[image] ([id], [product_id], [url]) VALUES (22, 12, N'/images/4e2e98dc-e15a-4152-82e4-02e09ceecb45_image.jpg')
INSERT [dbo].[image] ([id], [product_id], [url]) VALUES (23, 12, N'/images/b6fd11bf-5994-4e78-ac72-f773792fc2c2_image.jpg')
INSERT [dbo].[image] ([id], [product_id], [url]) VALUES (24, 13, N'/images/654f8f2c-415b-4e7c-a384-477c76fa233f_image.jpg')
INSERT [dbo].[image] ([id], [product_id], [url]) VALUES (25, 13, N'/images/0b77b788-4a42-4f1b-8553-c75b66382f18_image.jpg')
INSERT [dbo].[image] ([id], [product_id], [url]) VALUES (26, 13, N'/images/5035a51e-23d6-44ec-9a29-a8f47d32cd90_image.jpg')
SET IDENTITY_INSERT [dbo].[image] OFF
GO
INSERT [dbo].[order] ([id], [customer_id], [voucher_id], [full_name], [phone_number], [address], [note], [order_date], [status], [total_amount], [create_at], [update_at]) VALUES (N'f7bc5024-c7af-4b47-9b90-4628e276af6c', N'680fefa1-9463-40a3-9d48-594529ab96cf', NULL, N'Nguyen Van C', N'93021093019', N'Ninh Binh', N'none', CAST(N'2025-03-20T22:03:41.697' AS DateTime), 1, 250000, CAST(N'2025-03-20T22:03:41.697' AS DateTime), NULL)
INSERT [dbo].[order] ([id], [customer_id], [voucher_id], [full_name], [phone_number], [address], [note], [order_date], [status], [total_amount], [create_at], [update_at]) VALUES (N'680fefa1-9463-40a3-9d48-594529ab16cf', N'680fefa1-9463-40a3-9d48-594529ab96cf', NULL, N'Đặng Văn E', N'0987654321', N'Hà Nội', N'', CAST(N'2025-02-25T16:31:25.130' AS DateTime), 4, 950000, CAST(N'2025-02-25T16:31:25.130' AS DateTime), NULL)
INSERT [dbo].[order] ([id], [customer_id], [voucher_id], [full_name], [phone_number], [address], [note], [order_date], [status], [total_amount], [create_at], [update_at]) VALUES (N'680fefa1-9463-40a3-9d48-594529ab26cf', N'dde923de-6b2a-4104-a293-6da7aaa68ef3', NULL, N'Trần Thị B', N'0977654321', N'TP.HCM', N'', CAST(N'2025-02-25T16:31:25.130' AS DateTime), 4, 1260000, CAST(N'2025-02-25T16:31:25.130' AS DateTime), NULL)
INSERT [dbo].[order] ([id], [customer_id], [voucher_id], [full_name], [phone_number], [address], [note], [order_date], [status], [total_amount], [create_at], [update_at]) VALUES (N'680fefa1-9463-40a3-9d48-594529ab36cf', N'042d839d-8c72-49ba-8d6b-95b1d5a026a9', NULL, N'Phạm Văn C', N'0967654321', N'Đà Nẵng', N'', CAST(N'2025-02-25T16:31:25.130' AS DateTime), 4, 425000, CAST(N'2025-02-25T16:31:25.130' AS DateTime), NULL)
INSERT [dbo].[order] ([id], [customer_id], [voucher_id], [full_name], [phone_number], [address], [note], [order_date], [status], [total_amount], [create_at], [update_at]) VALUES (N'680fefa1-9463-40a3-9d48-594529ab46cf', N'1d809d9a-4780-48b4-8d2e-a1f3aad9faf6', NULL, N'Nguyễn Văn A', N'0957654321', N'Cần Thơ', N'', CAST(N'2025-02-25T16:31:25.130' AS DateTime), 4, 1738000, CAST(N'2025-02-25T16:31:25.130' AS DateTime), NULL)
INSERT [dbo].[order] ([id], [customer_id], [voucher_id], [full_name], [phone_number], [address], [note], [order_date], [status], [total_amount], [create_at], [update_at]) VALUES (N'680fefa1-9463-40a3-9d48-594529ab56cf', N'5400c558-7741-4b47-93c6-f5c23add1559', NULL, N'Trần Tuấn Anh', N'0344191620', N'Hà Nội', N'', CAST(N'2025-03-11T10:37:56.027' AS DateTime), 4, 1738000, CAST(N'2025-03-11T10:37:56.027' AS DateTime), NULL)
INSERT [dbo].[order] ([id], [customer_id], [voucher_id], [full_name], [phone_number], [address], [note], [order_date], [status], [total_amount], [create_at], [update_at]) VALUES (N'94808df8-5a21-44be-b373-61953d959a3f', N'680fefa1-9463-40a3-9d48-594529ab96cf', 2, N'Nguyen Van C', N'93021093019', N'Ninh Binh', N'mmmm', CAST(N'2025-03-20T21:44:07.883' AS DateTime), 2, 270000, CAST(N'2025-03-20T21:44:07.883' AS DateTime), NULL)
INSERT [dbo].[order] ([id], [customer_id], [voucher_id], [full_name], [phone_number], [address], [note], [order_date], [status], [total_amount], [create_at], [update_at]) VALUES (N'4397c55c-14ca-46f8-9cb6-7cd0c56ddc00', N'680fefa1-9463-40a3-9d48-594529ab96cf', NULL, N'Nguyen Van A', N'93021093019', N'Ha Noi', N'none', CAST(N'2025-03-14T22:04:57.013' AS DateTime), 4, 1000, CAST(N'2025-03-14T22:04:57.013' AS DateTime), NULL)
INSERT [dbo].[order] ([id], [customer_id], [voucher_id], [full_name], [phone_number], [address], [note], [order_date], [status], [total_amount], [create_at], [update_at]) VALUES (N'7bae99c3-2d64-4540-b334-d76a06143f2c', N'680fefa1-9463-40a3-9d48-594529ab96cf', 7, N'Nguyen Van C', N'93021093019', N'Ninh Binh', N'none', CAST(N'2025-03-20T22:34:02.943' AS DateTime), 1, 290000, CAST(N'2025-03-20T22:34:02.943' AS DateTime), NULL)
INSERT [dbo].[order] ([id], [customer_id], [voucher_id], [full_name], [phone_number], [address], [note], [order_date], [status], [total_amount], [create_at], [update_at]) VALUES (N'1174f72a-9024-4cce-97ab-e52973c76604', N'680fefa1-9463-40a3-9d48-594529ab96cf', NULL, N'Nguyen Van C', N'93021093019', N'Ninh Binh', N'none', CAST(N'2025-03-14T23:54:00.833' AS DateTime), 4, 1000, CAST(N'2025-03-14T23:54:00.833' AS DateTime), NULL)
INSERT [dbo].[order] ([id], [customer_id], [voucher_id], [full_name], [phone_number], [address], [note], [order_date], [status], [total_amount], [create_at], [update_at]) VALUES (N'983e5f37-1553-45a8-9b6b-e98a4d0f4186', N'680fefa1-9463-40a3-9d48-594529ab96cf', 2, N'Nguyen Van C', N'93021093019', N'Ninh Binh', N'meo', CAST(N'2025-03-20T21:41:44.663' AS DateTime), 1, 675000, CAST(N'2025-03-20T21:41:44.663' AS DateTime), NULL)
GO
SET IDENTITY_INSERT [dbo].[order_details] ON 

INSERT [dbo].[order_details] ([id], [order_id], [product_id], [quantity], [unit_price], [discount], [total_price], [status]) VALUES (1, N'680fefa1-9463-40a3-9d48-594529ab16cf', 1, 1, 250000, 0, 250000, 1)
INSERT [dbo].[order_details] ([id], [order_id], [product_id], [quantity], [unit_price], [discount], [total_price], [status]) VALUES (2, N'680fefa1-9463-40a3-9d48-594529ab16cf', 5, 1, 700000, 0, 700000, 1)
INSERT [dbo].[order_details] ([id], [order_id], [product_id], [quantity], [unit_price], [discount], [total_price], [status]) VALUES (3, N'680fefa1-9463-40a3-9d48-594529ab26cf', 4, 1, 450000, 20, 360000, 1)
INSERT [dbo].[order_details] ([id], [order_id], [product_id], [quantity], [unit_price], [discount], [total_price], [status]) VALUES (4, N'680fefa1-9463-40a3-9d48-594529ab26cf', 6, 1, 900000, 0, 900000, 1)
INSERT [dbo].[order_details] ([id], [order_id], [product_id], [quantity], [unit_price], [discount], [total_price], [status]) VALUES (5, N'680fefa1-9463-40a3-9d48-594529ab36cf', 2, 1, 500000, 15, 425000, 1)
INSERT [dbo].[order_details] ([id], [order_id], [product_id], [quantity], [unit_price], [discount], [total_price], [status]) VALUES (6, N'680fefa1-9463-40a3-9d48-594529ab46cf', 7, 1, 400000, 10, 360000, 1)
INSERT [dbo].[order_details] ([id], [order_id], [product_id], [quantity], [unit_price], [discount], [total_price], [status]) VALUES (7, N'680fefa1-9463-40a3-9d48-594529ab46cf', 9, 1, 350000, 8, 322000, 1)
INSERT [dbo].[order_details] ([id], [order_id], [product_id], [quantity], [unit_price], [discount], [total_price], [status]) VALUES (8, N'680fefa1-9463-40a3-9d48-594529ab46cf', 10, 1, 1200000, 12, 1056000, 1)
INSERT [dbo].[order_details] ([id], [order_id], [product_id], [quantity], [unit_price], [discount], [total_price], [status]) VALUES (9, N'680fefa1-9463-40a3-9d48-594529ab56cf', 7, 1, 400000, 10, 360000, 1)
INSERT [dbo].[order_details] ([id], [order_id], [product_id], [quantity], [unit_price], [discount], [total_price], [status]) VALUES (10, N'680fefa1-9463-40a3-9d48-594529ab56cf', 9, 1, 350000, 8, 322000, 1)
INSERT [dbo].[order_details] ([id], [order_id], [product_id], [quantity], [unit_price], [discount], [total_price], [status]) VALUES (11, N'680fefa1-9463-40a3-9d48-594529ab56cf', 10, 1, 1200000, 12, 1056000, 1)
INSERT [dbo].[order_details] ([id], [order_id], [product_id], [quantity], [unit_price], [discount], [total_price], [status]) VALUES (12, N'4397c55c-14ca-46f8-9cb6-7cd0c56ddc00', 10, 1, 1200000, 0, 1200000, 1)
INSERT [dbo].[order_details] ([id], [order_id], [product_id], [quantity], [unit_price], [discount], [total_price], [status]) VALUES (13, N'1174f72a-9024-4cce-97ab-e52973c76604', 1, 1, 250000, 0, 250000, 1)
INSERT [dbo].[order_details] ([id], [order_id], [product_id], [quantity], [unit_price], [discount], [total_price], [status]) VALUES (14, N'1174f72a-9024-4cce-97ab-e52973c76604', 9, 1, 350000, 0, 350000, 1)
INSERT [dbo].[order_details] ([id], [order_id], [product_id], [quantity], [unit_price], [discount], [total_price], [status]) VALUES (15, N'983e5f37-1553-45a8-9b6b-e98a4d0f4186', 1, 1, 250000, 0, 250000, 1)
INSERT [dbo].[order_details] ([id], [order_id], [product_id], [quantity], [unit_price], [discount], [total_price], [status]) VALUES (16, N'983e5f37-1553-45a8-9b6b-e98a4d0f4186', 2, 1, 500000, 0, 500000, 5)
INSERT [dbo].[order_details] ([id], [order_id], [product_id], [quantity], [unit_price], [discount], [total_price], [status]) VALUES (17, N'94808df8-5a21-44be-b373-61953d959a3f', 3, 1, 300000, 0, 300000, 1)
INSERT [dbo].[order_details] ([id], [order_id], [product_id], [quantity], [unit_price], [discount], [total_price], [status]) VALUES (22, N'f7bc5024-c7af-4b47-9b90-4628e276af6c', 1, 1, 250000, 0, 250000, 1)
INSERT [dbo].[order_details] ([id], [order_id], [product_id], [quantity], [unit_price], [discount], [total_price], [status]) VALUES (23, N'7bae99c3-2d64-4540-b334-d76a06143f2c', 3, 1, 300000, 0, 300000, 1)
SET IDENTITY_INSERT [dbo].[order_details] OFF
GO
INSERT [dbo].[order_status] ([id], [name]) VALUES (1, N'PENDING CONFIRMATION')
INSERT [dbo].[order_status] ([id], [name]) VALUES (2, N'CONFIRMED')
INSERT [dbo].[order_status] ([id], [name]) VALUES (3, N'SHIPPING')
INSERT [dbo].[order_status] ([id], [name]) VALUES (4, N'DELIVERED')
INSERT [dbo].[order_status] ([id], [name]) VALUES (5, N'CANCELLED')
INSERT [dbo].[order_status] ([id], [name]) VALUES (6, N'RETURNED')
GO
INSERT [dbo].[orderdetail_status] ([id], [name]) VALUES (1, N'PENDING CONFIRMATION')
INSERT [dbo].[orderdetail_status] ([id], [name]) VALUES (2, N'CONFIRMED')
INSERT [dbo].[orderdetail_status] ([id], [name]) VALUES (3, N'SHIPPING')
INSERT [dbo].[orderdetail_status] ([id], [name]) VALUES (4, N'DELIVERED')
INSERT [dbo].[orderdetail_status] ([id], [name]) VALUES (5, N'CANCELLED')
INSERT [dbo].[orderdetail_status] ([id], [name]) VALUES (6, N'RETURNED')
GO
SET IDENTITY_INSERT [dbo].[product] ON 

INSERT [dbo].[product] ([id], [seller_id], [name], [category_id], [thumbnail_url], [description], [price], [discount], [quantity], [status], [create_at], [update_at]) VALUES (1, N'1d809d9a-4780-48b4-8d2e-a1f3aad9faf6', N'Áo Thun Nam GENTO', 2, N'https://4menshop.com/cache/image/300x400/images/thumbs/2025/02/ao-thun-co-tron-in-chu-dream-form-regular-at163_small-19113.jpg', N'Áo thun nam cao cấp', 250000, 0, 97, 1, CAST(N'2025-02-19T17:01:27.673' AS DateTime), NULL)
INSERT [dbo].[product] ([id], [seller_id], [name], [category_id], [thumbnail_url], [description], [price], [discount], [quantity], [status], [create_at], [update_at]) VALUES (2, N'dde923de-6b2a-4104-a293-6da7aaa68ef3', N'Quần Jeans Nam Devis', 1, N'https://cdn.boo.vn/media/catalog/product/1/_/1.2.21.2.23.001.124.01.60600034_1__4.jpg', N'Quần jeans nam trẻ trung', 500000, 15, 49, 1, CAST(N'2025-02-19T17:01:27.673' AS DateTime), NULL)
INSERT [dbo].[product] ([id], [seller_id], [name], [category_id], [thumbnail_url], [description], [price], [discount], [quantity], [status], [create_at], [update_at]) VALUES (3, N'1d809d9a-4780-48b4-8d2e-a1f3aad9faf6', N'Áo Sơ Mi Nữ GENTO', 2, N'https://cdn.kkfashion.vn/25220-large_default/ao-so-mi-nu-cong-so-mau-trang-tay-dai-asm15-12.jpg', N'Áo sơ mi nữ thanh lịch', 300000, 0, 68, 1, CAST(N'2025-02-19T17:01:27.673' AS DateTime), NULL)
INSERT [dbo].[product] ([id], [seller_id], [name], [category_id], [thumbnail_url], [description], [price], [discount], [quantity], [status], [create_at], [update_at]) VALUES (4, N'dde923de-6b2a-4104-a293-6da7aaa68ef3', N'Váy Đầm Nữ GENTO', 1, N'https://pos.nvncdn.com/80a557-93682/ps/20230710_8xEuqh8NRG.png', N'Váy đầm nữ thời trang', 450000, 20, 30, 1, CAST(N'2025-02-19T17:01:27.673' AS DateTime), NULL)
INSERT [dbo].[product] ([id], [seller_id], [name], [category_id], [thumbnail_url], [description], [price], [discount], [quantity], [status], [create_at], [update_at]) VALUES (5, N'1d809d9a-4780-48b4-8d2e-a1f3aad9faf6', N'Giày Sneaker NIKE', 3, N'https://myshoes.vn/image/cache/catalog/2025/nike/giay-nike-pegasus-41-nu-phantom-01-500x500.jpg', N'Giày sneaker unisex', 700000, 0, 40, 1, CAST(N'2025-02-19T17:01:27.673' AS DateTime), NULL)
INSERT [dbo].[product] ([id], [seller_id], [name], [category_id], [thumbnail_url], [description], [price], [discount], [quantity], [status], [create_at], [update_at]) VALUES (6, N'dde923de-6b2a-4104-a293-6da7aaa68ef3', N'Túi Xách Nữ GENTO', 5, N'https://www.vascara.com/uploads/cms_productmedia/2025/January/23/tui-xach-tay-quai-doi-van-da-phoi-tuong-phan---tot-0193---mau-be__80307__1737606720-medium.jpg', N'Túi xách nữ hàng hiệu', 900000, 0, 20, 1, CAST(N'2025-02-19T17:01:27.673' AS DateTime), NULL)
INSERT [dbo].[product] ([id], [seller_id], [name], [category_id], [thumbnail_url], [description], [price], [discount], [quantity], [status], [create_at], [update_at]) VALUES (7, N'1d809d9a-4780-48b4-8d2e-a1f3aad9faf6', N'Áo Hoodie Nam Devis', 2, N'https://product.hstatic.net/1000360022/product/id-005769a_9d320dcfce3f4e5bb993a1ac13304c4f_master.jpg', N'Áo hoodie ấm áp', 400000, 10, 60, 1, CAST(N'2025-02-19T17:01:27.673' AS DateTime), NULL)
INSERT [dbo].[product] ([id], [seller_id], [name], [category_id], [thumbnail_url], [description], [price], [discount], [quantity], [status], [create_at], [update_at]) VALUES (8, N'dde923de-6b2a-4104-a293-6da7aaa68ef3', N'Mũ Lưỡi Trai Devis', 4, N'https://bizweb.dktcdn.net/100/287/440/products/mu-luoi-trai-local-brand-dep-mau-be-1.jpg?v=1644822065327', N'Mũ lưỡi trai phong cách', 150000, 5, 90, 1, CAST(N'2025-02-19T17:01:27.673' AS DateTime), NULL)
INSERT [dbo].[product] ([id], [seller_id], [name], [category_id], [thumbnail_url], [description], [price], [discount], [quantity], [status], [create_at], [update_at]) VALUES (9, N'dde923de-6b2a-4104-a293-6da7aaa68ef3', N'Kính Râm VSP303', 4, N'https://salt.tikicdn.com/cache/280x280/ts/product/9a/7c/6f/9edffc4f2ccd5be435fd2a0a784eeaa8.JPG', N'Kính râm chống UV', 350000, 8, 34, 1, CAST(N'2025-02-19T17:01:27.673' AS DateTime), NULL)
INSERT [dbo].[product] ([id], [seller_id], [name], [category_id], [thumbnail_url], [description], [price], [discount], [quantity], [status], [create_at], [update_at]) VALUES (10, N'1d809d9a-4780-48b4-8d2e-a1f3aad9faf6', N'Đồng Hồ Nam Citizen AN8201-57L', 4, N'https://donghoduyanh.com/images/products/2023/07/31/large/an8201-57l_1690775073.jpg', N'Đồng hồ nam thời trang', 1200000, 12, 0, 1, CAST(N'2025-02-19T17:01:27.673' AS DateTime), NULL)
INSERT [dbo].[product] ([id], [seller_id], [name], [category_id], [thumbnail_url], [description], [price], [discount], [quantity], [status], [create_at], [update_at]) VALUES (11, N'dde923de-6b2a-4104-a293-6da7aaa68ef3', N'Áo thun CỔ V vải COTTON', 2, N'/images/7a324831-9c0d-4b47-96ff-bf0928657d3f_thumbnail.jpg', N'Chất liệu cotton interlock 250gsm
Co giãn tốt, mặc cực thoải mái, thấm hút mồ hôi tốt
Thiết kế theo form rộng vừa, đơn giản, dễ mặc.', 200000, 0, 50, 2, CAST(N'2025-03-21T09:18:24.207' AS DateTime), NULL)
INSERT [dbo].[product] ([id], [seller_id], [name], [category_id], [thumbnail_url], [description], [price], [discount], [quantity], [status], [create_at], [update_at]) VALUES (12, N'dde923de-6b2a-4104-a293-6da7aaa68ef3', N'Giày sandals quai bản to nhấn khóa trang trí ', 3, N'/images/b6e9b694-583d-4d0c-b48c-9358af5c71fa_thumbnail.jpg', N'Giày sandals nhấn khóa trang trí ', 680000, 0, 10, 2, CAST(N'2025-03-21T09:25:39.713' AS DateTime), NULL)
INSERT [dbo].[product] ([id], [seller_id], [name], [category_id], [thumbnail_url], [description], [price], [discount], [quantity], [status], [create_at], [update_at]) VALUES (13, N'dde923de-6b2a-4104-a293-6da7aaa68ef3', N'Giày sandals quai bản to nhấn khóa trang trí ', 3, N'/images/1a26679e-f91e-427d-83b9-c4f7509c9da2_thumbnail.jpg', N'Giày sandals nhấn khóa trang trí ', 700000, 0, 10, 1, CAST(N'2025-03-21T09:34:04.910' AS DateTime), NULL)
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
INSERT [dbo].[userinfo] ([id], [full_name], [phone_number], [avatar_url], [gender], [date_of_birth], [address], [update_at]) VALUES (N'680fefa1-9463-40a3-9d48-594529ab96cf', N'Nguyen Van C', N'93021093019', NULL, NULL, NULL, N'Ninh Binh', NULL)
GO
SET IDENTITY_INSERT [dbo].[voucher] ON 

INSERT [dbo].[voucher] ([id], [code], [type], [value], [description], [start_date], [end_date], [status], [create_at], [update_at], [usage_limit], [usage_count]) VALUES (2, N'DISCOUNT10', 1, CAST(10.00 AS Decimal(10, 2)), N'Giảm giá 10% cho đơn hàng', CAST(N'2025-03-01T00:00:00.000' AS DateTime), CAST(N'2025-03-31T00:00:00.000' AS DateTime), 1, CAST(N'2025-03-20T21:38:45.450' AS DateTime), CAST(N'2025-03-20T21:38:45.450' AS DateTime), 10, 0)
INSERT [dbo].[voucher] ([id], [code], [type], [value], [description], [start_date], [end_date], [status], [create_at], [update_at], [usage_limit], [usage_count]) VALUES (3, N'DISCOUNT20', 1, CAST(20.00 AS Decimal(10, 2)), N'Giảm giá 20% cho đơn hàng', CAST(N'2024-09-01T00:00:00.000' AS DateTime), CAST(N'2024-09-30T00:00:00.000' AS DateTime), 2, CAST(N'2025-03-20T21:38:45.450' AS DateTime), CAST(N'2025-03-20T21:38:45.450' AS DateTime), 10, 0)
INSERT [dbo].[voucher] ([id], [code], [type], [value], [description], [start_date], [end_date], [status], [create_at], [update_at], [usage_limit], [usage_count]) VALUES (4, N'DISCOUNT30', 1, CAST(30.00 AS Decimal(10, 2)), N'Giảm giá 30% cho đơn hàng', CAST(N'2025-03-01T00:00:00.000' AS DateTime), CAST(N'2025-03-31T00:00:00.000' AS DateTime), 3, CAST(N'2025-03-20T21:38:45.450' AS DateTime), CAST(N'2025-03-20T21:38:45.450' AS DateTime), 10, 10)
INSERT [dbo].[voucher] ([id], [code], [type], [value], [description], [start_date], [end_date], [status], [create_at], [update_at], [usage_limit], [usage_count]) VALUES (5, N'DISCOUNT40', 1, CAST(40.00 AS Decimal(10, 2)), N'Giảm giá 40% cho đơn hàng', CAST(N'2025-03-01T00:00:00.000' AS DateTime), CAST(N'2025-03-31T00:00:00.000' AS DateTime), 4, CAST(N'2025-03-20T21:38:45.450' AS DateTime), CAST(N'2025-03-20T21:38:45.450' AS DateTime), 10, 0)
INSERT [dbo].[voucher] ([id], [code], [type], [value], [description], [start_date], [end_date], [status], [create_at], [update_at], [usage_limit], [usage_count]) VALUES (6, N'DISCOUNT50', 1, CAST(50.00 AS Decimal(10, 2)), N'Giảm giá 50% cho đơn hàng', CAST(N'2025-03-01T00:00:00.000' AS DateTime), CAST(N'2025-03-31T00:00:00.000' AS DateTime), 5, CAST(N'2025-03-20T21:38:45.450' AS DateTime), CAST(N'2025-03-20T21:38:45.450' AS DateTime), 10, 0)
INSERT [dbo].[voucher] ([id], [code], [type], [value], [description], [start_date], [end_date], [status], [create_at], [update_at], [usage_limit], [usage_count]) VALUES (7, N'AMOUNT10', 2, CAST(10000.00 AS Decimal(10, 2)), N'Giảm giá 10.000đ cho đơn hàng', CAST(N'2025-03-01T00:00:00.000' AS DateTime), CAST(N'2025-03-31T00:00:00.000' AS DateTime), 1, CAST(N'2025-03-20T21:38:45.450' AS DateTime), CAST(N'2025-03-20T21:38:45.450' AS DateTime), 10, 0)
INSERT [dbo].[voucher] ([id], [code], [type], [value], [description], [start_date], [end_date], [status], [create_at], [update_at], [usage_limit], [usage_count]) VALUES (8, N'AMOUNT20', 2, CAST(20000.00 AS Decimal(10, 2)), N'Giảm giá 20.000đ cho đơn hàng', CAST(N'2024-09-01T00:00:00.000' AS DateTime), CAST(N'2024-09-30T00:00:00.000' AS DateTime), 2, CAST(N'2025-03-20T21:38:45.450' AS DateTime), CAST(N'2025-03-20T21:38:45.450' AS DateTime), 10, 0)
INSERT [dbo].[voucher] ([id], [code], [type], [value], [description], [start_date], [end_date], [status], [create_at], [update_at], [usage_limit], [usage_count]) VALUES (9, N'AMOUNT30', 2, CAST(30000.00 AS Decimal(10, 2)), N'Giảm giá 30.000đ cho đơn hàng', CAST(N'2025-03-01T00:00:00.000' AS DateTime), CAST(N'2025-03-31T00:00:00.000' AS DateTime), 3, CAST(N'2025-03-20T21:38:45.450' AS DateTime), CAST(N'2025-03-20T21:38:45.450' AS DateTime), 10, 10)
INSERT [dbo].[voucher] ([id], [code], [type], [value], [description], [start_date], [end_date], [status], [create_at], [update_at], [usage_limit], [usage_count]) VALUES (10, N'AMOUNT40', 2, CAST(40000.00 AS Decimal(10, 2)), N'Giảm giá 40.000đ cho đơn hàng', CAST(N'2025-03-01T00:00:00.000' AS DateTime), CAST(N'2025-03-31T00:00:00.000' AS DateTime), 4, CAST(N'2025-03-20T21:38:45.450' AS DateTime), CAST(N'2025-03-20T21:38:45.450' AS DateTime), 10, 0)
INSERT [dbo].[voucher] ([id], [code], [type], [value], [description], [start_date], [end_date], [status], [create_at], [update_at], [usage_limit], [usage_count]) VALUES (11, N'AMOUNT50', 2, CAST(50000.00 AS Decimal(10, 2)), N'Giảm giá 50.000đ cho đơn hàng', CAST(N'2025-03-01T00:00:00.000' AS DateTime), CAST(N'2025-03-31T00:00:00.000' AS DateTime), 5, CAST(N'2025-03-20T21:38:45.450' AS DateTime), CAST(N'2025-03-20T21:38:45.450' AS DateTime), 10, 0)
INSERT [dbo].[voucher] ([id], [code], [type], [value], [description], [start_date], [end_date], [status], [create_at], [update_at], [usage_limit], [usage_count]) VALUES (12, N'FREESHIP1', 3, CAST(0.00 AS Decimal(10, 2)), N'Miễn phí vận chuyển', CAST(N'2025-03-01T00:00:00.000' AS DateTime), CAST(N'2025-03-31T00:00:00.000' AS DateTime), 1, CAST(N'2025-03-20T21:38:45.450' AS DateTime), CAST(N'2025-03-20T21:38:45.450' AS DateTime), 10, 0)
INSERT [dbo].[voucher] ([id], [code], [type], [value], [description], [start_date], [end_date], [status], [create_at], [update_at], [usage_limit], [usage_count]) VALUES (13, N'FREESHIP2', 3, CAST(0.00 AS Decimal(10, 2)), N'Miễn phí vận chuyển', CAST(N'2024-09-01T00:00:00.000' AS DateTime), CAST(N'2024-09-30T00:00:00.000' AS DateTime), 2, CAST(N'2025-03-20T21:38:45.450' AS DateTime), CAST(N'2025-03-20T21:38:45.450' AS DateTime), 10, 0)
INSERT [dbo].[voucher] ([id], [code], [type], [value], [description], [start_date], [end_date], [status], [create_at], [update_at], [usage_limit], [usage_count]) VALUES (14, N'FREESHIP3', 3, CAST(0.00 AS Decimal(10, 2)), N'Miễn phí vận chuyển', CAST(N'2025-03-01T00:00:00.000' AS DateTime), CAST(N'2025-03-31T00:00:00.000' AS DateTime), 3, CAST(N'2025-03-20T21:38:45.450' AS DateTime), CAST(N'2025-03-20T21:38:45.450' AS DateTime), 10, 10)
INSERT [dbo].[voucher] ([id], [code], [type], [value], [description], [start_date], [end_date], [status], [create_at], [update_at], [usage_limit], [usage_count]) VALUES (15, N'FREESHIP4', 3, CAST(0.00 AS Decimal(10, 2)), N'Miễn phí vận chuyển', CAST(N'2025-03-01T00:00:00.000' AS DateTime), CAST(N'2025-03-31T00:00:00.000' AS DateTime), 4, CAST(N'2025-03-20T21:38:45.450' AS DateTime), CAST(N'2025-03-20T21:38:45.450' AS DateTime), 10, 0)
INSERT [dbo].[voucher] ([id], [code], [type], [value], [description], [start_date], [end_date], [status], [create_at], [update_at], [usage_limit], [usage_count]) VALUES (16, N'FREESHIP5', 3, CAST(0.00 AS Decimal(10, 2)), N'Miễn phí vận chuyển', CAST(N'2025-03-01T00:00:00.000' AS DateTime), CAST(N'2025-03-31T00:00:00.000' AS DateTime), 5, CAST(N'2025-03-20T21:38:45.450' AS DateTime), CAST(N'2025-03-20T21:38:45.450' AS DateTime), 10, 0)
SET IDENTITY_INSERT [dbo].[voucher] OFF
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
SET IDENTITY_INSERT [dbo].[VoucherUsage] ON 

INSERT [dbo].[VoucherUsage] ([id], [user_id], [voucher_id], [used_at]) VALUES (1, N'680fefa1-9463-40a3-9d48-594529ab96cf', 2, CAST(N'2025-03-20T21:44:43.267' AS DateTime))
SET IDENTITY_INSERT [dbo].[VoucherUsage] OFF
GO
ALTER TABLE [dbo].[cart] ADD  DEFAULT (newid()) FOR [id]
GO
ALTER TABLE [dbo].[cart] ADD  DEFAULT ((0)) FOR [total_amount]
GO
ALTER TABLE [dbo].[cart] ADD  DEFAULT (getdate()) FOR [create_at]
GO
ALTER TABLE [dbo].[cart] ADD  DEFAULT (getdate()) FOR [update_at]
GO
ALTER TABLE [dbo].[cart_detail] ADD  DEFAULT ((0)) FOR [quantity]
GO
ALTER TABLE [dbo].[cart_detail] ADD  DEFAULT ((0)) FOR [total_price]
GO
ALTER TABLE [dbo].[cart_detail] ADD  DEFAULT (getdate()) FOR [create_at]
GO
ALTER TABLE [dbo].[cart_detail] ADD  DEFAULT (getdate()) FOR [update_at]
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
ALTER TABLE [dbo].[VoucherUsage] ADD  DEFAULT (getdate()) FOR [used_at]
GO
ALTER TABLE [dbo].[cart]  WITH CHECK ADD FOREIGN KEY([user_id])
REFERENCES [dbo].[user] ([id])
GO
ALTER TABLE [dbo].[cart_detail]  WITH CHECK ADD FOREIGN KEY([cart_id])
REFERENCES [dbo].[cart] ([id])
GO
ALTER TABLE [dbo].[cart_detail]  WITH CHECK ADD FOREIGN KEY([product_id])
REFERENCES [dbo].[product] ([id])
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
ALTER TABLE [dbo].[order_details]  WITH CHECK ADD FOREIGN KEY([status])
REFERENCES [dbo].[orderdetail_status] ([id])
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
ALTER TABLE [dbo].[VoucherUsage]  WITH CHECK ADD FOREIGN KEY([user_id])
REFERENCES [dbo].[user] ([id])
GO
ALTER TABLE [dbo].[VoucherUsage]  WITH CHECK ADD FOREIGN KEY([voucher_id])
REFERENCES [dbo].[voucher] ([id])
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
