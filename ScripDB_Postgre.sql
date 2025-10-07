DROP TABLE IF EXISTS 
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
    Users
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
   Bảng Products
================================ */
CREATE TABLE Products (
    ProductID SERIAL PRIMARY KEY,
    ProductName VARCHAR(100) NOT NULL,
    BriefDescription VARCHAR(255),
    FullDescription TEXT,
    TechnicalSpecifications TEXT,
    Price DECIMAL(18, 2) NOT NULL,
    ImageURL VARCHAR(255),
    CategoryID INT,
    BrandID INT,
    FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID),
    FOREIGN KEY (BrandID) REFERENCES Brands(BrandID)
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
('user04', 'hash_pw4', 'user04@gmail.com', '0909000005', '78 CMT8, Q3, HCM', 'Customer', 'https://example.com/avatars/user04.jpg'),
('user05', 'hash_pw5', 'user05@gmail.com', '0909000006', '90 Pasteur, Q1, HCM', 'Customer', 'https://example.com/avatars/user05.jpg');

/* ================================
   Insert Categories
================================ */
INSERT INTO Categories (CategoryName, ImageUrl)
VALUES
('Điện thoại', 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759834355/iphone17s-removebg-preview_ocdb5m.png'),
('Laptop', 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759834356/laptop-removebg-preview_jngmru.png'),
('Đồ gia dụng', 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759834357/dogiadung-removebg-preview_hjb22m.png'),
('Phụ kiện', 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759834355/PhukienRemove_pp3p1l.png');

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
INSERT INTO Products (ProductName, BriefDescription, FullDescription, TechnicalSpecifications, Price, ImageURL, CategoryID, BrandID)
VALUES
-- APPLE
('iPhone 15 Pro', 'Smartphone cao cấp', 'Điện thoại Apple iPhone 15 Pro 256GB', 'Chip A17 Pro, 6GB RAM, 256GB ROM', 28990000, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759834883/iphone-15-pro-xanh-halo_jet8zc.png', 1, 1),
('iPhone 14', 'Smartphone phổ thông', 'Điện thoại Apple iPhone 14 128GB', 'Chip A15 Bionic, 6GB RAM, 128GB ROM', 19990000, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759834883/iphone-14-pro-29-ff5703aa-730b-41fb-a818-3e4a2bd7c314-removebg-preview_ednzkc.png', 1, 1),
('MacBook Air M2', 'Laptop mỏng nhẹ', 'MacBook Air M2 2023', 'Apple M2, 16GB RAM, 512GB SSD', 31990000, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759834883/macbook_air_13_m2_midnight_1_35053fbcf9-removebg-preview_mvvl33.png', 2, 1),
('MacBook Pro 14', 'Laptop hiệu năng cao', 'MacBook Pro 14 inch M2 Pro', 'Apple M2 Pro, 32GB RAM, 1TB SSD', 52990000, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759834882/6fylky7q-138-macbook-pro-14-m1-pro-16gb-1tb-like-new-removebg-preview_tmio89.png', 2, 1),
('Apple Watch Ultra', 'Đồng hồ thông minh', 'Apple Watch Ultra 49mm', 'Chip S8, chống nước 100m', 19990000, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759834885/MT653_VW_34FRwatch-49-titanium-ultra2_VW_34FRwatch-face-49-ocean-ultra2_VW_34FR-scaled-removebg-preview_hyksbd.png', 4, 1),
('AirPods Pro 2', 'Tai nghe không dây', 'Apple AirPods Pro Gen 2', 'ANC, Adaptive Transparency', 5990000, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759834882/tai-nghe-bluetooth-airpods-pro-2nd-gen-usb-c-charge-apple-1-750x500-removebg-preview_rjtzpw.png', 4, 1),
-- SAMSUNG
('Samsung Galaxy S23', 'Android flagship', 'Samsung Galaxy S23 Ultra 256GB', 'Snapdragon 8 Gen 2, 12GB RAM, 256GB ROM', 25990000, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759835187/616xeaUEdbL._AC_SL1500_-removebg-preview_qnzv36.png', 1, 2),
('Samsung Galaxy A54', 'Smartphone tầm trung', 'Samsung Galaxy A54 5G 128GB', 'Exynos 1380, 8GB RAM, 128GB ROM', 9990000, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759835186/3d9a1a4ed9ed0496f0d1bc7e03b81e947e4d40ea-removebg-preview_uqhibl.png', 1, 2),
('Samsung Tủ lạnh Inverter', 'Tủ lạnh tiết kiệm điện', 'Tủ lạnh Samsung Inverter 450L', 'Ngăn đá trên, công nghệ Digital Inverter', 13990000, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759835186/tu-lanh-samsung-rf48a4010m9-sv-1_1654181912-removebg-preview_blkljw.png', 3, 2),
('Samsung Máy giặt AI', 'Máy giặt thông minh', 'Máy giặt Samsung AI Inverter 10kg', 'AI Control, Eco Bubble', 10990000, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759835264/qkl1637641932-removebg-preview_n8wuti.png', 3, 2),
('Samsung Galaxy Buds2 Pro', 'Tai nghe không dây', 'Samsung Galaxy Buds2 Pro', 'ANC, Bluetooth 5.3', 4990000, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759835187/Tai-nghe-Bluetooth-Samsung-Galaxy-Buds2-Pro-removebg-preview_rkmvfe.png', 4, 2),
-- XIAOMI
('Xiaomi 13 Pro', 'Flagship camera', 'Xiaomi 13 Pro 256GB', 'Snapdragon 8 Gen 2, Camera Leica', 18990000, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759835185/xiaomi-13-pro-9-removebg-preview_f8jc8t.png', 1, 3),
('Xiaomi Redmi Note 12', 'Giá rẻ hiệu năng cao', 'Redmi Note 12 128GB', 'Snapdragon 4 Gen 1, 6GB RAM, 128GB ROM', 5990000, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759835647/xiaomi-redmi-note-12-vang-1-thumb-momo-600x600-removebg-preview_h5v5kp.png', 1, 3),
('Xiaomi Robot Vacuum', 'Robot hút bụi', 'Xiaomi Robot Vacuum S10+', 'LDS Navigation, 4000Pa', 7990000, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759835751/0034208_xiaomi-robot-vacuum-e5_d6ubys.png', 3, 3),
('Xiaomi Buds 4 Pro', 'Tai nghe cao cấp', 'Xiaomi Buds 4 Pro', 'ANC, LHDC 5.0', 2990000, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759835645/xiaomi_buds_4_pro_5-removebg-preview_eddlnp.png', 4, 3),
-- DELL
('Dell XPS 13', 'Ultrabook cao cấp', 'Dell XPS 13 2023', 'Core i7, 16GB RAM, 512GB SSD', 38990000, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759835644/71okgs_gpel._ac_sl1496__6f9861a5e6764448abf1fb93c2f57015_master-removebg-preview_rmkhe3.png', 2, 4),
('Dell Inspiron 15', 'Laptop văn phòng', 'Dell Inspiron 15 3520', 'Core i5, 8GB RAM, 512GB SSD', 15990000, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759835643/71nP6lTogjL._AC_SL1500_-removebg-preview_r0hoyp.png', 2, 4),
-- LG
('LG Tủ lạnh Inverter', 'Tủ lạnh tiết kiệm điện', 'Tủ lạnh LG Inverter 420L', 'Ngăn đá trên, tiết kiệm điện A++', 11990000, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759835643/tu-lanh-sbs-lg-inverter-635l-gr-x257bg_d6a2ee2d-removebg-preview_qtrqjr.png', 3, 5),
('LG Máy lạnh Dual Inverter', 'Điều hòa cao cấp', 'Máy lạnh LG Dual Inverter 1.5HP', 'Turbo Cooling, tiết kiệm điện', 8990000, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759835642/lg-inverter-1-hp-v10win1-1-1-700x467-removebg-preview_hujngy.png', 3, 5),
-- SONY
('Sony WH-1000XM5', 'Tai nghe chống ồn', 'Sony WH-1000XM5 Over-ear', 'ANC, Pin 30h', 8990000, 'https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759835776/3123_tai_nghe_sony_wh_1000xm5_blue_songlongmedia-removebg-preview_old5bc.png', 4, 6);

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
('SALE10', 'Giảm 10% cho tất cả sản phẩm', 10.00, NULL, '2025-01-01', '2025-12-31', TRUE),
('WELCOME50K', 'Giảm 50k cho khách hàng mới', NULL, 50000, '2025-01-01', '2025-12-31', TRUE),
('FLASH100K', 'Flash sale giảm 100k', NULL, 100000, '2025-10-01', '2025-10-15', TRUE),
('LAPTOP15', 'Giảm 15% cho laptop', 15.00, NULL, '2025-01-01', '2025-06-30', TRUE),
('PHONE20', 'Giảm 20% cho điện thoại', 20.00, NULL, '2025-03-01', '2025-03-31', FALSE),
('NEWYEAR200K', 'Tết giảm 200k', NULL, 200000, '2025-01-20', '2025-02-10', TRUE),
('SUMMER25', 'Hè giảm 25%', 25.00, NULL, '2025-06-01', '2025-08-31', TRUE),
('BLACKFRIDAY30', 'Black Friday giảm 30%', 30.00, NULL, '2025-11-25', '2025-11-30', TRUE),
('FREESHIP', 'Miễn phí vận chuyển', NULL, 30000, '2025-01-01', '2025-12-31', TRUE),
('VIP500K', 'Giảm 500k cho VIP', NULL, 500000, '2025-01-01', '2025-12-31', TRUE);

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