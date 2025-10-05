USE master;
GO

ALTER DATABASE SalesAppDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
DROP DATABASE SalesAppDB;
GO

CREATE DATABASE SalesAppDB;
GO

USE SalesAppDB;
GO

/* ================================
   Bảng Users
================================ */
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    PhoneNumber NVARCHAR(15),
    Address NVARCHAR(255),
    Role NVARCHAR(50) NOT NULL,
    AvatarUrl NVARCHAR(255)
);
GO

/* ================================
   Bảng Categories
================================ */
CREATE TABLE Categories (
    CategoryID INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(100) NOT NULL,
    ImageUrl NVARCHAR(255)
);
GO

/* ================================
   Bảng Brands
================================ */
CREATE TABLE Brands (
    BrandID INT PRIMARY KEY IDENTITY(1,1),
    BrandName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255),
    LogoUrl NVARCHAR(255)
);
GO

/* ================================
   Bảng Products
================================ */
CREATE TABLE Products (
    ProductID INT PRIMARY KEY IDENTITY(1,1),
    ProductName NVARCHAR(100) NOT NULL,
    BriefDescription NVARCHAR(255),
    FullDescription NVARCHAR(MAX),
    TechnicalSpecifications NVARCHAR(MAX),
    Price DECIMAL(18, 2) NOT NULL,
    ImageURL NVARCHAR(255),
    CategoryID INT,
    BrandID INT,
    FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID),
    FOREIGN KEY (BrandID) REFERENCES Brands(BrandID)
);
GO

/* ================================
   Bảng Carts
================================ */
CREATE TABLE Carts (
    CartID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT,
    TotalPrice DECIMAL(18, 2) NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);
GO

/* ================================
   Bảng CartItems
================================ */
CREATE TABLE CartItems (
    CartItemID INT PRIMARY KEY IDENTITY(1,1),
    CartID INT,
    ProductID INT,
    Quantity INT NOT NULL,
    Price DECIMAL(18, 2) NOT NULL,
    FOREIGN KEY (CartID) REFERENCES Carts(CartID),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);
GO

/* ================================
   Bảng Orders
================================ */
CREATE TABLE Orders (
    OrderID INT PRIMARY KEY IDENTITY(1,1),
    CartID INT,
    UserID INT,
    PaymentMethod NVARCHAR(50) NOT NULL,
    BillingAddress NVARCHAR(255) NOT NULL,
    OrderStatus NVARCHAR(50) NOT NULL,
    OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (CartID) REFERENCES Carts(CartID),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);
GO

/* ================================
   Bảng Payments
================================ */
CREATE TABLE Payments (
    PaymentID INT PRIMARY KEY IDENTITY(1,1),
    OrderID INT,
    Amount DECIMAL(18, 2) NOT NULL,
    PaymentDate DATETIME NOT NULL DEFAULT GETDATE(),
    PaymentStatus NVARCHAR(50) NOT NULL,
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID)
);
GO

/* ================================
   Bảng Notifications
================================ */
CREATE TABLE Notifications (
    NotificationID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT,
    Message NVARCHAR(255),
    IsRead BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);
GO

/* ================================
   Bảng ChatMessages
================================ */
CREATE TABLE ChatMessages (
    ChatMessageID INT PRIMARY KEY IDENTITY(1,1),
    SenderID INT NOT NULL,
    ReceiverID INT NOT NULL,
    Message NVARCHAR(MAX),
    SentAt DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (SenderID) REFERENCES Users(UserID),
    FOREIGN KEY (ReceiverID) REFERENCES Users(UserID)
);
GO

/* ================================
   Bảng StoreLocations
================================ */
CREATE TABLE StoreLocations (
    LocationID INT PRIMARY KEY IDENTITY(1,1),
    Latitude DECIMAL(9, 6) NOT NULL,
    Longitude DECIMAL(9, 6) NOT NULL,
    Address NVARCHAR(255) NOT NULL
);
GO

/* ================================
   Bảng Wishlist
================================ */
CREATE TABLE Wishlists (
    WishlistID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    ProductID INT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID),
    CONSTRAINT UQ_Wishlist UNIQUE(UserID, ProductID)
);
GO

