DROP TABLE IF EXISTS
    ProductReviews,
    ProductImages,
    ProductVouchers,
    UserVouchers,
    Vouchers,
    Wishlists,
    StoreLocations,
    ChatMessages,
    Notifications,
    Payments,
    Orders,
    CartItems,
    Carts,
    Products,
    Brands,
    Categories,
    Users,
	UserDeviceTokens
CASCADE;

/* ================================
   Bảng Users
================================ */
CREATE TABLE Users (
    UserID SERIAL PRIMARY KEY,
    Username VARCHAR(50) NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    PhoneNumber VARCHAR(15),
    Address VARCHAR(255),
    Role VARCHAR(50) NOT NULL,
    AvatarUrl VARCHAR(255)
);

/* ================================
   Bảng Categories
================================ */
CREATE TABLE Categories (
    CategoryID SERIAL PRIMARY KEY,
    CategoryName VARCHAR(100) NOT NULL,
    ImageUrl VARCHAR(255)
);

/* ================================
   Bảng Brands
================================ */
CREATE TABLE Brands (
    BrandID SERIAL PRIMARY KEY,
    BrandName VARCHAR(100) NOT NULL,
    Description VARCHAR(255),
    LogoUrl VARCHAR(255)
);

/* ================================
   Bảng Products (ĐÃ CẬP NHẬT)
================================ */
CREATE TABLE Products (
    ProductID SERIAL PRIMARY KEY,
    ProductName VARCHAR(100) NOT NULL,
    BriefDescription VARCHAR(255),
    FullDescription TEXT,
    TechnicalSpecifications TEXT,
    Price DECIMAL(18, 2) NOT NULL,
    CategoryID INT,
    BrandID INT,
    -- Các trường mới được thêm vào
    Popularity INT NOT NULL DEFAULT 0,
    AverageRating DECIMAL(3, 2) NOT NULL DEFAULT 0.00,
    ReviewCount INT NOT NULL DEFAULT 0,
    FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID),
    FOREIGN KEY (BrandID) REFERENCES Brands(BrandID)
);

/* ================================
   Bảng ProductImages (MỚI)
================================ */
CREATE TABLE ProductImages (
    ImageID SERIAL PRIMARY KEY,
    ProductID INT NOT NULL,
    ImageUrl VARCHAR(255) NOT NULL,
    IsPrimary BOOLEAN NOT NULL DEFAULT FALSE,
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID) ON DELETE CASCADE
);

/* ================================
   Bảng ProductReviews (MỚI)
================================ */
CREATE TABLE ProductReviews (
    ReviewID SERIAL PRIMARY KEY,
    ProductID INT NOT NULL,
    UserID INT NOT NULL,
    Rating SMALLINT NOT NULL CHECK (Rating >= 1 AND Rating <= 5),
    Comment TEXT,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID) ON DELETE CASCADE,
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE SET NULL,
    CONSTRAINT UQ_User_Product_Review UNIQUE(UserID, ProductID)
);

