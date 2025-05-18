using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineTutor.Data;
using OnlineTutor.Models;
using OnlineTutor.Models.ViewModels;
using System.Text.RegularExpressions;
using OfficeOpenXml;
using System.Text;

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
                DateOfBirth = DateTime.Now.AddYears(-15) // Устанавливаем примерную дату рождения для ученика
            };

            // Получаем уникальные названия классов из базы данных
            var gradeNames = await _context.StudentProfiles
                .Where(sp => !string.IsNullOrEmpty(sp.Grade))
                .Select(sp => sp.Grade)
                .Distinct()
                .OrderBy(g => g)
                .ToListAsync();

            model.AvailableClassNames = gradeNames;

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

                    // Получаем уникальные названия классов из базы данных
                    var gradeNames = await _context.StudentProfiles
                        .Where(sp => !string.IsNullOrEmpty(sp.Grade))
                        .Select(sp => sp.Grade)
                        .Distinct()
                        .OrderBy(g => g)
                        .ToListAsync();

                    model.AvailableClassNames = gradeNames;

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

                    model.AvailableClassNames = gradeNames;

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

                    model.AvailableClassNames = gradeNames;

                    return View(model);
                }

                // Создаем пользователя
                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
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
                        DateOfBirth = model.DateOfBirth
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
            // Получаем уникальные названия классов из базы данных
            var classNames = await _context.StudentProfiles
                .Where(sp => !string.IsNullOrEmpty(sp.Grade))
                .Select(sp => sp.Grade)
                .Distinct()
                .OrderBy(g => g)
                .ToListAsync();

            model.AvailableClassNames = classNames;

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

            // Получаем уникальные названия классов из базы данных
            var gradeNames = await _context.StudentProfiles
                .Where(sp => !string.IsNullOrEmpty(sp.Grade))
                .Select(sp => sp.Grade)
                .Distinct()
                .OrderBy(g => g)
                .ToListAsync();

            var model = new EditStudentViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                MiddleName = user.MiddleName,
                Email = user.Email,
                Grade = studentProfile.Grade,
                DateOfBirth = studentProfile.DateOfBirth,
                AvailableClassNames = gradeNames
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

                        // Получаем уникальные названия классов из базы данных
                        var gradeNames = await _context.StudentProfiles
                            .Where(sp => !string.IsNullOrEmpty(sp.Grade))
                            .Select(sp => sp.Grade)
                            .Distinct()
                            .OrderBy(g => g)
                            .ToListAsync();

                        model.AvailableClassNames = gradeNames;

                        return View(model);
                    }
                }

                // Валидация имени и фамилии
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

                    model.AvailableClassNames = gradeNames;

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

                    model.AvailableClassNames = gradeNames;

                    return View(model);
                }

                // Обновляем данные пользователя
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.MiddleName = model.MiddleName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                user.UserName = model.Email; // UserName должен совпадать с Email

                // Обновляем профиль студента
                studentProfile.Grade = model.Grade;
                studentProfile.DateOfBirth = model.DateOfBirth;

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
                            // Получаем уникальные названия классов из базы данных
                            var gradeNames = await _context.StudentProfiles
                                .Where(sp => !string.IsNullOrEmpty(sp.Grade))
                                .Select(sp => sp.Grade)
                                .Distinct()
                                .OrderBy(g => g)
                                .ToListAsync();

                            model.AvailableClassNames = gradeNames;

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

            // Получаем уникальные названия классов из базы данных
            var classNames = await _context.StudentProfiles
                .Where(sp => !string.IsNullOrEmpty(sp.Grade))
                .Select(sp => sp.Grade)
                .Distinct()
                .OrderBy(g => g)
                .ToListAsync();

            model.AvailableClassNames = classNames;

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

        [HttpGet]
        public IActionResult ImportStudents()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ImportStudents(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                ModelState.AddModelError("", "Файл не выбран");
                return View();
            }

            var students = new List<StudentImportDto>();

            // Для работы с кириллицей
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using (var stream = new MemoryStream())
            {
                await excelFile.CopyToAsync(stream);

                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // Первый лист

                    // Начинаем с 2-й строки, предполагая что 1-я - заголовки
                    for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                    {
                        try
                        {
                            var student = new StudentImportDto
                            {
                                LastName = worksheet.Cells[row, 1].Value?.ToString(),
                                FirstName = worksheet.Cells[row, 2].Value?.ToString(),
                                MiddleName = worksheet.Cells[row, 3].Value?.ToString(),
                                Email = worksheet.Cells[row, 4].Value?.ToString(),
                                PhoneNumber = worksheet.Cells[row, 5].Value?.ToString(),
                                Grade = worksheet.Cells[row, 6].Value?.ToString(),
                                DateOfBirth = Convert.ToDateTime(worksheet.Cells[row, 7].Value)
                            };

                            students.Add(student);
                        }
                        catch (Exception ex)
                        {
                            // Логируем ошибку для строки
                            TempData["ImportErrors"] += $"Ошибка в строке {row}: {ex.Message}\n";
                        }
                    }
                }
            }

            // Массовое добавление учеников
            int successCount = 0;
            int errorCount = 0;

            foreach (var studentDto in students)
            {
                try
                {
                    var user = new User
                    {
                        UserName = studentDto.Email,
                        Email = studentDto.Email,
                        PhoneNumber = studentDto.PhoneNumber,
                        FirstName = studentDto.FirstName,
                        LastName = studentDto.LastName,
                        MiddleName = studentDto.MiddleName,
                        Role = UserRole.Student,
                        IsVerified = false
                    };

                    var result = await _userManager.CreateAsync(user, GenerateRandomPassword());

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Student");

                        var studentProfile = new StudentProfile
                        {
                            UserId = user.Id,
                            Grade = studentDto.Grade,
                            DateOfBirth = studentDto.DateOfBirth
                        };

                        _context.StudentProfiles.Add(studentProfile);
                        successCount++;
                    }
                    else
                    {
                        errorCount++;
                        TempData["ImportErrors"] += string.Join("\n", result.Errors.Select(e => e.Description));
                    }
                }
                catch (Exception ex)
                {
                    errorCount++;
                    TempData["ImportErrors"] += $"Ошибка при добавлении {studentDto.Email}: {ex.Message}\n";
                }
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Импорт завершен. Добавлено: {successCount}, Ошибок: {errorCount}";
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
