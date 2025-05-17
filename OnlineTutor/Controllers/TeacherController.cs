using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineTutor.Models;

namespace OnlineTutor.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class TeacherController : Controller
    {
        private readonly UserManager<User> _userManager;

        public TeacherController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        // GET: /Teacher
        public IActionResult Index()
        {
            return View();
        }
    }
}
