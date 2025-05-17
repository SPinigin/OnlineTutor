using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineTutor.Models;

namespace OnlineTutor.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly UserManager<User> _userManager;

        public StudentController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (!currentUser.IsVerified)
            {
                return RedirectToAction("AwaitingVerification");
            }

            return View();
        }

        public IActionResult AwaitingVerification()
        {
            return View();
        }
    }
}