/* ================================
   Bảng Vouchers
================================ */
CREATE TABLE Vouchers (
    VoucherID INT PRIMARY KEY IDENTITY(1,1),
    Code NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(255),
    DiscountPercent DECIMAL(5,2),
    DiscountAmount DECIMAL(18,2),
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1
);
GO

/* ================================
   Bảng UserVouchers (User được phát voucher)
================================ */
CREATE TABLE UserVouchers (
    UserVoucherID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    VoucherID INT NOT NULL,
    IsUsed BIT NOT NULL DEFAULT 0,
    UsedAt DATETIME NULL,
    OrderID INT NULL, -- voucher đã dùng thì gắn với order
    AssignedAt DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (VoucherID) REFERENCES Vouchers(VoucherID),
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID),
    CONSTRAINT UQ_UserVoucher UNIQUE(UserID, VoucherID) -- đảm bảo 1 user chỉ có 1 bản voucher này
);
GO

/* ================================
   Bảng ProductVouchers (voucher áp dụng cho product)
================================ */
CREATE TABLE ProductVouchers (
    ProductVoucherID INT PRIMARY KEY IDENTITY(1,1),
    VoucherID INT NOT NULL,
    ProductID INT NOT NULL,
    FOREIGN KEY (VoucherID) REFERENCES Vouchers(VoucherID),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID),
    CONSTRAINT UQ_ProductVoucher UNIQUE(VoucherID, ProductID)
);
GO

/* ================================
   Insert StoreLocations
================================ */
INSERT INTO StoreLocations (Latitude, Longitude, Address)
VALUES (10.762622, 106.660172, N'268 Lý Thường Kiệt, Quận 10, TP.HCM');
GO

/* ================================
   Insert Users (1 Admin + 5 Customers = 6 users)
================================ */
INSERT INTO Users (Username, PasswordHash, Email, PhoneNumber, Address, Role, AvatarUrl)
VALUES
(N'admin', 'hash_admin', N'admin@salesapp.com', '0909000001', N'268 Lý Thường Kiệt, Q10, HCM', N'Admin', N'https://example.com/avatars/admin.jpg'),
(N'user01', 'hash_pw1', N'user01@gmail.com', '0909000002', N'12 Lê Lợi, Q1, HCM', N'Customer', N'https://example.com/avatars/user01.jpg'),
(N'user02', 'hash_pw2', N'user02@gmail.com', '0909000003', N'34 Hai Bà Trưng, Q1, HCM', N'Customer', N'https://example.com/avatars/user02.jpg'),
(N'user03', 'hash_pw3', N'user03@gmail.com', '0909000004', N'56 Nguyễn Trãi, Q5, HCM', N'Customer', N'https://example.com/avatars/user03.jpg'),
(N'user04', 'hash_pw4', N'user04@gmail.com', '0909000005', N'78 CMT8, Q3, HCM', N'Customer', N'https://example.com/avatars/user04.jpg'),
(N'user05', 'hash_pw5', N'user05@gmail.com', '0909000006', N'90 Pasteur, Q1, HCM', N'Customer', N'https://example.com/avatars/user05.jpg');
GO

/* ================================
   Insert Categories
================================ */
INSERT INTO Categories (CategoryName, ImageUrl)
VALUES
(N'Điện thoại', N'https://example.com/categories/phone.jpg'),
(N'Laptop', N'https://example.com/categories/laptop.jpg'),
(N'Thiết bị gia dụng', N'https://example.com/categories/appliance.jpg'),
(N'Phụ kiện', N'https://example.com/categories/accessory.jpg');
GO

/* ================================
   Insert Brands (6 brands)
================================ */
INSERT INTO Brands (BrandName, Description, LogoUrl)
VALUES
(N'Apple', N'Thương hiệu công nghệ hàng đầu thế giới', N'https://example.com/brands/apple.png'),
(N'Samsung', N'Tập đoàn điện tử Hàn Quốc', N'https://example.com/brands/samsung.png'),
(N'Xiaomi', N'Thương hiệu điện tử Trung Quốc', N'https://example.com/brands/xiaomi.png'),
(N'Dell', N'Thương hiệu máy tính Mỹ', N'https://example.com/brands/dell.png'),
(N'LG', N'Tập đoàn điện tử Hàn Quốc', N'https://example.com/brands/lg.png'),
(N'Sony', N'Tập đoàn giải trí và điện tử Nhật Bản', N'https://example.com/brands/sony.png');
GO

