using MiniMartPOS.Models;

namespace MiniMartPOS.Services
{
    public interface IBackupService
    {
        Task<Backup> CreateBackupAsync(string userId);
        Task<byte[]> DownloadBackupAsync(int backupId);
        Task RestoreBackupAsync(byte[] backupData);
        Task<IEnumerable<Backup>> GetAllBackupsAsync();
        Task DeleteBackupAsync(int backupId);
        Task CleanupOldBackupsAsync(int keepCount);
    }
}
