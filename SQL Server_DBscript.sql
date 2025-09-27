USE master;
GO
ALTER DATABASE SalesAppDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
DROP DATABASE SalesAppDB;
GO

CREATE DATABASE SalesAppDB;
GO


USE SalesAppDB;
GO



-- Tạo bảng User
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    PhoneNumber NVARCHAR(15),
    Address NVARCHAR(255),
    Role NVARCHAR(50) NOT NULL
);
GO

-- Tạo bảng Category
CREATE TABLE Categories (
    CategoryID INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(100) NOT NULL
);
GO

-- Tạo bảng Product
CREATE TABLE Products (
    ProductID INT PRIMARY KEY IDENTITY(1,1),
    ProductName NVARCHAR(100) NOT NULL,
    BriefDescription NVARCHAR(255),
    FullDescription NVARCHAR(MAX),
    TechnicalSpecifications NVARCHAR(MAX),
    Price DECIMAL(18, 2) NOT NULL,
    ImageURL NVARCHAR(255),
    CategoryID INT,
    FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID)
);
GO

-- Tạo bảng Cart
CREATE TABLE Carts (
    CartID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT,
    TotalPrice DECIMAL(18, 2) NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);
GO

-- Tạo bảng CartItem
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

-- Tạo bảng Order
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

-- Tạo bảng Payment
CREATE TABLE Payments (
    PaymentID INT PRIMARY KEY IDENTITY(1,1),
    OrderID INT,
    Amount DECIMAL(18, 2) NOT NULL,
    PaymentDate DATETIME NOT NULL DEFAULT GETDATE(),
    PaymentStatus NVARCHAR(50) NOT NULL,
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID)
);
GO

-- Tạo bảng Notification
CREATE TABLE Notifications (
    NotificationID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT,
    Message NVARCHAR(255),
    IsRead BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);
GO

-- Tạo bảng ChatMessage
CREATE TABLE ChatMessages (
    ChatMessageID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT,
    Message NVARCHAR(MAX),
    SentAt DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);
GO

-- Tạo bảng StoreLocation
CREATE TABLE StoreLocations (
    LocationID INT PRIMARY KEY IDENTITY(1,1),
    Latitude DECIMAL(9, 6) NOT NULL,
    Longitude DECIMAL(9, 6) NOT NULL,
    Address NVARCHAR(255) NOT NULL
);
GO

-- Insert StoreLocations (chỉ 1 cửa hàng duy nhất)
INSERT INTO StoreLocations (Latitude, Longitude, Address)
VALUES (10.762622, 106.660172, N'268 Lý Thường Kiệt, Quận 10, TP.HCM');
GO

