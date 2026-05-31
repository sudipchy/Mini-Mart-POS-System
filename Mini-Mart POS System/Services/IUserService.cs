using MiniMartPOS.Models;

namespace MiniMartPOS.Services
{
    public interface IUserService
    {
        Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
        Task<ApplicationUser?> GetUserByIdAsync(string id);
        Task<ApplicationUser> CreateUserAsync(ApplicationUser user, string password);
        Task<ApplicationUser> UpdateUserAsync(ApplicationUser user);
        Task DeleteUserAsync(string id);
        Task AssignRoleAsync(string userId, string roleName);
        Task RemoveRoleAsync(string userId, string roleName);
        Task<IEnumerable<string>> GetUserRolesAsync(string userId);
        Task<bool> HasPermissionAsync(string userId, string permission);
    }
}
