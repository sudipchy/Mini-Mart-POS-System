-- Mini-Mart POS Database Setup Script
-- Run this script to create the database schema and seed initial data

-- Create Database (if not exists)
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'MiniMartPOSDB')
BEGIN
    CREATE DATABASE MiniMartPOSDB;
END
GO

USE MiniMartPOSDB;
GO

-- Create Tables

-- Categories Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Categories')
BEGIN
    CREATE TABLE Categories (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        CategoryName NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500),
        CreatedDate DATETIME DEFAULT GETDATE(),
        Status BIT DEFAULT 1
    );
END
GO

-- Suppliers Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Suppliers')
BEGIN
    CREATE TABLE Suppliers (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        SupplierName NVARCHAR(200) NOT NULL,
        Phone NVARCHAR(20),
        Email NVARCHAR(100),
        Address NVARCHAR(500),
        ContactPerson NVARCHAR(100),
        CreatedDate DATETIME DEFAULT GETDATE(),
        Status BIT DEFAULT 1
    );
END
GO

-- Products Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Products')
BEGIN
    CREATE TABLE Products (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Barcode NVARCHAR(50) UNIQUE,
        ProductName NVARCHAR(200) NOT NULL,
        CategoryId INT NOT NULL,
        PurchasePrice DECIMAL(18,2) NOT NULL,
        SellingPrice DECIMAL(18,2) NOT NULL,
        StockQty INT NOT NULL DEFAULT 0,
        MinimumStock INT NOT NULL DEFAULT 5,
        SupplierId INT,
        DateAdded DATETIME DEFAULT GETDATE(),
        Status BIT DEFAULT 1,
        ExpiryDate DATETIME,
        FOREIGN KEY (CategoryId) REFERENCES Categories(Id),
        FOREIGN KEY (SupplierId) REFERENCES Suppliers(Id)
    );
END
GO

-- Customers Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Customers')
BEGIN
    CREATE TABLE Customers (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        CustomerName NVARCHAR(200) NOT NULL,
        Phone NVARCHAR(20),
        Email NVARCHAR(100),
        Address NVARCHAR(500),
        LoyaltyPoints INT DEFAULT 0,
        OutstandingBalance DECIMAL(18,2) DEFAULT 0,
        CreatedDate DATETIME DEFAULT GETDATE(),
        Status BIT DEFAULT 1
    );
END
GO

-- Sales Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Sales')
BEGIN
    CREATE TABLE Sales (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        InvoiceNumber NVARCHAR(50) UNIQUE NOT NULL,
        SaleDate DATETIME DEFAULT GETDATE(),
        UserId NVARCHAR(450) NOT NULL,
        CustomerId INT,
        Subtotal DECIMAL(18,2) NOT NULL,
        Discount DECIMAL(18,2) DEFAULT 0,
        Tax DECIMAL(18,2) DEFAULT 0,
        GrandTotal DECIMAL(18,2) NOT NULL,
        PaidAmount DECIMAL(18,2) NOT NULL,
        ChangeAmount DECIMAL(18,2) DEFAULT 0,
        PaymentMethod NVARCHAR(50) NOT NULL,
        Notes NVARCHAR(500),
        Status BIT DEFAULT 1,
        FOREIGN KEY (CustomerId) REFERENCES Customers(Id)
    );
END
GO

-- SaleDetails Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SaleDetails')
BEGIN
    CREATE TABLE SaleDetails (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        SaleId INT NOT NULL,
        ProductId INT NOT NULL,
        Quantity INT NOT NULL,
        UnitPrice DECIMAL(18,2) NOT NULL,
        Discount DECIMAL(18,2) DEFAULT 0,
        Total DECIMAL(18,2) NOT NULL,
        FOREIGN KEY (SaleId) REFERENCES Sales(Id),
        FOREIGN KEY (ProductId) REFERENCES Products(Id)
    );
END
GO

-- Purchases Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Purchases')
BEGIN
    CREATE TABLE Purchases (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        PurchaseNumber NVARCHAR(50) UNIQUE NOT NULL,
        PurchaseDate DATETIME DEFAULT GETDATE(),
        SupplierId INT NOT NULL,
        UserId NVARCHAR(450) NOT NULL,
        TotalAmount DECIMAL(18,2) NOT NULL,
        PaidAmount DECIMAL(18,2) NOT NULL,
        DueAmount DECIMAL(18,2) DEFAULT 0,
        Notes NVARCHAR(500),
        Status BIT DEFAULT 1,
        FOREIGN KEY (SupplierId) REFERENCES Suppliers(Id)
    );