-- Insert Users (20 users, gồm cả Admin và Customer)
INSERT INTO Users (Username, PasswordHash, Email, PhoneNumber, Address, Role)
VALUES
(N'admin', 'hash_admin', N'admin@salesapp.com', '0909000001', N'268 Lý Thường Kiệt, Q10, HCM', N'Admin'),
(N'user01', 'hash_pw1', N'user01@gmail.com', '0909000002', N'12 Lê Lợi, Q1, HCM', N'Customer'),
(N'user02', 'hash_pw2', N'user02@gmail.com', '0909000003', N'34 Hai Bà Trưng, Q1, HCM', N'Customer'),
(N'user03', 'hash_pw3', N'user03@gmail.com', '0909000004', N'56 Nguyễn Trãi, Q5, HCM', N'Customer'),
(N'user04', 'hash_pw4', N'user04@gmail.com', '0909000005', N'78 CMT8, Q3, HCM', N'Customer'),
(N'user05', 'hash_pw5', N'user05@gmail.com', '0909000006', N'90 Pasteur, Q1, HCM', N'Customer'),
(N'user06', 'hash_pw6', N'user06@gmail.com', '0909000007', N'22 Nguyễn Huệ, Q1, HCM', N'Customer'),
(N'user07', 'hash_pw7', N'user07@gmail.com', '0909000008', N'15 Điện Biên Phủ, Q1, HCM', N'Customer'),
(N'user08', 'hash_pw8', N'user08@gmail.com', '0909000009', N'45 Lý Tự Trọng, Q1, HCM', N'Customer'),
(N'user09', 'hash_pw9', N'user09@gmail.com', '0909000010', N'67 Võ Văn Tần, Q3, HCM', N'Customer'),
(N'user10', 'hash_pw10', N'user10@gmail.com', '0909000011', N'89 Lý Chính Thắng, Q3, HCM', N'Customer'),
(N'user11', 'hash_pw11', N'user11@gmail.com', '0909000012', N'12 Nguyễn Văn Cừ, Q5, HCM', N'Customer'),
(N'user12', 'hash_pw12', N'user12@gmail.com', '0909000013', N'33 Tôn Đức Thắng, Q1, HCM', N'Customer'),
(N'user13', 'hash_pw13', N'user13@gmail.com', '0909000014', N'55 Trần Hưng Đạo, Q5, HCM', N'Customer'),
(N'user14', 'hash_pw14', N'user14@gmail.com', '0909000015', N'77 Nguyễn Thị Minh Khai, Q1, HCM', N'Customer'),
(N'user15', 'hash_pw15', N'user15@gmail.com', '0909000016', N'99 Võ Thị Sáu, Q3, HCM', N'Customer'),
(N'user16', 'hash_pw16', N'user16@gmail.com', '0909000017', N'12 Cách Mạng Tháng 8, Q10, HCM', N'Customer'),
(N'user17', 'hash_pw17', N'user17@gmail.com', '0909000018', N'21 Hồng Bàng, Q5, HCM', N'Customer'),
(N'user18', 'hash_pw18', N'user18@gmail.com', '0909000019', N'30 Nguyễn Tri Phương, Q10, HCM', N'Customer'),
(N'user19', 'hash_pw19', N'user19@gmail.com', '0909000020', N'40 Hoàng Văn Thụ, Q.Tân Bình, HCM', N'Customer');
GO

-- Insert Categories (ít thôi, khoảng 4)
INSERT INTO Categories (CategoryName)
VALUES
(N'Điện thoại'),
(N'Laptop'),
(N'Thiết bị gia dụng'),
(N'Phụ kiện');
GO

