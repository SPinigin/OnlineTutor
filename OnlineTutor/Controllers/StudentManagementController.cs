using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineTutor.Data;
using OnlineTutor.Models;
using OnlineTutor.Models.ViewModels;
using System.Text.RegularExpressions;

namespace OnlineTutor.Controllers
{
    [Authorize(Roles = "Administrator,Teacher")]
    public class StudentManagementController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly ApplicationDbContext _context;

        public StudentManagementController(
            UserManager<User> userManager,
            RoleManager<IdentityRole<int>> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // GET: /StudentManagement
        public async Task<IActionResult> Index()
        {
            var students = await _userManager.GetUsersInRoleAsync("Student");

            // Создаем список для результатов
            var studentList = new List<StudentListViewModel>();

            // Используем цикл для последовательной обработки каждого студента
            foreach (var student in students)
            {
                // Получаем профиль студента
                var profile = await _context.StudentProfiles
                    .Include(sp => sp.Class)
                    .FirstOrDefaultAsync(sp => sp.UserId == student.Id);

                // Создаем модель представления
                var viewModel = new StudentListViewModel
                {
                    Id = student.Id,
                    FullName = student.FullName,
                    Email = student.Email,
                    Grade = profile?.Grade ?? "-",
                    ClassName = profile?.Class?.Name ?? "-",
                    IsVerified = student.IsVerified // Добавляем это свойство
                };

                studentList.Add(viewModel);
            }

            // Сортируем результаты
            return View(studentList.OrderBy(s => s.FullName).ToList());
        }

        // GET: /StudentManagement/Create
        public async Task<IActionResult> Create()
        {
            var model = new CreateStudentViewModel
            {
                AvailableClasses = await _context.Classes.OrderBy(c => c.Name).ToListAsync(),
                DateOfBirth = DateTime.Now.AddYears(-15) // Устанавливаем примерную дату рождения для ученика
            };

            return View(model);
        }

        // POST: /StudentManagement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateStudentViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Проверка существования email
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "Этот email уже зарегистрирован в системе.");
                    model.AvailableClasses = await _context.Classes.OrderBy(c => c.Name).ToListAsync();
                    return View(model);
                }

                // Валидация имени и фамилии (только буквы)
                if (!Regex.IsMatch(model.FirstName, @"^[а-яА-ЯёЁa-zA-Z\s-]+$") ||
                    !Regex.IsMatch(model.LastName, @"^[а-яА-ЯёЁa-zA-Z\s-]+$"))
                {
                    ModelState.AddModelError(string.Empty, "Имя и фамилия должны содержать только буквы.");
                    model.AvailableClasses = await _context.Classes.OrderBy(c => c.Name).ToListAsync();
                    return View(model);
                }

                // Валидация отчества, если оно указано
                if (!string.IsNullOrEmpty(model.MiddleName) &&
                    !Regex.IsMatch(model.MiddleName, @"^[а-яА-ЯёЁa-zA-Z\s-]+$"))
                {
                    ModelState.AddModelError(string.Empty, "Отчество должно содержать только буквы.");
                    model.AvailableClasses = await _context.Classes.OrderBy(c => c.Name).ToListAsync();
                    return View(model);
                }

                // Создаем пользователя
                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    MiddleName = model.MiddleName,
                    Role = UserRole.Student,
                    EmailConfirmed = true,
                    IsVerified = true // Устанавливаем в true для новых учеников, добавленных через админ-панель
                };

                // Создаем пароль (либо используем заданный, либо генерируем)
                var password = !string.IsNullOrEmpty(model.Password) ? model.Password : GenerateRandomPassword();
                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    // Добавляем в роль Student
                    await _userManager.AddToRoleAsync(user, "Student");

                    // Создаем профиль студента
                    var studentProfile = new StudentProfile
                    {
                        UserId = user.Id,
                        Grade = model.Grade,
                        DateOfBirth = model.DateOfBirth,
                        ClassId = model.ClassId
                    };

                    _context.StudentProfiles.Add(studentProfile);
                    await _context.SaveChangesAsync();

                    // Если пароль был сгенерирован, сохраняем его для отображения
                    if (string.IsNullOrEmpty(model.Password))
                    {
                        TempData["GeneratedPassword"] = password;
                        TempData["SuccessMessage"] = $"Ученик успешно добавлен. Сгенерированный пароль: {password}";
                    }
                    else
                    {
                        TempData["SuccessMessage"] = "Ученик успешно добавлен.";
                    }

                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Если мы дошли до этой точки, что-то пошло не так, возвращаем форму
            model.AvailableClasses = await _context.Classes.OrderBy(c => c.Name).ToListAsync();
            return View(model);
        }

        // GET: /StudentManagement/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            var studentProfile = await _context.StudentProfiles
                .FirstOrDefaultAsync(sp => sp.UserId == id);

            if (studentProfile == null)
            {
                return NotFound("Профиль студента не найден");
            }

            var model = new EditStudentViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                MiddleName = user.MiddleName,
                Email = user.Email,
                Grade = studentProfile.Grade,
                DateOfBirth = studentProfile.DateOfBirth,
                ClassId = studentProfile.ClassId,
                AvailableClasses = await _context.Classes.OrderBy(c => c.Name).ToListAsync()
            };

            return View(model);
        }

        // POST: /StudentManagement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditStudentViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user == null)
                {
                    return NotFound();
                }

                var studentProfile = await _context.StudentProfiles
                    .FirstOrDefaultAsync(sp => sp.UserId == id);

                if (studentProfile == null)
                {
                    return NotFound("Профиль студента не найден");
                }

                // Проверка email, если он изменился
                if (user.Email != model.Email)
                {
                    var existingUser = await _userManager.FindByEmailAsync(model.Email);
                    if (existingUser != null && existingUser.Id != id)
                    {
                        ModelState.AddModelError(string.Empty, "Этот email уже зарегистрирован в системе.");
                        model.AvailableClasses = await _context.Classes.OrderBy(c => c.Name).ToListAsync();
                        return View(model);
                    }
                }

                // Валидация имени и фамилии
                if (!Regex.IsMatch(model.FirstName, @"^[а-яА-ЯёЁa-zA-Z\s-]+$") ||
                    !Regex.IsMatch(model.LastName, @"^[а-яА-ЯёЁa-zA-Z\s-]+$"))
                {
                    ModelState.AddModelError(string.Empty, "Имя и фамилия должны содержать только буквы.");
                    model.AvailableClasses = await _context.Classes.OrderBy(c => c.Name).ToListAsync();
                    return View(model);
                }

                // Валидация отчества, если оно указано
                if (!string.IsNullOrEmpty(model.MiddleName) &&
                    !Regex.IsMatch(model.MiddleName, @"^[а-яА-ЯёЁa-zA-Z\s-]+$"))
                {
                    ModelState.AddModelError(string.Empty, "Отчество должно содержать только буквы.");
                    model.AvailableClasses = await _context.Classes.OrderBy(c => c.Name).ToListAsync();
                    return View(model);
                }

                // Обновляем данные пользователя
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.MiddleName = model.MiddleName;
                user.Email = model.Email;
                user.UserName = model.Email; // UserName должен совпадать с Email

                // Обновляем профиль студента
                studentProfile.Grade = model.Grade;
                studentProfile.DateOfBirth = model.DateOfBirth;
                studentProfile.ClassId = model.ClassId;

                // Сохраняем изменения
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    _context.Update(studentProfile);
                    await _context.SaveChangesAsync();

                    // Если указан новый пароль, обновляем его
                    if (!string.IsNullOrEmpty(model.NewPassword))
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        var resetResult = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

                        if (!resetResult.Succeeded)
                        {
                            foreach (var error in resetResult.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                            model.AvailableClasses = await _context.Classes.OrderBy(c => c.Name).ToListAsync();
                            return View(model);
                        }
                    }

                    TempData["SuccessMessage"] = "Данные ученика успешно обновлены.";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            model.AvailableClasses = await _context.Classes.OrderBy(c => c.Name).ToListAsync();
            return View(model);
        }

        // GET: /StudentManagement/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            var studentProfile = await _context.StudentProfiles
                .Include(sp => sp.Class)
                .FirstOrDefaultAsync(sp => sp.UserId == id);

            if (studentProfile == null)
            {
                return NotFound("Профиль студента не найден");
            }

            var model = new StudentDetailsViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Grade = studentProfile.Grade,
                DateOfBirth = studentProfile.DateOfBirth,
                Age = studentProfile.Age,
                ClassName = studentProfile.Class?.Name ?? "-"
            };

            return View(model);
        }

        // GET: /StudentManagement/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            var studentProfile = await _context.StudentProfiles
                .Include(sp => sp.Class)
                .FirstOrDefaultAsync(sp => sp.UserId == id);

            if (studentProfile == null)
            {
                return NotFound("Профиль студента не найден");
            }

            var model = new StudentDetailsViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Grade = studentProfile.Grade,
                DateOfBirth = studentProfile.DateOfBirth,
                Age = studentProfile.Age,
                ClassName = studentProfile.Class?.Name ?? "-"
            };

            return View(model);
        }

        // POST: /StudentManagement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            var studentProfile = await _context.StudentProfiles
                .FirstOrDefaultAsync(sp => sp.UserId == id);

            if (studentProfile != null)
            {
                _context.StudentProfiles.Remove(studentProfile);
                await _context.SaveChangesAsync();
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Ученик успешно удален.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                TempData["ErrorMessage"] = error.Description;
            }

            return RedirectToAction(nameof(Index));
        }

        // Вспомогательный метод для генерации случайного пароля
        private string GenerateRandomPassword(int length = 8)
        {
            const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
            const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numbers = "0123456789";
            const string specialChars = "!$%^&*";

            var allChars = lowerChars + upperChars + numbers + specialChars;
            var random = new Random();
            var chars = new char[length];

            // Гарантируем, что пароль будет содержать хотя бы одну заглавную букву,
            // одну строчную букву, одну цифру и один специальный символ
            chars[0] = upperChars[random.Next(upperChars.Length)];
            chars[1] = lowerChars[random.Next(lowerChars.Length)];
            chars[2] = numbers[random.Next(numbers.Length)];
            chars[3] = specialChars[random.Next(specialChars.Length)];

            // Заполняем остальные символы случайным образом
            for (int i = 4; i < length; i++)
            {
                chars[i] = allChars[random.Next(allChars.Length)];
            }

            // Перемешиваем символы
            for (int i = 0; i < length; i++)
            {
                int swapIndex = random.Next(length);
                char temp = chars[i];
                chars[i] = chars[swapIndex];
                chars[swapIndex] = temp;
            }

            return new string(chars);
        }
    }
}
