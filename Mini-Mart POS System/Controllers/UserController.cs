using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniMartPOS.Models;
using MiniMartPOS.Services;

namespace MiniMartPOS.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Username,
                    Email = model.Email,
                    FullName = model.FullName,
                    Phone = model.Phone,
                    Address = model.Address,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };

                try
                {
                    await _userService.CreateUserAsync(user, model.Password);
                    
                    // Assign role
                    if (!string.IsNullOrEmpty(model.Role))
                    {
                        await _userService.AssignRoleAsync(user.Id, model.Role);
                    }

                    TempData["Success"] = "User created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new EditUserViewModel
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                Address = user.Address,
                IsActive = user.IsActive
            };

            viewModel.Roles = (await _userService.GetUserRolesAsync(id)).ToList();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.GetUserByIdAsync(model.Id);
                if (user == null)
                {
                    return NotFound();
                }

                user.Email = model.Email;
                user.FullName = model.FullName;
                user.Phone = model.Phone;
                user.Address = model.Address;
                user.IsActive = model.IsActive;

                try
                {
                    await _userService.UpdateUserAsync(user);
                    TempData["Success"] = "User updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _userService.DeleteUserAsync(id);
            TempData["Success"] = "User deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AssignRole(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var currentRoles = await _userService.GetUserRolesAsync(id);
            ViewBag.CurrentRoles = currentRoles;

            return View(new AssignRoleViewModel { UserId = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(AssignRoleViewModel model)
        {
            await _userService.AssignRoleAsync(model.UserId, model.RoleName);
            TempData["Success"] = "Role assigned successfully.";
            return RedirectToAction(nameof(Edit), new { id = model.UserId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveRole(string userId, string roleName)
        {
            await _userService.RemoveRoleAsync(userId, roleName);
            TempData["Success"] = "Role removed successfully.";
            return RedirectToAction(nameof(Edit), new { id = userId });
        }
    }
}