-- Insert Products (20 sản phẩm thực tế, thuộc nhiều category)
INSERT INTO Products (ProductName, BriefDescription, FullDescription, TechnicalSpecifications, Price, ImageURL, CategoryID)
VALUES
(N'iPhone 15 Pro', N'Smartphone cao cấp', N'Điện thoại Apple iPhone 15 Pro 256GB', N'Chip A17 Pro, 6GB RAM, 256GB ROM', 28990000, N'https://example.com/iphone15.jpg', 1),
(N'Samsung Galaxy S23', N'Android flagship', N'Samsung Galaxy S23 Ultra 256GB', N'Snapdragon 8 Gen 2, 12GB RAM, 256GB ROM', 25990000, N'https://example.com/s23.jpg', 1),
(N'Xiaomi Redmi Note 12', N'Giá rẻ hiệu năng cao', N'Redmi Note 12 128GB', N'Snapdragon 4 Gen 1, 6GB RAM, 128GB ROM', 5990000, N'https://example.com/redmi12.jpg', 1),
(N'MacBook Air M2', N'Laptop mỏng nhẹ', N'MacBook Air M2 2023', N'Apple M2, 16GB RAM, 512GB SSD', 31990000, N'https://example.com/macbookair.jpg', 2),
(N'MacBook Pro 14', N'Laptop hiệu năng cao', N'MacBook Pro 14 inch M2 Pro', N'Apple M2 Pro, 32GB RAM, 1TB SSD', 52990000, N'https://example.com/mbp14.jpg', 2),
(N'Asus ROG Strix G16', N'Laptop gaming', N'ROG Strix G16 RTX 4070', N'Core i9 13980HX, 32GB RAM, 1TB SSD, RTX 4070', 45990000, N'https://example.com/rog.jpg', 2),
(N'Dell XPS 13', N'Ultrabook cao cấp', N'Dell XPS 13 2023', N'Core i7, 16GB RAM, 512GB SSD', 38990000, N'https://example.com/xps13.jpg', 2),
(N'LG Tủ lạnh Inverter', N'Tủ lạnh tiết kiệm điện', N'Tủ lạnh LG Inverter 420L', N'Ngăn đá trên, tiết kiệm điện A++', 11990000, N'https://example.com/lgfridge.jpg', 3),
(N'Panasonic Máy giặt', N'Máy giặt cửa trước', N'Máy giặt Panasonic Inverter 9kg', N'TurboWash, tiết kiệm nước', 8990000, N'https://example.com/panasonicwm.jpg', 3),
(N'Sharp Lò vi sóng', N'Lò vi sóng gia đình', N'Lò vi sóng Sharp 25L', N'900W, 11 chế độ', 2490000, N'https://example.com/sharpmicrowave.jpg', 3),
(N'Apple Watch Ultra', N'Đồng hồ thông minh', N'Apple Watch Ultra 49mm', N'Sử dụng chip S8, chống nước 100m', 19990000, N'https://example.com/applewatch.jpg', 4),
(N'AirPods Pro 2', N'Tai nghe không dây', N'Apple AirPods Pro Gen 2', N'ANC, Adaptive Transparency', 5990000, N'https://example.com/airpodspro2.jpg', 4),
(N'Samsung Galaxy Buds2', N'Tai nghe không dây', N'Samsung Galaxy Buds2 Pro', N'ANC, Bluetooth 5.3', 4990000, N'https://example.com/buds2.jpg', 4),
(N'Logitech MX Master 3S', N'Chuột cao cấp', N'Chuột không dây Logitech MX Master 3S', N'Bluetooth/USB, DPI 8000', 2490000, N'https://example.com/mxmaster3s.jpg', 4),
(N'Razer BlackWidow V3', N'Bàn phím gaming', N'Razer BlackWidow V3 Mechanical Keyboard', N'Switch Green, RGB', 3990000, N'https://example.com/razerbw.jpg', 4),
(N'Sony WH-1000XM5', N'Tai nghe chống ồn', N'Sony WH-1000XM5 Over-ear', N'ANC, Pin 30h', 8990000, N'https://example.com/sony1000xm5.jpg', 4),
(N'Oppo Find X6', N'Flagship camera', N'Oppo Find X6 Pro 512GB', N'Snapdragon 8 Gen 2, Camera Hasselblad', 23990000, N'https://example.com/oppo.jpg', 1),
(N'HP Spectre x360', N'Laptop gập xoay', N'HP Spectre x360 OLED 2023', N'Core i7, 16GB RAM, 1TB SSD', 41990000, N'https://example.com/spectre.jpg', 2),
(N'Midea Điều hòa', N'Điều hòa 1.5HP', N'Máy lạnh Midea Inverter', N'Turbo Cooling, tiết kiệm điện', 7990000, N'https://example.com/mideaac.jpg', 3),
(N'JBL Flip 6', N'Loa bluetooth', N'JBL Flip 6 Portable Speaker', N'20W, chống nước IP67', 2990000, N'https://example.com/jblflip6.jpg', 4);
GO

