using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineTutor.Models.ViewModels;
using OnlineTutor.Models;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using OnlineTutor.Data;

namespace OnlineTutor.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly ApplicationDbContext _context;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole<int>> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Register()
        {
            // Получаем уникальные названия классов из базы данных
            var gradeNames = await _context.StudentProfiles
                .Where(sp => !string.IsNullOrEmpty(sp.Grade))
                .Select(sp => sp.Grade)
                .Distinct()
                .OrderBy(g => g)
                .ToListAsync();

            ViewBag.AvailableClassNames = gradeNames;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Проверка существования email
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "Этот email уже зарегистрирован в системе.");

                    // Получаем уникальные названия классов из базы данных
                    var gradeNames = await _context.StudentProfiles
                        .Where(sp => !string.IsNullOrEmpty(sp.Grade))
                        .Select(sp => sp.Grade)
                        .Distinct()
                        .OrderBy(g => g)
                        .ToListAsync();

                    ViewBag.AvailableClassNames = gradeNames;

                    return View(model);
                }

                // Валидация имени и фамилии (только буквы)
                if (!Regex.IsMatch(model.FirstName, @"^[а-яА-ЯёЁa-zA-Z\s-]+$") ||
                    !Regex.IsMatch(model.LastName, @"^[а-яА-ЯёЁa-zA-Z\s-]+$"))
                {
                    ModelState.AddModelError(string.Empty, "Имя и фамилия должны содержать только буквы.");

                    // Получаем уникальные названия классов из базы данных
                    var gradeNames = await _context.StudentProfiles
                        .Where(sp => !string.IsNullOrEmpty(sp.Grade))
                        .Select(sp => sp.Grade)
                        .Distinct()
                        .OrderBy(g => g)
                        .ToListAsync();

                    ViewBag.AvailableClassNames = gradeNames;

                    return View(model);
                }

                // Валидация отчества, если оно указано
                if (!string.IsNullOrEmpty(model.MiddleName) &&
                    !Regex.IsMatch(model.MiddleName, @"^[а-яА-ЯёЁa-zA-Z\s-]+$"))
                {
                    ModelState.AddModelError(string.Empty, "Отчество должно содержать только буквы.");

                    // Получаем уникальные названия классов из базы данных
                    var gradeNames = await _context.StudentProfiles
                        .Where(sp => !string.IsNullOrEmpty(sp.Grade))
                        .Select(sp => sp.Grade)
                        .Distinct()
                        .OrderBy(g => g)
                        .ToListAsync();

                    ViewBag.AvailableClassNames = gradeNames;

                    return View(model);
                }

                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    MiddleName = model.MiddleName,
                    Role = model.Role,
                    EmailConfirmed = true,
                    IsVerified = true // Устанавливаем в true для всех пользователей
                };

                if (model.Role == UserRole.Student)
                {
                    // Устанавливаем флаг, что аккаунт создан самостоятельно и требует подтверждения
                    user.IsVerified = false; // Добавьте это поле в модель User
                }
                else
                {
                    // Для учителей и администраторов требуется дополнительная проверка
                    // или можно вообще запретить такую регистрацию
                    ModelState.AddModelError(string.Empty, "Самостоятельная регистрация доступна только для учеников.");

                    // Получаем уникальные названия классов из базы данных
                    var gradeNames = await _context.StudentProfiles
                        .Where(sp => !string.IsNullOrEmpty(sp.Grade))
                        .Select(sp => sp.Grade)
                        .Distinct()
                        .OrderBy(g => g)
                        .ToListAsync();

                    ViewBag.AvailableClassNames = gradeNames;

                    return View(model);
                }

                // Добавление специфических полей в зависимости от роли
                if (model.Role == UserRole.Student)
                {
                    if (!model.DateOfBirth.HasValue)
                    {
                        ModelState.AddModelError(string.Empty, "Необходимо указать дату рождения для ученика.");

                        // Получаем уникальные названия классов из базы данных
                        var gradeNames = await _context.StudentProfiles
                            .Where(sp => !string.IsNullOrEmpty(sp.Grade))
                            .Select(sp => sp.Grade)
                            .Distinct()
                            .OrderBy(g => g)
                            .ToListAsync();

                        ViewBag.AvailableClassNames = gradeNames;

                        return View(model);
                    }
                }
                else if (model.Role == UserRole.Teacher)
                {
                    // Валидация для учителя
                    if (string.IsNullOrEmpty(model.Subject))
                    {
                        ModelState.AddModelError(string.Empty, "Необходимо указать предмет для учителя.");

                        // Получаем уникальные названия классов из базы данных
                        var gradeNames = await _context.StudentProfiles
                            .Where(sp => !string.IsNullOrEmpty(sp.Grade))
                            .Select(sp => sp.Grade)
                            .Distinct()
                            .OrderBy(g => g)
                            .ToListAsync();

                        ViewBag.AvailableClassNames = gradeNames;

                        return View(model);
                    }

                    if (!model.TeachingExperience.HasValue || model.TeachingExperience < 0)
                    {
                        ModelState.AddModelError(string.Empty, "Необходимо указать корректный опыт работы.");

                        // Получаем уникальные названия классов из базы данных
                        var gradeNames = await _context.StudentProfiles
                            .Where(sp => !string.IsNullOrEmpty(sp.Grade))
                            .Select(sp => sp.Grade)
                            .Distinct()
                            .OrderBy(g => g)
                            .ToListAsync();

                        ViewBag.AvailableClassNames = gradeNames;

                        return View(model);
                    }

                    if (string.IsNullOrEmpty(model.Education))
                    {
                        ModelState.AddModelError(string.Empty, "Необходимо указать образование.");

                        // Получаем уникальные названия классов из базы данных
                        var gradeNames = await _context.StudentProfiles
                            .Where(sp => !string.IsNullOrEmpty(sp.Grade))
                            .Select(sp => sp.Grade)
                            .Distinct()
                            .OrderBy(g => g)
                            .ToListAsync();

                        ViewBag.AvailableClassNames = gradeNames;

                        return View(model);
                    }
                }

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Создание роли, если она не существует
                    string roleName = model.Role.ToString();
                    if (!await _roleManager.RoleExistsAsync(roleName))
                    {
                        await _roleManager.CreateAsync(new IdentityRole<int>(roleName));
                    }

                    // Добавление пользователя в роль
                    await _userManager.AddToRoleAsync(user, roleName);

                    // Создаем профиль в зависимости от роли
                    if (model.Role == UserRole.Student)
                    {
                        // Создаем профиль студента
                        var studentProfile = new StudentProfile
                        {
                            UserId = user.Id,
                            Grade = model.Grade,
                            DateOfBirth = model.DateOfBirth.Value
                        };

                        user.StudentProfile = studentProfile;
                        _context.StudentProfiles.Add(studentProfile);
                        await _context.SaveChangesAsync();
                    }
                    else if (model.Role == UserRole.Teacher)
                    {
                        // Создаем профиль учителя
                        var teacherProfile = new TeacherProfile
                        {
                            UserId = user.Id,
                            Subject = model.Subject,
                            TeachingExperience = model.TeachingExperience.Value,
                            Education = model.Education
                        };

                        user.TeacherProfile = teacherProfile;
                        _context.TeacherProfiles.Add(teacherProfile);
                        await _context.SaveChangesAsync();
                    }

                    // Можно добавить отправку email для подтверждения
                    // await SendConfirmationEmailAsync(user);

                    // Автоматический вход после регистрации
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // Перенаправление в зависимости от роли
                    if (model.Role == UserRole.Student)
                    {
                        return RedirectToAction("Index", "Student");
                    }
                    else if (model.Role == UserRole.Teacher)
                    {
                        return RedirectToAction("Index", "Teacher");
                    }

                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Получаем уникальные названия классов из базы данных
            var classNames = await _context.StudentProfiles
                .Where(sp => !string.IsNullOrEmpty(sp.Grade))
                .Select(sp => sp.Grade)
                .Distinct()
                .OrderBy(g => g)
                .ToListAsync();

            ViewBag.AvailableClassNames = classNames;

            // Если мы дошли до этой точки, что-то пошло не так, возвращаем форму
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    // Получаем пользователя для определения его роли
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        // Перенаправление в зависимости от роли
                        if (await _userManager.IsInRoleAsync(user, "Student"))
                        {
                            return RedirectToAction("Index", "Student");
                        }
                        else if (await _userManager.IsInRoleAsync(user, "Teacher"))
                        {
                            return RedirectToAction("Index", "Teacher");
                        }
                        else if (await _userManager.IsInRoleAsync(user, "Administrator"))
                        {
                            return RedirectToAction("Index", "Admin");
                        }
                    }

                    // Если нет специфичной роли или URL возврата
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction(nameof(HomeController.Index), "Home");
                }

                ModelState.AddModelError(string.Empty, "Неверный логин или пароль.");
                return View(model);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    // Не сообщаем пользователю, что email не существует
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                // Здесь можно добавить логику отправки email для сброса пароля
                // var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Scheme);
                // await _emailSender.SendEmailAsync(model.Email, "Сброс пароля", $"Для сброса пароля перейдите по ссылке: <a href='{callbackUrl}'>link</a>");

                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                return BadRequest("Для сброса пароля требуется код.");
            }
            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Не раскрываем информацию о том, что пользователь не существует
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // Профиль пользователя
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Загружаем профиль в зависимости от роли
            if (user.Role == UserRole.Student)
            {
                user.StudentProfile = await _context.StudentProfiles
                    .FirstOrDefaultAsync(sp => sp.UserId == user.Id);
            }
            else if (user.Role == UserRole.Teacher)
            {
                user.TeacherProfile = await _context.TeacherProfiles
                    .FirstOrDefaultAsync(tp => tp.UserId == user.Id);
            }

            var model = new ProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                MiddleName = user.MiddleName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role
            };

            // Заполняем данные в зависимости от роли
            if (user.Role == UserRole.Student && user.StudentProfile != null)
            {
                model.Grade = user.StudentProfile.Grade;
                model.DateOfBirth = user.StudentProfile.DateOfBirth;
                // Дополнительные поля студента
            }
            else if (user.Role == UserRole.Teacher && user.TeacherProfile != null)
            {
                model.Subject = user.TeacherProfile.Subject;
                model.TeachingExperience = user.TeacherProfile.TeachingExperience;
                model.Education = user.TeacherProfile.Education;
                // Дополнительные поля учителя
            }

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                // Обновляем базовые данные пользователя
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.MiddleName = model.MiddleName;
                user.PhoneNumber = model.PhoneNumber;

                // Обновляем данные профиля в зависимости от роли
                if (user.Role == UserRole.Student)
                {
                    var studentProfile = await _context.StudentProfiles
                        .FirstOrDefaultAsync(sp => sp.UserId == user.Id);

                    if (studentProfile == null)
                    {
                        studentProfile = new StudentProfile
                        {
                            UserId = user.Id
                        };
                        _context.StudentProfiles.Add(studentProfile);
                    }

                    studentProfile.Grade = model.Grade;
                    studentProfile.DateOfBirth = model.DateOfBirth.Value;
                    // Обновляем дополнительные поля студента
                }
                else if (user.Role == UserRole.Teacher)
                {
                    var teacherProfile = await _context.TeacherProfiles
                        .FirstOrDefaultAsync(tp => tp.UserId == user.Id);

                    if (teacherProfile == null)
                    {
                        teacherProfile = new TeacherProfile
                        {
                            UserId = user.Id
                        };
                        _context.TeacherProfiles.Add(teacherProfile);
                    }

                    teacherProfile.Subject = model.Subject;
                    teacherProfile.TeachingExperience = model.TeachingExperience.Value;
                    teacherProfile.Education = model.Education;
                    // Обновляем дополнительные поля учителя
                }

                // Сохраняем изменения в базе данных
                await _context.SaveChangesAsync();
                await _userManager.UpdateAsync(user);

                TempData["StatusMessage"] = "Ваш профиль был обновлен.";
                return RedirectToAction(nameof(Profile));
            }

            return View(model);
        }

        // Вспомогательные методы
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}
