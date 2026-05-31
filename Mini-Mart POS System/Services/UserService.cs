using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiniMartPOS.Data;
using MiniMartPOS.Models;

namespace MiniMartPOS.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public UserService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
        {
            return await _context.Users
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<ApplicationUser> CreateUserAsync(ApplicationUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return user;
        }

        public async Task<ApplicationUser> UpdateUserAsync(ApplicationUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return user;
        }

        public async Task DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.IsActive = false;
                await _userManager.UpdateAsync(user);
            }
        }

        public async Task AssignRoleAsync(string userId, string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.AddToRoleAsync(user, roleName);
            }
        }

        public async Task RemoveRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.RemoveFromRoleAsync(user, roleName);
            }
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                return await _userManager.GetRolesAsync(user);
            }
            return Enumerable.Empty<string>();
        }

        public async Task<bool> HasPermissionAsync(string userId, string permission)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var roles = await _userManager.GetRolesAsync(user);

            // Define role permissions
            var rolePermissions = new Dictionary<string, List<string>>
            {
                ["SuperAdmin"] = new List<string> { "FullAccess" },
                ["Admin"] = new List<string> { "ViewReports", "ManageProducts", "ManageUsers", "BackupDatabase" },
                ["Manager"] = new List<string> { "ViewReports", "ManageInventory" },
                ["Cashier"] = new List<string> { "SellProducts", "PrintReceipt" },
                ["StockClerk"] = new List<string> { "ManageInventory" }
            };

            foreach (var role in roles)
            {
                if (rolePermissions.ContainsKey(role) && rolePermissions[role].Contains(permission))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
