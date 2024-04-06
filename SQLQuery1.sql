-- Kiểm tra nếu cơ sở dữ liệu đã tồn tại, thì xóa nó
IF DB_ID('QLMT_DATN') IS NOT NULL
BEGIN
    USE master;
    ALTER DATABASE QLMT_DATN SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QLMT_DATN;
END

-- Tạo mới cơ sở dữ liệu
CREATE DATABASE QLMT_DATN;
GO
USE [QLMT_DATN]
GO

-- Bảng cho người dùng
CREATE TABLE users (
    user_id INT PRIMARY KEY IDENTITY,
    username VARCHAR(50) NOT NULL,
    email VARCHAR(100) NOT NULL UNIQUE,
    password VARCHAR(100) NOT NULL,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Bảng cho sản phẩm
CREATE TABLE products (
    product_id INT PRIMARY KEY IDENTITY,
    product_name VARCHAR(100) NOT NULL,
    price DECIMAL(10,2) NOT NULL,
    description TEXT,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Bảng cho đơn đặt hàng
CREATE TABLE orders (
    order_id INT PRIMARY KEY IDENTITY,
    user_id INT NOT NULL,
    product_id INT NOT NULL,
    quantity INT NOT NULL,
    order_date DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(user_id),
    FOREIGN KEY (product_id) REFERENCES products(product_id)
);

-- Bảng danh mục sản phẩm
CREATE TABLE categories (
    category_id INT PRIMARY KEY IDENTITY,
    category_name VARCHAR(100) NOT NULL
);

-- Bảng liên kết sản phẩm với danh mục
CREATE TABLE product_categories (
    product_id INT,
    category_id INT,
    PRIMARY KEY (product_id, category_id),
    FOREIGN KEY (product_id) REFERENCES products(product_id),
    FOREIGN KEY (category_id) REFERENCES categories(category_id)
);

-- Bảng đánh giá sản phẩm
CREATE TABLE reviews (
    review_id INT PRIMARY KEY IDENTITY,
    product_id INT NOT NULL,
    user_id INT NOT NULL,
    rating INT NOT NULL,
    comment TEXT,
    review_date DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (product_id) REFERENCES products(product_id),
    FOREIGN KEY (user_id) REFERENCES users(user_id)
);

-- Bảng giỏ hàng
CREATE TABLE cart (
    cart_id INT PRIMARY KEY IDENTITY,
    user_id INT NOT NULL,
    product_id INT NOT NULL,
    quantity INT NOT NULL,
    FOREIGN KEY (user_id) REFERENCES users(user_id),
    FOREIGN KEY (product_id) REFERENCES products(product_id)
);
