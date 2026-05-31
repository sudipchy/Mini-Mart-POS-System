# Mini-Mart POS System - Setup Instructions

## Quick Start Guide

### Prerequisites
- .NET 8.0 SDK (Download from https://dotnet.microsoft.com/download)
- SQL Server Express or SQL Server (Download from https://www.microsoft.com/sql-server/sql-server-downloads)
- Visual Studio 2022 or VS Code (Recommended)

### Step 1: Database Setup

#### Option A: Using SQL Server Management Studio (SSMS)
1. Open SSMS and connect to your SQL Server instance
2. Open the `DatabaseSetup.sql` file
3. Execute the script to create the database and tables
4. Verify the database `MiniMartPOSDB` is created

#### Option B: Using Command Line
```bash
sqlcmd -S localhost\SQLEXPRESS -E -i DatabaseSetup.sql
```

#### Option C: Using Entity Framework Migrations (Recommended)
1. Update connection string in `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=MiniMartPOSDB;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

2. Run the following commands in Package Manager Console or terminal:
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Step 2: Configure Application

1. Open the project in Visual Studio or VS Code
2. Update `appsettings.json` with your database connection string
3. Update other settings as needed (backup path, QR payment settings, etc.)

### Step 3: Build and Run

#### Using Visual Studio
1. Open the solution file
2. Press F5 or click "Start" button
3. The application will open in your browser at `https://localhost:5001`

#### Using Command Line
```bash
dotnet build
dotnet run
```

### Step 4: Initial Setup

1. Register your first user account
   - Navigate to `/Account/Register`
   - Fill in the registration form
   - The first registered user will automatically be assigned the Admin role

2. Add initial data:
   - Go to Categories and create product categories
   - Go to Suppliers and add your suppliers
   - Go to Products and add your products with barcodes

3. Configure settings:
   - Set up backup schedule in appsettings.json
   - Configure QR payment options
   - Set loyalty program parameters

## Folder Structure

After setup, your folder structure should look like:

```
MiniMartPOS/
├── Controllers/          # MVC Controllers
├── Models/              # Data models and ViewModels
│   └── ViewModels/      # View-specific models
├── Services/            # Business logic services
├── Data/                # Database context
├── Views/               # Razor views
│   ├── Account/
│   ├── Backup/
│   ├── Customer/
│   ├── Home/
│   ├── Inventory/
│   ├── POS/
│   ├── Product/
│   ├── Report/
│   ├── Shared/
│   ├── Supplier/
│   └── User/
├── wwwroot/             # Static files
│   ├── css/
│   │   └── style.css
│   ├── js/
│   │   └── main.js
│   └── images/
│       └── logo.svg
├── Backups/             # Database backups (auto-created)
├── Logs/                # Application logs (auto-created)
├── Uploads/             # File uploads (auto-created)
├── Reports/             # Generated reports (auto-created)
├── Program.cs           # Application entry point
├── appsettings.json     # Configuration
├── MiniMartPOS.csproj   # Project file
├── DatabaseSetup.sql    # Database setup script
└── README.md            # Documentation
```

## Default User Roles

The system includes the following roles:

- **SuperAdmin**: Full access to all features
- **Admin**: View reports, manage products, manage users, backup database
- **Manager**: View reports, manage inventory, supplier management
- **Cashier**: Sell products, print receipts, view daily sales
- **StockClerk**: Manage inventory, stock in/out, view inventory logs

## Common Issues and Solutions

### Issue: Database Connection Error
**Solution**: 
- Verify SQL Server is running
- Check connection string in appsettings.json
- Ensure SQL Server allows remote connections if needed

### Issue: Migration Errors
**Solution**:
- Delete the Migrations folder
- Run `dotnet ef migrations add InitialCreate`
- Run `dotnet ef database update`

### Issue: Barcode Scanner Not Working
**Solution**:
- Ensure barcode scanner is connected via USB
- Scanner should act as keyboard input
- Test by opening Notepad and scanning a barcode

### Issue: Backup Folder Not Created
**Solution**:
- Manually create the Backups folder in the project root
- Ensure the application has write permissions

## Deployment to IIS

### Prerequisites
- IIS installed on Windows Server
- ASP.NET Core Hosting Bundle installed
- SQL Server installed and configured

### Steps

1. **Publish the Application**
```bash
dotnet publish -c Release -o C:\inetpub\wwwroot\MiniMartPOS
```

2. **Configure IIS**
   - Open IIS Manager
   - Add Website pointing to published folder
   - Set Application Pool:
     - Name: MiniMartPOS
     - .NET CLR version: No Managed Code
     - Pipeline mode: Integrated

3. **Configure Connection String**
   - Update connection string in `appsettings.json` in published folder
   - Use production SQL Server connection details

4. **Set File Permissions**
   - Grant IIS_IUSRS write access to:
     - Backups folder
     - Logs folder
     - Uploads folder
     - Reports folder

5. **Test the Application**
   - Browse to the site URL
   - Test login functionality
   - Test all major features

## Security Recommendations

1. **Change Default Passwords**: Change default admin password immediately
2. **Enable HTTPS**: Use SSL certificate for production
3. **Regular Backups**: Schedule automatic daily backups
4. **User Access**: Assign minimum required permissions to users
5. **Audit Logs**: Regularly review audit logs for suspicious activity
6. **Network Security**: Use firewall to restrict database access
7. **Update Dependencies**: Keep .NET and packages updated

## Support and Maintenance

### Regular Maintenance Tasks
- Review and clean up old backups (keep last 30 days)
- Monitor disk space for backups and logs
- Review low stock alerts weekly
- Update product information regularly
- Reconcile sales reports monthly

### Backup Strategy
- Automatic daily backups at 11:59 PM
- Manual backup before major changes
- Store backups in multiple locations
- Test restore process periodically

### Performance Optimization
- Rebuild database indexes monthly
- Archive old sales data (older than 1 year)
- Monitor database size and growth
- Optimize queries for slow reports

## Contact

For technical support or questions, refer to the README.md file or contact the development team.

## License

Copyright © 2026 Mini-Mart POS System. All rights reserved.
