using Microsoft.AspNetCore.Mvc;
using UserManagement.Web.Models;
using UserManagement.Web.Services;

namespace UserManagement.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserApiService _userApi;

        public UsersController(UserApiService userApi)
        {
            _userApi = userApi;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var users = await _userApi.GetAllAsync();
                return View(users);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] =
                    "Unable to connect to the API. Make sure the backend is running.";

                return View(new List<UserViewModel>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var user = await _userApi.GetByIdAsync(id);

                if (user is null)
                    return NotFound();

                return View(user);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Unable to retrieve the user.";
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Create()
        {
            return View(new CreateUserViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                using var response = await _userApi.CreateAsync(model);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "User created successfully.";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(
                    string.Empty,
                    response.StatusCode == System.Net.HttpStatusCode.Conflict
                        ? "This email may already be registered."
                        : "Unable to create the user. Please try again.");
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to connect to the API.");
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var user = await _userApi.GetByIdAsync(id);

                if (user is null)
                    return NotFound();

                var model = new UpdateUserViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    IsActive = user.IsActive
                };

                return View(model);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Unable to retrieve the user.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id, UpdateUserViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                using var response = await _userApi.UpdateAsync(model);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "User updated successfully.";
                    return RedirectToAction(nameof(Index));
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return NotFound();

                ModelState.AddModelError(
                    string.Empty,
                    response.StatusCode == System.Net.HttpStatusCode.Conflict
                        ? "This email may already be registered."
                        : "Unable to update the user. Please try again.");
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to connect to the API.");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                using var response = await _userApi.DeleteAsync(id);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "User deleted successfully.";
                }
                else if (response.StatusCode ==
                         System.Net.HttpStatusCode.NotFound)
                {
                    TempData["Error"] = "User not found.";
                }
                else
                {
                    TempData["Error"] = "Unable to delete the user.";
                }
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Unable to connect to the API.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