/* ================================
   Insert Products (20 sản phẩm - mỗi brand có sản phẩm từ nhiều category)
================================ */
INSERT INTO Products (ProductName, BriefDescription, FullDescription, TechnicalSpecifications, Price, ImageURL, CategoryID, BrandID)
VALUES
-- APPLE (Điện thoại + Laptop + Phụ kiện)
(N'iPhone 15 Pro', N'Smartphone cao cấp', N'Điện thoại Apple iPhone 15 Pro 256GB', N'Chip A17 Pro, 6GB RAM, 256GB ROM', 28990000, N'https://example.com/iphone15.jpg', 1, 1),
(N'iPhone 14', N'Smartphone phổ thông', N'Điện thoại Apple iPhone 14 128GB', N'Chip A15 Bionic, 6GB RAM, 128GB ROM', 19990000, N'https://example.com/iphone14.jpg', 1, 1),
(N'MacBook Air M2', N'Laptop mỏng nhẹ', N'MacBook Air M2 2023', N'Apple M2, 16GB RAM, 512GB SSD', 31990000, N'https://example.com/macbookair.jpg', 2, 1),
(N'MacBook Pro 14', N'Laptop hiệu năng cao', N'MacBook Pro 14 inch M2 Pro', N'Apple M2 Pro, 32GB RAM, 1TB SSD', 52990000, N'https://example.com/mbp14.jpg', 2, 1),
(N'Apple Watch Ultra', N'Đồng hồ thông minh', N'Apple Watch Ultra 49mm', N'Chip S8, chống nước 100m', 19990000, N'https://example.com/applewatch.jpg', 4, 1),
(N'AirPods Pro 2', N'Tai nghe không dây', N'Apple AirPods Pro Gen 2', N'ANC, Adaptive Transparency', 5990000, N'https://example.com/airpodspro2.jpg', 4, 1),

-- SAMSUNG (Điện thoại + Thiết bị gia dụng + Phụ kiện)
(N'Samsung Galaxy S23', N'Android flagship', N'Samsung Galaxy S23 Ultra 256GB', N'Snapdragon 8 Gen 2, 12GB RAM, 256GB ROM', 25990000, N'https://example.com/s23.jpg', 1, 2),
(N'Samsung Galaxy A54', N'Smartphone tầm trung', N'Samsung Galaxy A54 5G 128GB', N'Exynos 1380, 8GB RAM, 128GB ROM', 9990000, N'https://example.com/a54.jpg', 1, 2),
(N'Samsung Tủ lạnh Inverter', N'Tủ lạnh tiết kiệm điện', N'Tủ lạnh Samsung Inverter 450L', N'Ngăn đá trên, công nghệ Digital Inverter', 13990000, N'https://example.com/samsungfridge.jpg', 3, 2),
(N'Samsung Máy giặt AI', N'Máy giặt thông minh', N'Máy giặt Samsung AI Inverter 10kg', N'AI Control, Eco Bubble', 10990000, N'https://example.com/samsungwm.jpg', 3, 2),
(N'Samsung Galaxy Buds2 Pro', N'Tai nghe không dây', N'Samsung Galaxy Buds2 Pro', N'ANC, Bluetooth 5.3', 4990000, N'https://example.com/buds2.jpg', 4, 2),

-- XIAOMI (Điện thoại + Thiết bị gia dụng + Phụ kiện)
(N'Xiaomi 13 Pro', N'Flagship camera', N'Xiaomi 13 Pro 256GB', N'Snapdragon 8 Gen 2, Camera Leica', 18990000, N'https://example.com/mi13pro.jpg', 1, 3),
(N'Xiaomi Redmi Note 12', N'Giá rẻ hiệu năng cao', N'Redmi Note 12 128GB', N'Snapdragon 4 Gen 1, 6GB RAM, 128GB ROM', 5990000, N'https://example.com/redmi12.jpg', 1, 3),
(N'Xiaomi Robot Vacuum', N'Robot hút bụi', N'Xiaomi Robot Vacuum S10+', N'LDS Navigation, 4000Pa', 7990000, N'https://example.com/mivacuum.jpg', 3, 3),
(N'Xiaomi Buds 4 Pro', N'Tai nghe cao cấp', N'Xiaomi Buds 4 Pro', N'ANC, LHDC 5.0', 2990000, N'https://example.com/mibuds4.jpg', 4, 3),

