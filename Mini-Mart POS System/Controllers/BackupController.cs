using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniMartPOS.Services;

namespace MiniMartPOS.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class BackupController : Controller
    {
        private readonly IBackupService _backupService;

        public BackupController(IBackupService backupService)
        {
            _backupService = backupService;
        }

        public async Task<IActionResult> Index()
        {
            var backups = await _backupService.GetAllBackupsAsync();
            return View(backups);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBackup()
        {
            var userId = User.Identity?.Name ?? "";
            var backup = await _backupService.CreateBackupAsync(userId);
            TempData["Success"] = $"Backup created successfully: {backup.FileName}";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Download(int id)
        {
            try
            {
                var backupData = await _backupService.DownloadBackupAsync(id);
                var backup = await _backupService.GetAllBackupsAsync();
                var backupFile = backup.FirstOrDefault(b => b.Id == id);
                
                return File(backupData, "application/zip", backupFile?.FileName ?? "backup.zip");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Restore()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Restore(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                var backupData = memoryStream.ToArray();

                try
                {
                    await _backupService.RestoreBackupAsync(backupData);
                    TempData["Success"] = "Database restored successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Error restoring backup: " + ex.Message);
                }
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _backupService.DeleteBackupAsync(id);
            TempData["Success"] = "Backup deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Cleanup()
        {
            var keepCount = 30; // Keep last 30 backups
            await _backupService.CleanupOldBackupsAsync(keepCount);
            TempData["Success"] = "Old backups cleaned up successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