-- Insert Carts (20 giỏ hàng, mỗi user có thể có 1 giỏ)
INSERT INTO Carts (UserID, TotalPrice, Status)
VALUES
(2, 28990000, N'Active'),
(3, 25990000, N'Completed'),
(4, 9190000, N'Completed'),
(5, 52990000, N'Completed'),
(6, 3990000, N'Active'),
(7, 11990000, N'Completed'),
(8, 8990000, N'Completed'),
(9, 2490000, N'Completed'),
(10, 41990000, N'Active'),
(11, 23990000, N'Completed'),
(12, 2990000, N'Active'),
(13, 45990000, N'Completed'),
(14, 7990000, N'Completed'),
(15, 5990000, N'Completed'),
(16, 28990000, N'Active'),
(17, 38990000, N'Completed'),
(18, 45990000, N'Completed'),
(19, 4990000, N'Active'),
(20, 8990000, N'Completed'),
(2, 2490000, N'Completed');
GO

-- Insert CartItems (mỗi giỏ có nhiều item)
INSERT INTO CartItems (CartID, ProductID, Quantity, Price)
VALUES
(1, 1, 1, 28990000),
(2, 2, 1, 25990000),
(3, 3, 1, 5990000),
(3, 12, 1, 5990000),
(4, 5, 1, 52990000),
(5, 15, 1, 3990000),
(6, 8, 1, 11990000),
(7, 9, 1, 8990000),
(8, 10, 1, 2490000),
(9, 18, 1, 41990000),
(10, 17, 1, 23990000),
(11, 20, 1, 2990000),
(12, 6, 1, 45990000),
(13, 19, 1, 7990000),
(14, 12, 1, 5990000),
(15, 1, 1, 28990000),
(16, 4, 1, 31990000),
(17, 6, 1, 45990000),
(18, 13, 1, 4990000),
(19, 16, 1, 8990000);
GO

-- Insert Orders (20 order, liên kết carts)
INSERT INTO Orders (CartID, UserID, PaymentMethod, BillingAddress, OrderStatus)
VALUES
(2, 3, N'Credit Card', N'34 Hai Bà Trưng, Q1, HCM', N'Success'),
(3, 4, N'COD', N'56 Nguyễn Trãi, Q5, HCM', N'Success'),
(4, 5, N'Credit Card', N'90 Pasteur, Q1, HCM', N'Success'),
(6, 7, N'Bank Transfer', N'15 Điện Biên Phủ, Q1, HCM', N'Success'),
(7, 8, N'COD', N'45 Lý Tự Trọng, Q1, HCM', N'Success'),
(8, 9, N'Momo', N'67 Võ Văn Tần, Q3, HCM', N'Success'),
(9, 10, N'Credit Card', N'89 Lý Chính Thắng, Q3, HCM', N'Pending'),
(10, 11, N'Bank Transfer', N'12 Nguyễn Văn Cừ, Q5, HCM', N'Success'),
(11, 12, N'COD', N'33 Tôn Đức Thắng, Q1, HCM', N'Success'),
(12, 13, N'COD', N'55 Trần Hưng Đạo, Q5, HCM', N'Success'),
(13, 14, N'Momo', N'77 Nguyễn Thị Minh Khai, Q1, HCM', N'Success'),
(14, 15, N'COD', N'99 Võ Thị Sáu, Q3, HCM', N'Success'),
(15, 16, N'Credit Card', N'12 CMT8, Q10, HCM', N'Pending'),
(16, 17, N'Momo', N'21 Hồng Bàng, Q5, HCM', N'Success'),
(17, 18, N'Bank Transfer', N'30 Nguyễn Tri Phương, Q10, HCM', N'Success'),
(18, 19, N'COD', N'40 Hoàng Văn Thụ, Q.Tân Bình, HCM', N'Success'),
(19, 20, N'COD', N'268 Lý Thường Kiệt, Q10, HCM', N'Success'),
(3, 2, N'Momo', N'12 Lê Lợi, Q1, HCM', N'Success'),
(4, 6, N'COD', N'22 Nguyễn Huệ, Q1, HCM', N'Success'),
(10, 8, N'Bank Transfer', N'15 Điện Biên Phủ, Q1, HCM', N'Pending');
GO

