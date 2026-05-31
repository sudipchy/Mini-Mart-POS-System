# Mini-Mart POS System

A comprehensive Point of Sale (POS) system for mini-marts and local shops, built with ASP.NET Core and SQL Server.

## Features

### Core Modules
- **Dashboard** - Real-time sales metrics, profit tracking, low stock alerts, recent transactions
- **Product Management** - Add, edit, delete products with barcode support, import/export Excel
- **POS Billing** - Fast barcode scanning, cart management, multiple payment methods, receipt printing
- **Inventory Management** - Stock in/out, stock adjustments, inventory logs, expiry tracking
- **Sales Reports** - Daily/monthly sales, stock reports, profit analysis, top-selling products
- **Supplier Management** - Supplier profiles, purchase history tracking
- **User Management** - Role-based access control (Admin, Manager, Cashier, Stock Clerk)
- **Backup System** - Automatic and manual database backups with restore functionality
- **Customer Management** - Customer profiles, loyalty points system
- **QR Payments** - Support for eSewa, Khalti, and FonePay

### Security Features
- ASP.NET Identity for authentication
- Password hashing with salt
- Session timeout after 5 minutes idle
- Role-based permissions
- Audit logging for all actions

## Technology Stack

- **Web Server**: IIS
- **Backend**: ASP.NET Core 8.0
- **Database**: SQL Server Express
- **Frontend**: HTML, CSS, JavaScript, Bootstrap 5
- **Reports**: iTextSharp (PDF generation)
- **Excel**: EPPlus
- **QR Codes**: QRCoder

## Prerequisites

- .NET 8.0 SDK
- SQL Server Express or SQL Server
- Visual Studio 2022 or VS Code

## Installation

1. Clone the repository
2. Open the project in Visual Studio
3. Update connection string in `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER;Database=MiniMartPOSDB;Trusted_Connection=True;MultipleActiveResultSets=true"
   }
   ```
4. Run the following commands in Package Manager Console:
   ```
   Update-Database
   ```
5. Build and run the project

## Default Admin Account

After running the application for the first time, register a new account. The first registered user will be assigned the Admin role automatically.

## Folder Structure

```
MiniMartPOS/
├── Controllers/          # MVC Controllers
├── Models/              # Data models and ViewModels
├── Services/            # Business logic services
├── Data/                # Database context
├── Views/               # Razor views
├── wwwroot/             # Static files (CSS, JS, images)
│   ├── css/
│   ├── js/
│   └── images/
├── Reports/             # Generated reports
├── Backups/             # Database backups
├── Logs/                # Application logs
├── Uploads/             # File uploads
└── Settings/            # Configuration files
```

## User Roles and Permissions

### Super Admin
- Full access to all features

### Admin
- View reports
- Manage products
- Manage users
- Backup database
- Full inventory access

### Manager
- View reports
- Manage inventory
- Supplier management

### Cashier
- Sell products
- Print receipts
- View daily sales

### Stock Clerk
- Manage inventory
- Stock in/out
- View inventory logs

## API Endpoints

### Authentication
- POST /Account/Login - User login
- POST /Account/Logout - User logout
- POST /Account/Register - Register new user

### Products
- GET /Product - List all products
- GET /Product/Create - Create product form
- POST /Product/Create - Save new product
- GET /Product/Edit/{id} - Edit product
- POST /Product/Edit - Update product
- POST /Product/Delete/{id} - Delete product
- GET /Product/LowStock - View low stock products
- POST /Product/Import - Import products from Excel
- GET /Product/Export - Export products to Excel

### POS
- GET /POS - POS billing interface
- POST /POS/ScanBarcode - Scan product barcode
- POST /POS/AddToCart - Add product to cart
- POST /POS/CompleteSale - Complete sale transaction
- GET /POS/Receipt/{id} - View receipt

### Inventory
- GET /Inventory - View inventory logs
- GET /Inventory/StockIn - Stock in form
- POST /Inventory/StockIn - Process stock in
- GET /Inventory/StockOut - Stock out form
- POST /Inventory/StockOut - Process stock out
- GET /Inventory/Adjust - Stock adjustment form
- POST /Inventory/Adjust - Adjust stock quantity

### Reports
- GET /Report - Reports dashboard
- POST /Report/DailySales - Generate daily sales report
- POST /Report/MonthlySales - Generate monthly sales report
- GET /Report/Stock - Generate stock report
- POST /Report/TopSelling - Generate top selling products report
- POST /Report/Profit - Generate profit report

### Backup
- GET /Backup - View backup history
- POST /Backup/CreateBackup - Create new backup
- GET /Backup/Download/{id} - Download backup file
- POST /Backup/Restore - Restore from backup
- POST /Backup/Delete/{id} - Delete backup

## Configuration

### Backup Settings
Configure in `appsettings.json`:
```json
"BackupSettings": {
  "BackupPath": "Backups",
  "BackupTime": "23:59",
  "KeepLastBackups": 30,
  "AutoBackupEnabled": true
}
```

### QR Payment Settings
```json
"QRPaymentSettings": {
  "eSewaEnabled": true,
  "KhaltiEnabled": true,
  "FonePayEnabled": true
}
```

### Loyalty Settings
```json
"LoyaltySettings": {
  "PointsPerRupee": 1,
  "PointsForDiscount": 100,
  "DiscountPercentage": 5
}
```

## Development

### Adding New Features

1. Create model in `Models/` folder
2. Add DbSet in `ApplicationDbContext`
3. Create service interface in `Services/`
4. Implement service in `Services/`
5. Register service in `Program.cs`
6. Create controller in `Controllers/`
7. Create views in `Views/{ControllerName}/`

### Database Migrations

```bash
Add-Migration MigrationName
Update-Database
```

## Deployment

### IIS Deployment

1. Publish the application:
   ```
   dotnet publish -c Release -o C:\inetpub\wwwroot\MiniMartPOS
   ```
2. Create IIS website pointing to published folder
3. Configure Application Pool with .NET CLR version: No Managed Code
4. Update connection string for production database
5. Set appropriate file permissions for Backups, Logs, and Uploads folders

## Support

For issues and questions, please contact the development team.

## License

Copyright © 2026 Mini-Mart POS System. All rights reserved.
