using Microsoft.EntityFrameworkCore;
using MiniMartPOS.Data;
using MiniMartPOS.Models;
using System.IO.Compression;
using System.Text;

namespace MiniMartPOS.Services
{
    public class BackupService : IBackupService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly string _backupPath;

        public BackupService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _backupPath = configuration["BackupSettings:BackupPath"] ?? "Backups";
            
            if (!Directory.Exists(_backupPath))
            {
                Directory.CreateDirectory(_backupPath);
            }
        }

        public async Task<Backup> CreateBackupAsync(string userId)
        {
            var fileName = $"MiniMartPOS_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
            var filePath = Path.Combine(_backupPath, fileName);

            // Generate SQL backup script
            var sqlScript = await GenerateSqlScriptAsync();
            
            // Write to file
            await File.WriteAllTextAsync(filePath, sqlScript);

            // Create ZIP
            var zipFileName = fileName.Replace(".bak", ".zip");
            var zipFilePath = Path.Combine(_backupPath, zipFileName);
            
            using (var zip = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
            {
                zip.CreateEntryFromFile(filePath, fileName);
            }

            // Delete the .bak file
            File.Delete(filePath);

            var fileInfo = new FileInfo(zipFilePath);

            var backup = new Backup
            {
                FileName = zipFileName,
                FilePath = zipFilePath,
                BackupDate = DateTime.Now,
                FileSize = fileInfo.Length,
                BackupType = "Manual",
                UserId = userId,
                Status = true
            };

            _context.Backups.Add(backup);
            await _context.SaveChangesAsync();

            return backup;
        }

        public async Task<byte[]> DownloadBackupAsync(int backupId)
        {
            var backup = await _context.Backups.FindAsync(backupId);
            if (backup == null || !File.Exists(backup.FilePath))
            {
                throw new FileNotFoundException("Backup file not found");
            }

            return await File.ReadAllBytesAsync(backup.FilePath);
        }

        public async Task RestoreBackupAsync(byte[] backupData)
        {
            // Extract ZIP and restore database
            using var memoryStream = new MemoryStream(backupData);
            using var archive = new ZipArchive(memoryStream);
            
            var entry = archive.Entries.FirstOrDefault();
            if (entry == null)
            {
                throw new InvalidOperationException("Invalid backup file");
            }

            using var entryStream = entry.Open();
            using var reader = new StreamReader(entryStream);
            var sqlScript = await reader.ReadToEndAsync();

            // Execute SQL script to restore database
            // Note: This is a simplified implementation
            // In production, use proper SQL Server backup/restore commands
            await _context.Database.ExecuteSqlRawAsync(sqlScript);
        }

        public async Task<IEnumerable<Backup>> GetAllBackupsAsync()
        {
            return await _context.Backups
                .Include(b => b.User)
                .Where(b => b.Status)
                .OrderByDescending(b => b.BackupDate)
                .ToListAsync();
        }

        public async Task DeleteBackupAsync(int backupId)
        {
            var backup = await _context.Backups.FindAsync(backupId);
            if (backup != null)
            {
                if (File.Exists(backup.FilePath))
                {
                    File.Delete(backup.FilePath);
                }
                
                backup.Status = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task CleanupOldBackupsAsync(int keepCount)
        {
            var backups = await _context.Backups
                .Where(b => b.Status)
                .OrderByDescending(b => b.BackupDate)
                .ToListAsync();

            var backupsToDelete = backups.Skip(keepCount).ToList();

            foreach (var backup in backupsToDelete)
            {
                await DeleteBackupAsync(backup.Id);
            }
        }

        private async Task<string> GenerateSqlScriptAsync()
        {
            var script = new StringBuilder();
            
            // Get all table data and generate INSERT statements
            // This is a simplified implementation
            // In production, use SQL Server's BACKUP DATABASE command
            
            script.AppendLine("-- MiniMart POS Database Backup");
            script.AppendLine($"-- Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            script.AppendLine();

            // Add data for each table
            var products = await _context.Products.ToListAsync();
            script.AppendLine("-- Products");
            foreach (var product in products)
            {
                script.AppendLine($"INSERT INTO Products (Id, Barcode, ProductName, CategoryId, PurchasePrice, SellingPrice, StockQty, MinimumStock, SupplierId, DateAdded, Status, ExpiryDate) VALUES ({product.Id}, '{product.Barcode}', '{product.ProductName}', {product.CategoryId}, {product.PurchasePrice}, {product.SellingPrice}, {product.StockQty}, {product.MinimumStock}, {product.SupplierId}, '{product.DateAdded:yyyy-MM-dd}', {(product.Status ? 1 : 0)}, {(product.ExpiryDate.HasValue ? $"'{product.ExpiryDate.Value:yyyy-MM-dd}'" : "NULL")});");
            }

            return script.ToString();
        }
    }
}
