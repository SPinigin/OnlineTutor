using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineTutor.Models.ViewModels;
using OnlineTutor.Models;
using System.Text.RegularExpressions;

namespace OnlineTutor.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
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
                    return View(model);
                }

                // Валидация имени и фамилии (только буквы)
                if (!Regex.IsMatch(model.FirstName, @"^[а-яА-ЯёЁa-zA-Z\s-]+$") ||
                    !Regex.IsMatch(model.LastName, @"^[а-яА-ЯёЁa-zA-Z\s-]+$"))
                {
                    ModelState.AddModelError(string.Empty, "Имя и фамилия должны содержать только буквы.");
                    return View(model);
                }

                // Валидация отчества, если оно указано
                if (!string.IsNullOrEmpty(model.MiddleName) &&
                    !Regex.IsMatch(model.MiddleName, @"^[а-яА-ЯёЁa-zA-Z\s-]+$"))
                {
                    ModelState.AddModelError(string.Empty, "Отчество должно содержать только буквы.");
                    return View(model);
                }

                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    MiddleName = model.MiddleName,
                    Role = model.Role,
                    EmailConfirmed = true // Временно подтверждаем email без проверки
                };

                // Добавление специфических полей в зависимости от роли
                if (model.Role == UserRole.Student)
                {
                    // Валидация для ученика
                    if (string.IsNullOrEmpty(model.Grade))
                    {
                        ModelState.AddModelError(string.Empty, "Необходимо указать класс для ученика.");
                        return View(model);
                    }

                    if (!model.DateOfBirth.HasValue)
                    {
                        ModelState.AddModelError(string.Empty, "Необходимо указать дату рождения для ученика.");
                        return View(model);
                    }

                    user.Grade = model.Grade;
                    user.DateOfBirth = model.DateOfBirth;
                }
                else if (model.Role == UserRole.Teacher)
                {
                    // Валидация для учителя
                    if (string.IsNullOrEmpty(model.Subject))
                    {
                        ModelState.AddModelError(string.Empty, "Необходимо указать предмет для учителя.");
                        return View(model);
                    }

                    if (!model.TeachingExperience.HasValue || model.TeachingExperience < 0)
                    {
                        ModelState.AddModelError(string.Empty, "Необходимо указать корректный опыт работы.");
                        return View(model);
                    }

                    if (string.IsNullOrEmpty(model.Education))
                    {
                        ModelState.AddModelError(string.Empty, "Необходимо указать образование.");
                        return View(model);
                    }

                    user.Subject = model.Subject;
                    user.TeachingExperience = model.TeachingExperience;
                    user.Education = model.Education;
                }

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Создание роли, если она не существует
                    string roleName = model.Role.ToString();
                    if (!await _roleManager.RoleExistsAsync(roleName))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(roleName));
                    }

                    // Добавление пользователя в роль
                    await _userManager.AddToRoleAsync(user, roleName);

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
        public IActionResult AccessDenied()
        {
            return View();
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