/* ================================
   Bảng Carts
================================ */
CREATE TABLE Carts (
    CartID SERIAL PRIMARY KEY,
    UserID INT,
    TotalPrice DECIMAL(18, 2) NOT NULL,
    Status VARCHAR(50) NOT NULL,
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

/* ================================
   Bảng CartItems
================================ */
CREATE TABLE CartItems (
    CartItemID SERIAL PRIMARY KEY,
    CartID INT,
    ProductID INT,
    Quantity INT NOT NULL,
    Price DECIMAL(18, 2) NOT NULL,
    FOREIGN KEY (CartID) REFERENCES Carts(CartID),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);

/* ================================
   Bảng Orders
================================ */
CREATE TABLE Orders (
    OrderID SERIAL PRIMARY KEY,
    CartID INT,
    UserID INT,
    PaymentMethod VARCHAR(50) NOT NULL,
    BillingAddress VARCHAR(255) NOT NULL,
    OrderStatus VARCHAR(50) NOT NULL,
    OrderDate TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (CartID) REFERENCES Carts(CartID),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

/* ================================
   Bảng Payments
================================ */
CREATE TABLE Payments (
    PaymentID SERIAL PRIMARY KEY,
    OrderID INT,
    Amount DECIMAL(18, 2) NOT NULL,
    PaymentDate TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PaymentStatus VARCHAR(50) NOT NULL,
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID)
);

/* ================================
   Bảng Notifications
================================ */
CREATE TABLE Notifications (
    NotificationID SERIAL PRIMARY KEY,
    UserID INT,
    Message VARCHAR(255),
    IsRead BOOLEAN NOT NULL DEFAULT FALSE,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

/* ================================
   Bảng ChatMessages
================================ */
CREATE TABLE ChatMessages (
    ChatMessageID SERIAL PRIMARY KEY,
    SenderID INT NOT NULL,
    ReceiverID INT NOT NULL,
    Message TEXT,
    SentAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (SenderID) REFERENCES Users(UserID),
    FOREIGN KEY (ReceiverID) REFERENCES Users(UserID)
);

/* ================================
   Bảng StoreLocations
================================ */
CREATE TABLE StoreLocations (
    LocationID SERIAL PRIMARY KEY,
    Latitude DECIMAL(9, 6) NOT NULL,
    Longitude DECIMAL(9, 6) NOT NULL,
    Address VARCHAR(255) NOT NULL
);

/* ================================
   Bảng Wishlist
================================ */
CREATE TABLE Wishlists (
    WishlistID SERIAL PRIMARY KEY,
    UserID INT NOT NULL,
    ProductID INT NOT NULL,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID),
    CONSTRAINT UQ_Wishlist UNIQUE(UserID, ProductID)
);

/* ================================
   Bảng Vouchers
================================ */
CREATE TABLE Vouchers (
    VoucherID SERIAL PRIMARY KEY,
    Code VARCHAR(50) NOT NULL UNIQUE,
    Description VARCHAR(255),
    DiscountPercent DECIMAL(5,2),
    DiscountAmount DECIMAL(18,2),
    StartDate TIMESTAMP NOT NULL,
    EndDate TIMESTAMP NOT NULL,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);

/* ================================
   Bảng UserVouchers (User được phát voucher)
================================ */
CREATE TABLE UserVouchers (
    UserVoucherID SERIAL PRIMARY KEY,
    UserID INT NOT NULL,
    VoucherID INT NOT NULL,
    IsUsed BOOLEAN NOT NULL DEFAULT FALSE,
    UsedAt TIMESTAMP NULL,
    OrderID INT NULL,
    AssignedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (VoucherID) REFERENCES Vouchers(VoucherID),
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID),
    CONSTRAINT UQ_UserVoucher UNIQUE(UserID, VoucherID)
);

/* ================================
   Bảng ProductVouchers (voucher áp dụng cho product)
================================ */
CREATE TABLE ProductVouchers (
    ProductVoucherID SERIAL PRIMARY KEY,
    VoucherID INT NOT NULL,
    ProductID INT NOT NULL,
    FOREIGN KEY (VoucherID) REFERENCES Vouchers(VoucherID),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID),
    CONSTRAINT UQ_ProductVoucher UNIQUE(VoucherID, ProductID)
);
CREATE TABLE UserDeviceTokens (
    TokenID SERIAL PRIMARY KEY,
    UserID INT NOT NULL,
    FCMToken VARCHAR(500) NOT NULL,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE,
    LastUpdatedDate TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
);

/* ================================
   Insert StoreLocations
================================ */
INSERT INTO StoreLocations (Latitude, Longitude, Address)
VALUES (10.762622, 106.660172, '268 Lý Thường Kiệt, Quận 10, TP.HCM');