-- DELL (Laptop + Phụ kiện)
(N'Dell XPS 13', N'Ultrabook cao cấp', N'Dell XPS 13 2023', N'Core i7, 16GB RAM, 512GB SSD', 38990000, N'https://example.com/xps13.jpg', 2, 4),
(N'Dell Inspiron 15', N'Laptop văn phòng', N'Dell Inspiron 15 3520', N'Core i5, 8GB RAM, 512GB SSD', 15990000, N'https://example.com/inspiron15.jpg', 2, 4),

-- LG (Thiết bị gia dụng + Phụ kiện)
(N'LG Tủ lạnh Inverter', N'Tủ lạnh tiết kiệm điện', N'Tủ lạnh LG Inverter 420L', N'Ngăn đá trên, tiết kiệm điện A++', 11990000, N'https://example.com/lgfridge.jpg', 3, 5),
(N'LG Máy lạnh Dual Inverter', N'Điều hòa cao cấp', N'Máy lạnh LG Dual Inverter 1.5HP', N'Turbo Cooling, tiết kiệm điện', 8990000, N'https://example.com/lgac.jpg', 3, 5),

-- SONY (Phụ kiện)
(N'Sony WH-1000XM5', N'Tai nghe chống ồn', N'Sony WH-1000XM5 Over-ear', N'ANC, Pin 30h', 8990000, N'https://example.com/sony1000xm5.jpg', 4, 6);
GO

/* ================================
   Insert Carts (6 giỏ hàng cho 5 customers - 1 user có 2 giỏ)
================================ */
INSERT INTO Carts (UserID, TotalPrice, Status)
VALUES
(2, 28990000, N'Active'),      -- user01 giỏ đang active
(3, 25990000, N'Completed'),   -- user02 đã hoàn thành
(4, 52990000, N'Completed'),   -- user03 đã hoàn thành
(5, 11990000, N'Completed'),   -- user04 đã hoàn thành
(6, 8990000, N'Active'),       -- user05 đang active
(2, 19990000, N'Completed');   -- user01 giỏ thứ 2 đã hoàn thành
GO

/* ================================
   Insert CartItems
================================ */
INSERT INTO CartItems (CartID, ProductID, Quantity, Price)
VALUES
(1, 1, 1, 28990000),  -- user01 cart active: iPhone 15 Pro
(2, 7, 1, 25990000),  -- user02 cart completed: Samsung S23
(3, 4, 1, 52990000),  -- user03 cart completed: MacBook Pro 14
(4, 18, 1, 11990000), -- user04 cart completed: LG Tủ lạnh
(5, 20, 1, 8990000),  -- user05 cart active: Sony WH-1000XM5
(6, 2, 1, 19990000);  -- user01 cart 2 completed: iPhone 14
GO

/* ================================
   Insert Orders (5 orders từ 5 carts đã completed)
================================ */
INSERT INTO Orders (CartID, UserID, PaymentMethod, BillingAddress, OrderStatus, OrderDate)
VALUES
(2, 3, N'Credit Card', N'34 Hai Bà Trưng, Q1, HCM', N'Success', '2025-09-15 10:30:00'),
(3, 4, N'COD', N'56 Nguyễn Trãi, Q5, HCM', N'Success', '2025-09-16 14:20:00'),
(4, 5, N'Bank Transfer', N'78 CMT8, Q3, HCM', N'Success', '2025-09-17 09:15:00'),
(6, 2, N'Momo', N'12 Lê Lợi, Q1, HCM', N'Success', '2025-09-18 11:45:00'),
(2, 3, N'COD', N'34 Hai Bà Trưng, Q1, HCM', N'Pending', '2025-09-19 16:30:00');
GO

/* ================================
   Insert Payments (5 payments tương ứng 5 orders)
================================ */
INSERT INTO Payments (OrderID, Amount, PaymentStatus, PaymentDate)
VALUES
(1, 25990000, N'Paid', '2025-09-15 10:35:00'),
(2, 52990000, N'Paid', '2025-09-16 14:25:00'),
(3, 11990000, N'Paid', '2025-09-17 09:20:00'),
(4, 19990000, N'Paid', '2025-09-18 11:50:00'),
(5, 25990000, N'Pending', '2025-09-19 16:35:00');
GO

