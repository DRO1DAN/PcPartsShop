using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PcPartsShopDomain.Model;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic; // Required by View

namespace PcPartsShopInfrastructure.Controllers
{
    // *** Authorize only SuperAdmins for this entire controller ***
    [Authorize(Roles = "SuperAdmin")]
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager; // Keep RoleManager if needed for checks

        public AdminController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Admin/UserList
        public async Task<IActionResult> UserList()
        {
            // Passing the domain User objects directly to the view
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        // POST: Admin/MakeAdmin - Only SuperAdmin can trigger this
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeAdmin(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) { TempData["ErrorMessage"] = "User not found."; return RedirectToAction("UserList"); }

            // Ensure Admin role exists (good practice)
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                TempData["ErrorMessage"] = "Admin role does not exist. Please seed roles.";
                return RedirectToAction("UserList");
            }

            // Check if user is already an Admin
            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                TempData["InfoMessage"] = $"User {user.UserName} is already in the Admin role.";
                return RedirectToAction("UserList");
            }

            var result = await _userManager.AddToRoleAsync(user, "Admin");
            if (result.Succeeded) { TempData["SuccessMessage"] = $"User {user.UserName} successfully added to Admin role."; }
            else { TempData["ErrorMessage"] = $"Error adding Admin role: {string.Join(", ", result.Errors.Select(e => e.Description))}"; }
            return RedirectToAction("UserList");
        }

        // POST: Admin/RemoveAdmin - Only SuperAdmin can trigger this
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveAdmin(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) { TempData["ErrorMessage"] = "User not found."; return RedirectToAction("UserList"); }

            // *** Crucially, SuperAdmin manages the "Admin" role, not "SuperAdmin" role here ***
            // We are removing the *Admin* role, even if the target user might *also* be a SuperAdmin.
            // However, the self-check prevents a SuperAdmin removing their own Admin role if they have it.

            var currentUserId = _userManager.GetUserId(User);
            if (user.Id == currentUserId)
            {
                // Technically, a SuperAdmin *could* remove their own *Admin* role if they wanted,
                // but it might be confusing. Let's prevent it for simplicity, assuming SuperAdmin implies Admin.
                // If you want to allow this, remove this self-check for this specific action.
                TempData["ErrorMessage"] = "You cannot remove roles from yourself.";
                return RedirectToAction("UserList");
            }

            // Check if user is actually in the Admin role before trying to remove
            if (!await _userManager.IsInRoleAsync(user, "Admin"))
            {
                TempData["InfoMessage"] = $"User {user.UserName} is not in the Admin role.";
                return RedirectToAction("UserList");
            }


            var result = await _userManager.RemoveFromRoleAsync(user, "Admin");
            if (result.Succeeded) { TempData["SuccessMessage"] = $"User {user.UserName} successfully removed from Admin role."; }
            else { TempData["ErrorMessage"] = $"Error removing Admin role: {string.Join(", ", result.Errors.Select(e => e.Description))}"; }
            return RedirectToAction("UserList");
        }

        // POST: Admin/DeleteUser - Only SuperAdmin can trigger this
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) { TempData["ErrorMessage"] = "User not found."; return RedirectToAction("UserList"); }

            var currentUserId = _userManager.GetUserId(User);
            if (user.Id == currentUserId)
            {
                TempData["ErrorMessage"] = "You cannot delete your own account.";
                return RedirectToAction("UserList");
            }

            // *** Prevent deleting other SuperAdmins (Recommended) ***
            if (await _userManager.IsInRoleAsync(user, "SuperAdmin"))
            {
                TempData["ErrorMessage"] = "Cannot delete another SuperAdmin account.";
                return RedirectToAction("UserList");
            }


            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded) { TempData["SuccessMessage"] = $"User {user.UserName} successfully deleted."; }
            else { TempData["ErrorMessage"] = $"Error deleting user: {string.Join(", ", result.Errors.Select(e => e.Description))}"; }
            return RedirectToAction("UserList");
        }
    }
}