/* ================================
   Insert Users (1 Admin + 5 Customers = 6 users)
================================ */
INSERT INTO Users (Username, PasswordHash, Email, PhoneNumber, Address, Role, AvatarUrl)
VALUES
('hau1310', 'WtuvvAnc0apQ8UmR65nUsPqxqYdLiQIvsd83qj6fsPA=', 'ngochau1310@gmail.com', '0333006947', '268 Lý Thường Kiệt, Q10, HCM', 'Admin', 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759833475/Hau_r3dqvg.jpg'),
('nhan123', 'NBh1sys/kDeD+4Dd/riCFlNxGOKyLjXBmv4x823w3vs=', 'nhannbse183352@fpt.edu.vn', '0333984762', '12 Lê Lợi, Q1, HCM', 'Customer', 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759833606/fotor-face-swap-20251003223459_velc22.jpg'),
('huy123', 'NBh1sys/kDeD+4Dd/riCFlNxGOKyLjXBmv4x823w3vs=', 'huynqse183261@fpt.edu.vn', '0909000003', '34 Hai Bà Trưng, Q1, HCM', 'Customer', 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759833818/image-4_rfw4da.png'),
('quoc123', 'NBh1sys/kDeD+4Dd/riCFlNxGOKyLjXBmv4x823w3vs=', 'quocthkse183295@fpt.edu.vn', '0909000004', '56 Nguyễn Trãi, Q5, HCM', 'Customer', 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759833818/image_auvknc.png'),
('user04', 'NBh1sys/kDeD+4Dd/riCFlNxGOKyLjXBmv4x823w3vs=', 'user04@gmail.com', '0909000005', '78 CMT8, Q3, HCM', 'Customer', 'https://example.com/avatars/user04.jpg'),
('user05', 'NBh1sys/kDeD+4Dd/riCFlNxGOKyLjXBmv4x823w3vs=', 'user05@gmail.com', '0909000006', '90 Pasteur, Q1, HCM', 'Customer', 'https://example.com/avatars/user05.jpg');

/* ================================
   Insert Categories
================================ */
INSERT INTO Categories (CategoryName, ImageUrl)
VALUES
('Điện thoại', 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759834355/iphone17s-removebg-preview_ocdb5m.png'),
('Laptop', 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759840893/laptop-removebg-preview_1_wu51sl.png'),
('Đồ gia dụng', 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759840825/dogiadung-removebg-preview_1_thydjm.png'),
('Phụ kiện', 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759840873/PhukienRemove_1_mrv6ey.png');

/* ================================
   Insert Brands (6 brands)
================================ */
INSERT INTO Brands (BrandName, Description, LogoUrl)
VALUES
('Apple', 'Thương hiệu công nghệ hàng đầu thế giới', 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759834575/8ed3d547-94ff-48e1-9f20-8c14a7030a02_2000x2000_wla77t.jpg'),
('Samsung', 'Tập đoàn điện tử Hàn Quốc', 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759834575/SAMSUNG_oujj7x.avif'),
('Xiaomi', 'Thương hiệu điện tử Trung Quốc', 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759834575/Xiaomi_logo__2021-.svg_harbst.png'),
('Dell', 'Thương hiệu máy tính Mỹ', 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759834574/Dell_Logo.svg_uvnbsv.png'),
('LG', 'Tập đoàn điện tử Hàn Quốc', 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759834574/LG_symbol.svg_pctfqc.png'),
('Sony', 'Tập đoàn giải trí và điện tử Nhật Bản', 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759834574/sony-logo-sony-icon-transparent-free_vvjcck.webp');

/* ================================
   Insert Products (20 sản phẩm)
================================ */
INSERT INTO Products (ProductName, BriefDescription, FullDescription, TechnicalSpecifications, Price, CategoryID, BrandID)
VALUES
-- APPLE (ID: 1-6)
('iPhone 15 Pro', 'Smartphone cao cấp', 'Điện thoại Apple iPhone 15 Pro 256GB', 'Chip A17 Pro, 6GB RAM, 256GB ROM', 28990000, 1, 1),
('iPhone 14', 'Smartphone phổ thông', 'Điện thoại Apple iPhone 14 128GB', 'Chip A15 Bionic, 6GB RAM, 128GB ROM', 19990000, 1, 1),
('MacBook Air M2', 'Laptop mỏng nhẹ', 'MacBook Air M2 2023', 'Apple M2, 16GB RAM, 512GB SSD', 31990000, 2, 1),
('MacBook Pro 14', 'Laptop hiệu năng cao', 'MacBook Pro 14 inch M2 Pro', 'Apple M2 Pro, 32GB RAM, 1TB SSD', 52990000, 2, 1),
('Apple Watch Ultra', 'Đồng hồ thông minh', 'Apple Watch Ultra 49mm', 'Chip S8, chống nước 100m', 19990000, 4, 1),
('AirPods Pro 2', 'Tai nghe không dây', 'Apple AirPods Pro Gen 2', 'ANC, Adaptive Transparency', 5990000, 4, 1),
-- SAMSUNG (ID: 7-11)
('Samsung Galaxy S23', 'Android flagship', 'Samsung Galaxy S23 Ultra 256GB', 'Snapdragon 8 Gen 2, 12GB RAM, 256GB ROM', 25990000, 1, 2),
('Samsung Galaxy A54', 'Smartphone tầm trung', 'Samsung Galaxy A54 5G 128GB', 'Exynos 1380, 8GB RAM, 128GB ROM', 9990000, 1, 2),
('Samsung Tủ lạnh Inverter', 'Tủ lạnh tiết kiệm điện', 'Tủ lạnh Samsung Inverter 450L', 'Ngăn đá trên, công nghệ Digital Inverter', 13990000, 3, 2),
('Samsung Máy giặt AI', 'Máy giặt thông minh', 'Máy giặt Samsung AI Inverter 10kg', 'AI Control, Eco Bubble', 10990000, 3, 2),
('Samsung Galaxy Buds2 Pro', 'Tai nghe không dây', 'Samsung Galaxy Buds2 Pro', 'ANC, Bluetooth 5.3', 4990000, 4, 2),
-- XIAOMI (ID: 12-15)
('Xiaomi 13 Pro', 'Flagship camera', 'Xiaomi 13 Pro 256GB', 'Snapdragon 8 Gen 2, Camera Leica', 18990000, 1, 3),
('Xiaomi Redmi Note 12', 'Giá rẻ hiệu năng cao', 'Redmi Note 12 128GB', 'Snapdragon 4 Gen 1, 6GB RAM, 128GB ROM', 5990000, 1, 3),
('Xiaomi Robot Vacuum', 'Robot hút bụi', 'Xiaomi Robot Vacuum S10+', 'LDS Navigation, 4000Pa', 7990000, 3, 3),
('Xiaomi Buds 4 Pro', 'Tai nghe cao cấp', 'Xiaomi Buds 4 Pro', 'ANC, LHDC 5.0', 2990000, 4, 3),
-- DELL (ID: 16-17)
('Dell XPS 13', 'Ultrabook cao cấp', 'Dell XPS 13 2023', 'Core i7, 16GB RAM, 512GB SSD', 38990000, 2, 4),
('Dell Inspiron 15', 'Laptop văn phòng', 'Dell Inspiron 15 3520', 'Core i5, 8GB RAM, 512GB SSD', 15990000, 2, 4),
-- LG (ID: 18-19)
('LG Tủ lạnh Inverter', 'Tủ lạnh tiết kiệm điện', 'Tủ lạnh LG Inverter 420L', 'Ngăn đá trên, tiết kiệm điện A++', 11990000, 3, 5),
('LG Máy lạnh Dual Inverter', 'Điều hòa cao cấp', 'Máy lạnh LG Dual Inverter 1.5HP', 'Turbo Cooling, tiết kiệm điện', 8990000, 3, 5),
-- SONY (ID: 20)
('Sony WH-1000XM5', 'Tai nghe chống ồn', 'Sony WH-1000XM5 Over-ear', 'ANC, Pin 30h', 8990000, 4, 6);

/* ================================
   Insert ProductImages (DỮ LIỆU MỚI)
================================ */
INSERT INTO ProductImages (ProductID, ImageUrl, IsPrimary)
VALUES
-- Product 1: iPhone 15 Pro (có 2 ảnh)
(1, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759834883/iphone-15-pro-xanh-halo_jet8zc.png', TRUE),
(1, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1760068101/image-removebg-preview_2_f5h2ur.png', FALSE),
(1, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1760068102/image-removebg-preview_1_vaeu9d.png', FALSE),
(1, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1760068100/image-removebg-preview_gsgenn.png', FALSE),

-- Product 2: iPhone 14
(2, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759834883/iphone-14-pro-29-ff5703aa-730b-41fb-a818-3e4a2bd7c314-removebg-preview_ednzkc.png', TRUE),
(2, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1760068100/14trang-600x600-1_pl9sgt.png', FALSE),
(2, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1760068101/image-removebg-preview_3_ds7elz.png', FALSE),
(2, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1760068101/14do-600x600-1_oafiwm.png', FALSE),
(2, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1760068100/image-removebg-preview_4_nt31ia.png', FALSE),
(2, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1760068100/14xam-600x600-1_kwmafx.png', FALSE),

-- Product 3: MacBook Air M2
(3, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759834883/macbook_air_13_m2_midnight_1_35053fbcf9-removebg-preview_mvvl33.png', TRUE),
(3, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1760068101/image-removebg-preview_5_qm0rg4.png', FALSE),

-- Product 4: MacBook Pro 14
(4, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759834882/6fylky7q-138-macbook-pro-14-m1-pro-16gb-1tb-like-new-removebg-preview_tmio89.png', TRUE),
(4, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1760068100/image-removebg-preview_6_fla5xs.png', FALSE),

-- Product 5: Apple Watch Ultra
(5, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759834885/MT653_VW_34FRwatch-49-titanium-ultra2_VW_34FRwatch-face-49-ocean-ultra2_VW_34FR-scaled-removebg-preview_hyksbd.png', TRUE),
(5, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1760068365/image-removebg-preview_8_t311ge.png', FALSE),
(5, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1760068366/image-removebg-preview_7_ivvpep.png', FALSE),
(5, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1760068366/image-removebg-preview_9_dtva5r.png', FALSE),

-- Product 6: AirPods Pro 2
(6, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759834882/tai-nghe-bluetooth-airpods-pro-2nd-gen-usb-c-charge-apple-1-750x500-removebg-preview_rjtzpw.png', TRUE),
-- Product 7: Samsung Galaxy S23
(7, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759835187/616xeaUEdbL._AC_SL1500_-removebg-preview_qnzv36.png', TRUE),
(7, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1760069959/image-removebg-preview_12_zbctw6.png', FALSE),
(7, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1760069960/image-removebg-preview_10_x71jpr.png', FALSE),
(7, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1760069960/image-removebg-preview_11_wdizhh.png', FALSE),


-- Product 8: Samsung Galaxy A54
(8, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759835186/3d9a1a4ed9ed0496f0d1bc7e03b81e947e4d40ea-removebg-preview_uqhibl.png', TRUE),
(8, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1760070101/image-removebg-preview_13_jty4es.png', FALSE),
(8, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1760070100/image-removebg-preview_14_mcplsr.png', FALSE),

-- Product 9: Samsung Tủ lạnh Inverter
(9, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759835186/tu-lanh-samsung-rf48a4010m9-sv-1_1654181912-removebg-preview_blkljw.png', TRUE),
(9, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1760070192/image-removebg-preview_15_dyjyro.png', FALSE),

-- Product 10: Samsung Máy giặt AI
(10, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759835264/qkl1637641932-removebg-preview_n8wuti.png', TRUE),
(10, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1760070265/image-removebg-preview_16_sodpse.png', FALSE),

-- Product 11: Samsung Galaxy Buds2 Pro
(11, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759835187/Tai-nghe-Bluetooth-Samsung-Galaxy-Buds2-Pro-removebg-preview_rkmvfe.png', TRUE),
(11, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1760070707/image-removebg-preview_17_w0ihap.png', FALSE),

-- Product 12: Xiaomi 13 Pro
(12, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759835185/xiaomi-13-pro-9-removebg-preview_f8jc8t.png', TRUE),
(12, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1760070856/image-removebg-preview_18_j0b2vp.png', FALSE),
(12, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1760070856/image-removebg-preview_19_unbhl6.png', FALSE),
(12, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1760070857/image-removebg-preview_20_gkmpgs.png', FALSE),

-- Product 13: Xiaomi Redmi Note 12
(13, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759835647/xiaomi-redmi-note-12-vang-1-thumb-momo-600x600-removebg-preview_h5v5kp.png', TRUE),
(13, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1760071041/image-removebg-preview_23_qsevsj.png', FALSE),
(13, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1760071042/image-removebg-preview_21_je78wj.png', FALSE),
(13, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1760071044/image-removebg-preview_22_nbae0d.png', FALSE),


-- Product 14: Xiaomi Robot Vacuum
(14, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759835751/0034208_xiaomi-robot-vacuum-e5_d6ubys.png', TRUE),
(14, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1760071171/image-removebg-preview_24_xojqho.png', FALSE),

-- Product 15: Xiaomi Buds 4 Pro
(15, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759835645/xiaomi_buds_4_pro_5-removebg-preview_eddlnp.png', TRUE),
(15, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1760071229/image-removebg-preview_25_eertah.png', FALSE),

-- Product 16: Dell XPS 13
(16, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759835644/71okgs_gpel._ac_sl1496__6f9861a5e6764448abf1fb93c2f57015_master-removebg-preview_rmkhe3.png', TRUE),
(16, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1760071430/image-removebg-preview_26_jnpdjo.png', FALSE),
(16, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1760071432/image-removebg-preview_27_tvnww2.png', FALSE),
(16, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1760071432/image-removebg-preview_28_ejmvpo.png', FALSE),

-- Product 17: Dell Inspiron 15
(17, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759835643/71nP6lTogjL._AC_SL1500_-removebg-preview_r0hoyp.png', TRUE),
(17, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1760071509/image-removebg-preview_29_td170t.png', FALSE),

-- Product 18: LG Tủ lạnh Inverter
(18, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759835643/tu-lanh-sbs-lg-inverter-635l-gr-x257bg_d6a2ee2d-removebg-preview_qtrqjr.png', TRUE),
(18, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1760071562/image-removebg-preview_30_fmqbzp.png', FALSE),

-- Product 19: LG Máy lạnh Dual Inverter
(19, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759835642/lg-inverter-1-hp-v10win1-1-1-700x467-removebg-preview_hujngy.png', TRUE),
(19, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1760071708/image-removebg-preview_32_kp3zgf.png', FALSE),

-- Product 20: Sony WH-1000XM5
(20, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759835776/3123_tai_nghe_sony_wh_1000xm5_blue_songlongmedia-removebg-preview_old5bc.png', TRUE),
(20, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1760071808/image-removebg-preview_31_zmpd4y.png', FALSE),
(20, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1760071810/image-removebg-preview_34_vthj89.png', FALSE),
(20, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1760071812/image-removebg-preview_33_szttxg.png', FALSE);


/* ================================
   Insert Carts
================================ */
INSERT INTO Carts (UserID, TotalPrice, Status)
VALUES
(2, 28990000, 'Active'),
(3, 25990000, 'Completed'),
(4, 52990000, 'Completed'),
(5, 11990000, 'Completed'),
(6, 8990000, 'Active'),
(3, 0, 'Active'),
(1, 0, 'Active'),
(5, 0, 'Active'),
(6, 0, 'Active'),
(2, 19990000, 'Completed');

/* ================================
   Insert CartItems
================================ */
INSERT INTO CartItems (CartID, ProductID, Quantity, Price)
VALUES
(1, 1, 1, 28990000),
(2, 7, 1, 25990000),
(3, 4, 1, 52990000),
(4, 18, 1, 11990000),
(5, 20, 1, 8990000),
(6, 2, 1, 19990000);

/* ================================
   Insert Orders
================================ */
INSERT INTO Orders (CartID, UserID, PaymentMethod, BillingAddress, OrderStatus, OrderDate)
VALUES
(2, 3, 'Credit Card', '34 Hai Bà Trưng, Q1, HCM', 'Success', '2025-09-15 10:30:00'),
(3, 4, 'COD', '56 Nguyễn Trãi, Q5, HCM', 'Success', '2025-09-16 14:20:00'),
(4, 5, 'Bank Transfer', '78 CMT8, Q3, HCM', 'Success', '2025-09-17 09:15:00'),
(6, 2, 'Momo', '12 Lê Lợi, Q1, HCM', 'Success', '2025-09-18 11:45:00'),
(2, 3, 'COD', '34 Hai Bà Trưng, Q1, HCM', 'Pending', '2025-09-19 16:30:00');

/* ================================
   Insert ProductReviews (DỮ LIỆU MỚI)
================================ */
INSERT INTO ProductReviews(UserID, ProductID, Rating, Comment)
VALUES
(3, 7, 5, 'Sản phẩm tuyệt vời, camera chụp ảnh rất đẹp! Giao hàng nhanh.'),
(4, 4, 4, 'Máy mạnh, dùng cho công việc rất tốt, hơi nóng một chút.'),
(5, 18, 5, 'Tủ lạnh chạy êm, tiết kiệm điện, rất đáng tiền.'),
(2, 2, 4, 'Điện thoại dùng mượt, pin ổn trong tầm giá.'),
(2, 7, 4, 'Mua cái thứ 2 cho người nhà. Máy chính hãng, hài lòng.');


/* ================================
   Insert Payments
================================ */
INSERT INTO Payments (OrderID, Amount, PaymentStatus, PaymentDate)
VALUES
(1, 25990000, 'Paid', '2025-09-15 10:35:00'),
(2, 52990000, 'Paid', '2025-09-16 14:25:00'),
(3, 11990000, 'Paid', '2025-09-17 09:20:00'),
(4, 19990000, 'Paid', '2025-09-18 11:50:00'),
(5, 25990000, 'Pending', '2025-09-19 16:35:00');

/* ================================
   Insert Notifications
================================ */
INSERT INTO Notifications (UserID, Message, IsRead, CreatedAt)
VALUES
(2, 'Đơn hàng của bạn đã được xác nhận.', FALSE, '2025-09-15 10:30:00'),
(3, 'Thanh toán thành công đơn hàng.', TRUE, '2025-09-16 14:20:00'),
(4, 'Giỏ hàng của bạn còn sản phẩm chưa thanh toán.', FALSE, '2025-09-17 09:15:00'),
(5, 'Đơn hàng #3 đang được giao.', TRUE, '2025-09-18 11:45:00'),
(6, 'Bạn đã thêm sản phẩm mới vào giỏ.', FALSE, '2025-09-19 16:30:00'),
(2, 'Thanh toán đơn hàng #4 thành công.', TRUE, '2025-09-20 13:10:00'),
(3, 'Đơn hàng #5 đang chờ xác nhận.', FALSE, '2025-09-21 10:00:00'),
(4, 'Bạn nhận được voucher giảm giá 10%.', FALSE, '2025-09-22 15:25:00'),
(5, 'Sản phẩm yêu thích của bạn đang giảm giá.', FALSE, '2025-09-23 12:40:00'),
(6, 'Chúc mừng bạn nhận voucher 100k.', FALSE, '2025-09-24 09:50:00');

/* ================================
   Insert ChatMessages
================================ */
INSERT INTO ChatMessages (SenderID, ReceiverID, Message, SentAt)
VALUES
(2, 1, 'Tôi muốn hỏi về iPhone 15 Pro còn hàng không?', '2025-09-15 10:00:00'),
(1, 2, 'Dạ vẫn còn hàng ạ. Anh/chị cần thêm thông tin gì không?', '2025-09-15 10:05:00'),
(3, 1, 'Xin báo giá chi tiết Samsung S23.', '2025-09-16 11:30:00'),
(1, 3, 'Samsung S23 hiện tại giá 25.990.000đ, bảo hành 12 tháng.', '2025-09-16 11:35:00'),
(4, 1, 'MacBook Pro 14 có giao hàng về Bình Dương không?', '2025-09-17 14:20:00'),
(1, 4, 'Shop có giao hàng toàn quốc ạ, Bình Dương được giao trong 1-2 ngày.', '2025-09-17 14:25:00'),
(5, 1, 'Tủ lạnh LG có bảo hành bao lâu?', '2025-09-18 09:15:00'),
(1, 5, 'Tủ lạnh LG bảo hành 12 tháng chính hãng ạ.', '2025-09-18 09:20:00'),
(6, 1, 'Sony WH-1000XM5 có app điều khiển không?', '2025-09-19 15:40:00'),
(1, 6, 'Có ạ, tai nghe có app Sony Headphones Connect để tùy chỉnh âm thanh.', '2025-09-19 15:45:00');

/* ================================
   Insert Wishlists
================================ */
INSERT INTO Wishlists (UserID, ProductID, CreatedAt)
VALUES
(2, 1, '2025-09-10 10:00:00'),
(2, 3, '2025-09-11 14:30:00'),
(3, 7, '2025-09-12 09:15:00'),
(3, 5, '2025-09-12 16:45:00'),
(4, 4, '2025-09-13 11:20:00'),
(4, 16, '2025-09-14 13:50:00'),
(5, 18, '2025-09-15 10:30:00'),
(5, 9, '2025-09-16 15:10:00'),
(6, 20, '2025-09-17 12:40:00'),
(6, 6, '2025-09-18 09:55:00');

/* ================================
   Insert Vouchers
================================ */
INSERT INTO Vouchers (Code, Description, DiscountPercent, DiscountAmount, StartDate, EndDate, IsActive)
VALUES
('SALE10', 'Giảm 10% cho tất cả sản phẩm', 10.00, NULL, '2025-01-01', '2026-12-31', TRUE),
('WELCOME50K', 'Giảm 50k cho khách hàng mới', NULL, 50000, '2025-01-01', '2026-12-31', TRUE),
('FLASH100K', 'Flash sale giảm 100k', NULL, 100000, '2025-10-01', '2026-10-15', TRUE),
('LAPTOP15', 'Giảm 15% cho laptop', 15.00, NULL, '2025-01-01', '2026-06-30', TRUE),
('PHONE20', 'Giảm 20% cho điện thoại', 20.00, NULL, '2025-03-01', '2026-03-31', FALSE),
('NEWYEAR200K', 'Tết giảm 200k', NULL, 200000, '2025-01-20', '2026-02-10', TRUE),
('SUMMER25', 'Hè giảm 25%', 25.00, NULL, '2025-06-01', '2026-08-31', TRUE),
('BLACKFRIDAY30', 'Black Friday giảm 30%', 30.00, NULL, '2026-11-25', '2026-11-30', TRUE),
('FREESHIP', 'Miễn phí vận chuyển', NULL, 30000, '2025-01-01', '2026-12-31', TRUE),
('VIP500K', 'Giảm 500k cho VIP', NULL, 500000, '2025-01-01', '2026-12-31', TRUE);

/* ================================
   Insert ProductVouchers
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

/* ================================
   Insert UserVouchers
================================ */
INSERT INTO UserVouchers (UserID, VoucherID, IsUsed, UsedAt, OrderID, AssignedAt)
VALUES
-- User đã sử dụng voucher
(3, 1, TRUE, '2025-09-15 10:30:00', 1, '2025-09-10 09:00:00'),
(4, 2, TRUE, '2025-09-16 14:20:00', 2, '2025-09-11 10:00:00'),
(5, 4, TRUE, '2025-09-17 09:15:00', 3, '2025-09-12 11:00:00'),
-- User chưa sử dụng voucher
(2, 1, FALSE, NULL, NULL, '2025-09-15 08:00:00'),
(2, 9, FALSE, NULL, NULL, '2025-09-16 09:00:00'),
(3, 3, FALSE, NULL, NULL, '2025-09-17 10:00:00'),
(4, 9, FALSE, NULL, NULL, '2025-09-18 11:00:00'),
(5, 1, FALSE, NULL, NULL, '2025-09-19 12:00:00'),
(6, 2, FALSE, NULL, NULL, '2025-09-20 13:00:00'),
(6, 10, FALSE, NULL, NULL, '2025-09-21 14:00:00');


/* Cập nhật AverageRating và ReviewCount cho các sản phẩm có review */
UPDATE Products SET AverageRating = 4.50, ReviewCount = 2 WHERE ProductID = 7; -- (5+4)/2
UPDATE Products SET AverageRating = 4.00, ReviewCount = 1 WHERE ProductID = 4;
UPDATE Products SET AverageRating = 5.00, ReviewCount = 1 WHERE ProductID = 18;
UPDATE Products SET AverageRating = 4.00, ReviewCount = 1 WHERE ProductID = 2;

/* Cập nhật Popularity cho một số sản phẩm hot */
UPDATE Products SET Popularity = 1500 WHERE ProductID = 1; -- iPhone 15 Pro
UPDATE Products SET Popularity = 1250 WHERE ProductID = 7; -- Samsung S23
UPDATE Products SET Popularity = 980 WHERE ProductID = 4; -- MacBook Pro 14
UPDATE Products SET Popularity = 850 WHERE ProductID = 3; -- MacBook Air M2
UPDATE Products SET Popularity = 700 WHERE ProductID = 20; -- Sony WH-1000XM5