-- Insert Payments (20 payment, mapping order)
INSERT INTO Payments (OrderID, Amount, PaymentStatus)
VALUES
(1, 25990000, N'Paid'),
(2, 9190000, N'Paid'),
(3, 52990000, N'Paid'),
(4, 3990000, N'Paid'),
(5, 11990000, N'Paid'),
(6, 8990000, N'Paid'),
(7, 41990000, N'Pending'),
(8, 23990000, N'Paid'),
(9, 2990000, N'Paid'),
(10, 45990000, N'Paid'),
(11, 7990000, N'Paid'),
(12, 5990000, N'Paid'),
(13, 28990000, N'Pending'),
(14, 38990000, N'Paid'),
(15, 45990000, N'Paid'),
(16, 4990000, N'Paid'),
(17, 8990000, N'Paid'),
(18, 5990000, N'Paid'),
(19, 11990000, N'Paid'),
(20, 23990000, N'Pending');
GO

-- Insert Notifications (20 row)
INSERT INTO Notifications (UserID, Message, IsRead)
VALUES
(2, N'Đơn hàng của bạn đã được xác nhận.', 0),
(3, N'Thanh toán thành công đơn hàng.', 1),
(4, N'Giỏ hàng của bạn còn sản phẩm chưa thanh toán.', 0),
(5, N'Đơn hàng #3 đang được giao.', 1),
(6, N'Bạn đã thêm sản phẩm mới vào giỏ.', 0),
(7, N'Thanh toán đơn hàng thành công.', 1),
(8, N'Đơn hàng #7 đã được giao.', 1),
(9, N'Đơn hàng #8 đã được hủy.', 0),
(10, N'Bạn nhận được khuyến mãi 10% cho đơn hàng tiếp theo.', 0),
(11, N'Đơn hàng #10 đang chờ thanh toán.', 0),
(12, N'Bạn vừa thêm sản phẩm vào wishlist.', 0),
(13, N'Đơn hàng #12 đã hoàn tất.', 1),
(14, N'Voucher 50k cho bạn trong tuần này.', 0),
(15, N'Đơn hàng #14 thành công.', 1),
(16, N'Đơn hàng #15 đang xử lý.', 0),
(17, N'Bạn đã thanh toán đơn hàng #16.', 1),
(18, N'Đơn hàng #17 thành công.', 1),
(19, N'Đơn hàng #18 đang giao.', 0),
(20, N'Chúc mừng bạn nhận voucher 100k.', 0);
GO

-- Insert ChatMessages (20 row)
INSERT INTO ChatMessages (UserID, Message)
VALUES
(2, N'Tôi muốn hỏi về iPhone 15 Pro còn hàng không?'),
(3, N'Xin báo giá chi tiết Samsung S23.'),
(4, N'Có giao hàng về Bình Dương không?'),
(5, N'Thời gian bảo hành của MacBook Pro là bao lâu?'),
(6, N'Tôi muốn trả góp Asus ROG Strix được không?'),
(7, N'LG Tủ lạnh Inverter có màu bạc không?'),
(8, N'Panasonic Máy giặt có giặt hơi nước không?'),
(9, N'Lò vi sóng Sharp bảo hành bao lâu?'),
(10, N'Apple Watch Ultra có chống nước biển không?'),
(11, N'AirPods Pro 2 kết nối được Android không?'),
(12, N'Samsung Buds2 có micro không?'),
(13, N'Logitech MX Master có hỗ trợ Mac không?'),
(14, N'Bàn phím Razer có layout TKL không?'),
(15, N'Sony WH-1000XM5 có app điều khiển không?'),
(16, N'Oppo Find X6 có sạc nhanh bao nhiêu W?'),
(17, N'HP Spectre có màn hình cảm ứng không?'),
(18, N'Điều hòa Midea có chế độ ngủ không?'),
(19, N'JBL Flip 6 có sạc nhanh không?'),
(20, N'Tôi cần xuất hóa đơn VAT cho đơn hàng.');
GO