/* ================================
   Insert Notifications (10 rows)
================================ */
INSERT INTO Notifications (UserID, Message, IsRead, CreatedAt)
VALUES
(2, N'Đơn hàng của bạn đã được xác nhận.', 0, '2025-09-15 10:30:00'),
(3, N'Thanh toán thành công đơn hàng.', 1, '2025-09-16 14:20:00'),
(4, N'Giỏ hàng của bạn còn sản phẩm chưa thanh toán.', 0, '2025-09-17 09:15:00'),
(5, N'Đơn hàng #3 đang được giao.', 1, '2025-09-18 11:45:00'),
(6, N'Bạn đã thêm sản phẩm mới vào giỏ.', 0, '2025-09-19 16:30:00'),
(2, N'Thanh toán đơn hàng #4 thành công.', 1, '2025-09-20 13:10:00'),
(3, N'Đơn hàng #5 đang chờ xác nhận.', 0, '2025-09-21 10:00:00'),
(4, N'Bạn nhận được voucher giảm giá 10%.', 0, '2025-09-22 15:25:00'),
(5, N'Sản phẩm yêu thích của bạn đang giảm giá.', 0, '2025-09-23 12:40:00'),
(6, N'Chúc mừng bạn nhận voucher 100k.', 0, '2025-09-24 09:50:00');
GO

/* ================================
   Insert ChatMessages (10 rows)
================================ */
INSERT INTO ChatMessages (SenderID, ReceiverID, Message, SentAt)
VALUES
(2, 1, N'Tôi muốn hỏi về iPhone 15 Pro còn hàng không?', '2025-09-15 10:00:00'),
(1, 2, N'Dạ vẫn còn hàng ạ. Anh/chị cần thêm thông tin gì không?', '2025-09-15 10:05:00'),
(3, 1, N'Xin báo giá chi tiết Samsung S23.', '2025-09-16 11:30:00'),
(1, 3, N'Samsung S23 hiện tại giá 25.990.000đ, bảo hành 12 tháng.', '2025-09-16 11:35:00'),
(4, 1, N'MacBook Pro 14 có giao hàng về Bình Dương không?', '2025-09-17 14:20:00'),
(1, 4, N'Shop có giao hàng toàn quốc ạ, Bình Dương được giao trong 1-2 ngày.', '2025-09-17 14:25:00'),
(5, 1, N'Tủ lạnh LG có bảo hành bao lâu?', '2025-09-18 09:15:00'),
(1, 5, N'Tủ lạnh LG bảo hành 12 tháng chính hãng ạ.', '2025-09-18 09:20:00'),
(6, 1, N'Sony WH-1000XM5 có app điều khiển không?', '2025-09-19 15:40:00'),
(1, 6, N'Có ạ, tai nghe có app Sony Headphones Connect để tùy chỉnh âm thanh.', '2025-09-19 15:45:00');
GO

/* ================================
   Insert Wishlists (10 rows)
================================ */
INSERT INTO Wishlists (UserID, ProductID, CreatedAt)
VALUES
(2, 1, '2025-09-10 10:00:00'),  -- user01 thích iPhone 15 Pro
(2, 3, '2025-09-11 14:30:00'),  -- user01 thích MacBook Air M2
(3, 7, '2025-09-12 09:15:00'),  -- user02 thích Samsung S23
(3, 5, '2025-09-12 16:45:00'),  -- user02 thích Apple Watch
(4, 4, '2025-09-13 11:20:00'),  -- user03 thích MacBook Pro 14
(4, 16, '2025-09-14 13:50:00'), -- user03 thích Dell XPS 13
(5, 18, '2025-09-15 10:30:00'), -- user04 thích LG Tủ lạnh
(5, 9, '2025-09-16 15:10:00'),  -- user04 thích Samsung Tủ lạnh
(6, 20, '2025-09-17 12:40:00'), -- user05 thích Sony WH-1000XM5
(6, 6, '2025-09-18 09:55:00');  -- user05 thích AirPods Pro 2
GO