END
GO

-- PurchaseDetails Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PurchaseDetails')
BEGIN
    CREATE TABLE PurchaseDetails (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        PurchaseId INT NOT NULL,
        ProductId INT NOT NULL,
        Quantity INT NOT NULL,
        UnitPrice DECIMAL(18,2) NOT NULL,
        Total DECIMAL(18,2) NOT NULL,
        FOREIGN KEY (PurchaseId) REFERENCES Purchases(Id),
        FOREIGN KEY (ProductId) REFERENCES Products(Id)
    );
END
GO

-- InventoryLogs Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'InventoryLogs')
BEGIN
    CREATE TABLE InventoryLogs (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ProductId INT NOT NULL,
        UserId NVARCHAR(450) NOT NULL,
        TransactionType NVARCHAR(20) NOT NULL,
        Quantity INT NOT NULL,
        PreviousStock INT NOT NULL,
        NewStock INT NOT NULL,
        TransactionDate DATETIME DEFAULT GETDATE(),
        Notes NVARCHAR(500),
        FOREIGN KEY (ProductId) REFERENCES Products(Id)
    );
END
GO

-- Expenses Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Expenses')
BEGIN
    CREATE TABLE Expenses (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ExpenseName NVARCHAR(100) NOT NULL,
        Amount DECIMAL(18,2) NOT NULL,
        Category NVARCHAR(50) NOT NULL,
        ExpenseDate DATETIME DEFAULT GETDATE(),
        Description NVARCHAR(500),
        UserId NVARCHAR(450) NOT NULL,
        Status BIT DEFAULT 1
    );
END
GO

-- Backups Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Backups')
BEGIN
    CREATE TABLE Backups (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        FileName NVARCHAR(200) NOT NULL,
        FilePath NVARCHAR(500) NOT NULL,
        BackupDate DATETIME DEFAULT GETDATE(),
        FileSize BIGINT NOT NULL,
        BackupType NVARCHAR(50) NOT NULL,
        UserId NVARCHAR(450) NOT NULL,
        Status BIT DEFAULT 1
    );
END
GO

-- AuditLogs Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AuditLogs')
BEGIN
    CREATE TABLE AuditLogs (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UserId NVARCHAR(450) NOT NULL,
        Action NVARCHAR(100) NOT NULL,
        Module NVARCHAR(100) NOT NULL,
        RecordId NVARCHAR(50),
        OldValues NVARCHAR(1000),
        NewValues NVARCHAR(1000),
        ActionDate DATETIME DEFAULT GETDATE(),
        IPAddress NVARCHAR(100)
    );
END
GO

-- Seed Initial Data

-- Seed Categories
IF NOT EXISTS (SELECT * FROM Categories)
BEGIN
    INSERT INTO Categories (CategoryName, Description, Status) VALUES
    ('Beverages', 'Soft drinks, juices, water', 1),
    ('Snacks', 'Chips, biscuits, cookies', 1),
    ('Dairy', 'Milk, cheese, yogurt', 1),
    ('Rice & Grains', 'Rice, wheat, flour', 1),
    ('Cosmetics', 'Beauty products', 1),
    ('Household', 'Cleaning supplies', 1),
    ('Personal Care', 'Soap, shampoo, toothpaste', 1),
    ('Frozen Foods', 'Frozen items', 1);
END
GO

-- Create Indexes for Performance
CREATE INDEX IX_Products_Barcode ON Products(Barcode);
CREATE INDEX IX_Products_CategoryId ON Products(CategoryId);
CREATE INDEX IX_Products_SupplierId ON Products(SupplierId);
CREATE INDEX IX_Sales_InvoiceNumber ON Sales(InvoiceNumber);
CREATE INDEX IX_Sales_SaleDate ON Sales(SaleDate);
CREATE INDEX IX_SaleDetails_SaleId ON SaleDetails(SaleId);
CREATE INDEX IX_SaleDetails_ProductId ON SaleDetails(ProductId);
CREATE INDEX IX_Purchases_PurchaseNumber ON Purchases(PurchaseNumber);
CREATE INDEX IX_InventoryLogs_ProductId ON InventoryLogs(ProductId);
CREATE INDEX IX_InventoryLogs_TransactionDate ON InventoryLogs(TransactionDate);
GO

PRINT 'Database setup completed successfully!';