/* ================================
   Insert Vouchers (10 vouchers)
================================ */
INSERT INTO Vouchers (Code, Description, DiscountPercent, DiscountAmount, StartDate, EndDate, IsActive)
VALUES
(N'SALE10', N'Giảm 10% cho tất cả sản phẩm', 10.00, NULL, '2025-01-01', '2025-12-31', 1),
(N'WELCOME50K', N'Giảm 50k cho khách hàng mới', NULL, 50000, '2025-01-01', '2025-12-31', 1),
(N'FLASH100K', N'Flash sale giảm 100k', NULL, 100000, '2025-10-01', '2025-10-15', 1),
(N'LAPTOP15', N'Giảm 15% cho laptop', 15.00, NULL, '2025-01-01', '2025-06-30', 1),
(N'PHONE20', N'Giảm 20% cho điện thoại', 20.00, NULL, '2025-03-01', '2025-03-31', 0),
(N'NEWYEAR200K', N'Tết giảm 200k', NULL, 200000, '2025-01-20', '2025-02-10', 1),
(N'SUMMER25', N'Hè giảm 25%', 25.00, NULL, '2025-06-01', '2025-08-31', 1),
(N'BLACKFRIDAY30', N'Black Friday giảm 30%', 30.00, NULL, '2025-11-25', '2025-11-30', 1),
(N'FREESHIP', N'Miễn phí vận chuyển', NULL, 30000, '2025-01-01', '2025-12-31', 1),
(N'VIP500K', N'Giảm 500k cho VIP', NULL, 500000, '2025-01-01', '2025-12-31', 1);
GO

/* ================================
   Insert ProductVouchers (liên kết voucher với sản phẩm)
================================ */
INSERT INTO ProductVouchers (VoucherID, ProductID)
VALUES
-- SALE10 áp dụng cho tất cả sản phẩm
(1, 1), (1, 2), (1, 3), (1, 4), (1, 5), (1, 6), (1, 7), (1, 8), (1, 9), (1, 10),
(1, 11), (1, 12), (1, 13), (1, 14), (1, 15), (1, 16), (1, 17), (1, 18), (1, 19), (1, 20),
-- LAPTOP15 chỉ cho laptop
(4, 3), (4, 4), (4, 16), (4, 17),
-- PHONE20 chỉ cho điện thoại
(5, 1), (5, 2), (5, 7), (5, 8), (5, 12), (5, 13),
-- FLASH100K cho một số sản phẩm hot
(3, 1), (3, 4), (3, 5), (3, 6),
-- SUMMER25 cho phụ kiện
(7, 5), (7, 6), (7, 11), (7, 15), (7, 20),
-- VIP500K cho sản phẩm cao cấp
(10, 1), (10, 4), (10, 7), (10, 16);
GO

/* ================================
   Insert UserVouchers (phát voucher cho 5 customers)
================================ */
INSERT INTO UserVouchers (UserID, VoucherID, IsUsed, UsedAt, OrderID, AssignedAt)
VALUES
-- User đã sử dụng voucher
(3, 1, 1, '2025-09-15 10:30:00', 1, '2025-09-10 09:00:00'), -- user02 dùng SALE10 cho order 1
(4, 2, 1, '2025-09-16 14:20:00', 2, '2025-09-11 10:00:00'), -- user03 dùng WELCOME50K cho order 2
(5, 4, 1, '2025-09-17 09:15:00', 3, '2025-09-12 11:00:00'), -- user04 dùng LAPTOP15 cho order 3
-- User chưa sử dụng voucher
(2, 1, 0, NULL, NULL, '2025-09-15 08:00:00'), -- user01 có SALE10 chưa dùng
(2, 9, 0, NULL, NULL, '2025-09-16 09:00:00'), -- user01 có FREESHIP chưa dùng
(3, 3, 0, NULL, NULL, '2025-09-17 10:00:00'), -- user02 có FLASH100K chưa dùng
(4, 9, 0, NULL, NULL, '2025-09-18 11:00:00'), -- user03 có FREESHIP chưa dùng
(5, 1, 0, NULL, NULL, '2025-09-19 12:00:00'), -- user04 có SALE10 chưa dùng
(6, 2, 0, NULL, NULL, '2025-09-20 13:00:00'), -- user05 có WELCOME50K chưa dùng
(6, 10, 0, NULL, NULL, '2025-09-21 14:00:00'); -- user05 có VIP500K chưa dùng
