using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OnlineTutor.Data;
using OnlineTutor.Models;
using OnlineTutor.Models.ViewModels;
using System.Text;

namespace OnlineTutor.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class TestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public TestController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Test
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var tests = await _context.Tests
                .Where(t => t.TeacherId == currentUser.Id)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();

            return View(tests);
        }

        // GET: /Test/Create
        public IActionResult Create()
        {
            return View(new TestViewModel());
        }

        // POST: /Test/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TestViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);

                var test = new Test
                {
                    Title = model.Title,
                    Description = model.Description,
                    Instructions = model.Instructions,
                    Subject = model.Subject,
                    Topic = model.Topic,
                    TimeLimit = model.TimeLimit,
                    PassingScore = model.PassingScore,
                    IsPublished = model.IsPublished,
                    CreatedDate = DateTime.Now,
                    TeacherId = currentUser.Id
                };

                _context.Tests.Add(test);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Edit), new { id = test.Id });
            }

            return View(model);
        }

        // GET: /Test/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var test = await _context.Tests
                .Include(t => t.Questions)
                .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (test == null)
            {
                return NotFound();
            }

            // Проверка прав доступа
            var currentUser = await _userManager.GetUserAsync(User);
            if (test.TeacherId != currentUser.Id)
            {
                return Forbid();
            }

            var model = new TestViewModel
            {
                Id = test.Id,
                Title = test.Title,
                Description = test.Description,
                Instructions = test.Instructions,
                Subject = test.Subject,
                Topic = test.Topic,
                TimeLimit = test.TimeLimit,
                PassingScore = test.PassingScore,
                IsPublished = test.IsPublished
            };

            return View(model);
        }

        // POST: /Test/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TestViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var test = await _context.Tests.FindAsync(id);

                    if (test == null)
                    {
                        return NotFound();
                    }

                    // Проверка прав доступа
                    var currentUser = await _userManager.GetUserAsync(User);
                    if (test.TeacherId != currentUser.Id)
                    {
                        return Forbid();
                    }

                    test.Title = model.Title;
                    test.Description = model.Description;
                    test.Instructions = model.Instructions;
                    test.Subject = model.Subject;
                    test.Topic = model.Topic;
                    test.TimeLimit = model.TimeLimit;
                    test.PassingScore = model.PassingScore;
                    test.IsPublished = model.IsPublished;

                    _context.Update(test);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TestExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: /Test/Questions/5
        public async Task<IActionResult> Questions(int id)
        {
            var test = await _context.Tests
                .Include(t => t.Questions.OrderBy(q => q.OrderIndex))
                .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (test == null)
            {
                return NotFound();
            }

            // Проверка прав доступа
            var currentUser = await _userManager.GetUserAsync(User);
            if (test.TeacherId != currentUser.Id)
            {
                return Forbid();
            }

            ViewBag.TestId = id;
            ViewBag.TestTitle = test.Title;

            return View(test.Questions.ToList());
        }

        // GET: /Test/AddQuestion/5
        public IActionResult AddQuestion(int testId)
        {
            var model = new QuestionViewModel
            {
                TestId = testId,
                OrderIndex = 1 // По умолчанию первый вопрос
            };

            return View(model);
        }

        // POST: /Test/AddQuestion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddQuestion(QuestionViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Проверка прав доступа к тесту
                var test = await _context.Tests.FindAsync(model.TestId);
                if (test == null)
                {
                    return NotFound();
                }

                var currentUser = await _userManager.GetUserAsync(User);
                if (test.TeacherId != currentUser.Id)
                {
                    return Forbid();
                }

                // Определяем порядковый номер вопроса
                var maxOrder = await _context.Questions
                    .Where(q => q.TestId == model.TestId)
                    .Select(q => (int?)q.OrderIndex)
                    .MaxAsync() ?? 0;

                var question = new Question
                {
                    TestId = model.TestId,
                    Text = model.Text,
                    Type = model.Type,
                    Points = model.Points,
                    OrderIndex = maxOrder + 1
                };

                _context.Questions.Add(question);
                await _context.SaveChangesAsync();

                // Добавляем варианты ответов (если есть)
                if (model.Options != null && model.Options.Any())
                {
                    foreach (var optionModel in model.Options)
                    {
                        var option = new QuestionOption
                        {
                            QuestionId = question.Id,
                            Text = optionModel.Text,
                            IsCorrect = optionModel.IsCorrect
                        };

                        _context.QuestionOptions.Add(option);
                    }

                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Questions), new { id = model.TestId });
            }

            return View(model);
        }

        // GET: /Test/EditQuestion/5
        public async Task<IActionResult> EditQuestion(int id)
        {
            var question = await _context.Questions
                .Include(q => q.Options)
                .Include(q => q.Test)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null)
            {
                return NotFound();
            }

            // Проверка прав доступа
            var currentUser = await _userManager.GetUserAsync(User);
            if (question.Test.TeacherId != currentUser.Id)
            {
                return Forbid();
            }

            var model = new QuestionViewModel
            {
                Id = question.Id,
                TestId = question.TestId,
                Text = question.Text,
                Type = question.Type,
                Points = question.Points,
                OrderIndex = question.OrderIndex,
                Options = question.Options.Select(o => new QuestionOptionViewModel
                {
                    Id = o.Id,
                    Text = o.Text,
                    IsCorrect = o.IsCorrect
                }).ToList()
            };

            return View(model);
        }

        // POST: /Test/EditQuestion/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditQuestion(int id, QuestionViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var question = await _context.Questions
                        .Include(q => q.Options)
                        .Include(q => q.Test)
                        .FirstOrDefaultAsync(q => q.Id == id);

                    if (question == null)
                    {
                        return NotFound();
                    }

                    // Проверка прав доступа
                    var currentUser = await _userManager.GetUserAsync(User);
                    if (question.Test.TeacherId != currentUser.Id)
                    {
                        return Forbid();
                    }

                    question.Text = model.Text;
                    question.Type = model.Type;
                    question.Points = model.Points;

                    // Обновляем существующие опции и добавляем новые
                    var existingOptions = question.Options.ToList();

                    // Удаляем опции, которых нет в модели
                    foreach (var existingOption in existingOptions)
                    {
                        var modelOption = model.Options.FirstOrDefault(o => o.Id == existingOption.Id);
                        if (modelOption == null)
                        {
                            _context.QuestionOptions.Remove(existingOption);
                        }
                    }

                    // Обновляем существующие и добавляем новые опции
                    foreach (var optionModel in model.Options)
                    {
                        if (optionModel.Id > 0)
                        {
                            // Обновляем существующую опцию
                            var existingOption = existingOptions.FirstOrDefault(o => o.Id == optionModel.Id);
                            if (existingOption != null)
                            {
                                existingOption.Text = optionModel.Text;
                                existingOption.IsCorrect = optionModel.IsCorrect;
                                _context.Update(existingOption);
                            }
                        }
                        else
                        {
                            // Добавляем новую опцию
                            var newOption = new QuestionOption
                            {
                                QuestionId = question.Id,
                                Text = optionModel.Text,
                                IsCorrect = optionModel.IsCorrect
                            };
                            _context.QuestionOptions.Add(newOption);
                        }
                    }

                    _context.Update(question);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Questions), new { id = model.TestId });
            }

            return View(model);
        }

        // GET: /Test/Assign/5
        public async Task<IActionResult> Assign(int id)
        {
            var test = await _context.Tests.FindAsync(id);
            if (test == null)
            {
                return NotFound();
            }

            // Проверка прав доступа
            var currentUser = await _userManager.GetUserAsync(User);
            if (test.TeacherId != currentUser.Id)
            {
                return Forbid();
            }

            // Получаем классы, где текущий учитель является преподавателем
            var classes = await _context.Classes
                .Where(c => c.TeacherId == currentUser.Id)
                .ToListAsync();

            // Получаем учеников
            var students = await _userManager.GetUsersInRoleAsync("Student");

            var model = new TestAssignmentViewModel
            {
                TestId = id,
                TestTitle = test.Title,
                AvailableClasses = classes,
                AvailableStudents = students.ToList()
            };

            return View(model);
        }

        // POST: /Test/Assign
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(TestAssignmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var test = await _context.Tests.FindAsync(model.TestId);
                if (test == null)
                {
                    return NotFound();
                }

                // Проверка прав доступа
                var currentUser = await _userManager.GetUserAsync(User);
                if (test.TeacherId != currentUser.Id)
                {
                    return Forbid();
                }

                // Проверяем, что выбран либо класс, либо ученик
                if (model.ClassId == null && model.StudentId == null)
                {
                    ModelState.AddModelError("", "Необходимо выбрать класс или ученика");

                    // Повторно заполняем списки для выбора
                    model.AvailableClasses = await _context.Classes
                        .Where(c => c.TeacherId == currentUser.Id)
                        .ToListAsync();

                    model.AvailableStudents = (await _userManager.GetUsersInRoleAsync("Student")).ToList();

                    return View(model);
                }

                var assignment = new TestAssignment
                {
                    TestId = model.TestId,
                    ClassId = model.ClassId,
                    StudentId = model.StudentId,
                    AssignedDate = DateTime.Now,
                    DueDate = model.DueDate
                };

                _context.TestAssignments.Add(assignment);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: /Test/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var test = await _context.Tests
                .Include(t => t.Questions)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (test == null)
            {
                return NotFound();
            }

            // Проверка прав доступа
            var currentUser = await _userManager.GetUserAsync(User);
            if (test.TeacherId != currentUser.Id)
            {
                return Forbid();
            }

            return View(test);
        }

        // POST: /Test/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var test = await _context.Tests.FindAsync(id);

            if (test == null)
            {
                return NotFound();
            }

            // Проверка прав доступа
            var currentUser = await _userManager.GetUserAsync(User);
            if (test.TeacherId != currentUser.Id)
            {
                return Forbid();
            }

            _context.Tests.Remove(test);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /Test/DeleteQuestion/5
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var question = await _context.Questions
                .Include(q => q.Test)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null)
            {
                return NotFound();
            }

            // Проверка прав доступа
            var currentUser = await _userManager.GetUserAsync(User);
            if (question.Test.TeacherId != currentUser.Id)
            {
                return Forbid();
            }

            var testId = question.TestId;
            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();

            // Перенумеровываем оставшиеся вопросы
            var remainingQuestions = await _context.Questions
                .Where(q => q.TestId == testId)
                .OrderBy(q => q.OrderIndex)
                .ToListAsync();

            for (int i = 0; i < remainingQuestions.Count; i++)
            {
                remainingQuestions[i].OrderIndex = i + 1;
                _context.Update(remainingQuestions[i]);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Questions), new { id = testId });
        }

        // Вспомогательные методы
        private bool TestExists(int id)
        {
            return _context.Tests.Any(e => e.Id == id);
        }

        private bool QuestionExists(int id)
        {
            return _context.Questions.Any(e => e.Id == id);
        }

        // AJAX методы для управления вариантами ответов
        [HttpPost]
        public IActionResult AddOptionRow()
        {
            ViewBag.Index = 0; // Индекс будет заменен на клиенте
            return PartialView("_OptionRow", new QuestionOptionViewModel());
        }

        //[HttpGet]
        //public async Task<IActionResult> Topics()
        //{
        //    var topics = await _context.TestTopics
        //        .Include(t => t.Tests)
        //        .ToListAsync();
        //    return View(topics);
        //}

        //[HttpGet]
        //public async Task<IActionResult> Topics()
        //{
        //    var topics = await _context.TestTopics
        //        .Include(t => t.Tests)
        //        .ToListAsync();
        //    return View(topics);
        //}

        //[HttpGet]
        //public async Task<IActionResult> Groups()
        //{
        //    var groups = await _context.TestGroups.ToListAsync();
        //    return View(groups);
        //}

        [HttpGet]
        public async Task<IActionResult> CreateTopic()
        {
            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> CreateTopic(TestTopic model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.TestTopics.Add(model);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Topics));
        //    }
        //    return View(model);
        //}

        [HttpGet]
        public async Task<IActionResult> CreateGroup()
        {
            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> CreateGroup(TestGroup model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.TestGroups.Add(model);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Groups));
        //    }
        //    return View(model);
        //}

        //[HttpGet]
        //public async Task<IActionResult> Groups()
        //{
        //    var groups = await _context.TestGroups.ToListAsync();
        //    return View(groups);
        //}

        //[HttpGet]
        //public async Task<IActionResult> CreateTopic()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> CreateTopic(TestTopic model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.TestTopics.Add(model);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Topics));
        //    }
        //    return View(model);
        //}

        //[HttpGet]
        //public async Task<IActionResult> CreateGroup()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> AutoGroupTests()
        //{
        //    var tests = await _context.Tests.Include(t => t.Questions).ToListAsync();

        //    foreach (var test in tests)
        //    {
        //        test.AutoGroup();
        //    }

        //    await _context.SaveChangesAsync();

        //    return RedirectToAction(nameof(Index));
        //}

        //[HttpGet]
        //public async Task<IActionResult> FilterTests(string topic = null, string group = null)
        //{
        //    var query = _context.Tests.AsQueryable();

        //    if (!string.IsNullOrEmpty(topic))
        //    {
        //        query = query.Where(t => t.Topic.Name == topic);
        //    }

        //    if (!string.IsNullOrEmpty(group))
        //    {
        //        query = query.Where(t => t.Group.Name == group);
        //    }

        //    var tests = await query.ToListAsync();
        //    return View(tests);
        //}

        [HttpPost]
        //public async Task<IActionResult> CreateGroup(TestGroup model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.TestGroups.Add(model);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Groups));
        //    }
        //    return View(model);
        //}

        [HttpGet]
        public IActionResult ImportQuestions()
        {
            // Получаем список тестов для выбора
            ViewBag.Tests = _context.Tests.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ImportQuestions(IFormFile excelFile, int testId)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                ModelState.AddModelError("", "Файл не выбран");
                ViewBag.Tests = _context.Tests.ToList();
                return View();
            }

            var questions = new List<QuestionImportDto>();

            // Регистрация провайдера кодировок для работы с Excel
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using (var stream = new MemoryStream())
            {
                await excelFile.CopyToAsync(stream);

                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];

                    for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                    {
                        try
                        {
                            var question = new QuestionImportDto
                            {
                                Text = worksheet.Cells[row, 1].Value?.ToString(),
                                Type = ParseQuestionType(worksheet.Cells[row, 2].Value?.ToString()),
                                Points = Convert.ToInt32(worksheet.Cells[row, 3].Value),
                                Options = new List<QuestionOptionImportDto>()
                            };

                            // Парсинг вариантов ответов
                            if (question.Type == QuestionType.SingleChoice ||
                                question.Type == QuestionType.MultipleChoice)
                            {
                                for (int col = 4; col <= worksheet.Dimension.Columns; col += 2)
                                {
                                    var optionText = worksheet.Cells[row, col].Value?.ToString();
                                    var isCorrect = worksheet.Cells[row, col + 1].Value?.ToString()?.ToLower() == "да";

                                    if (!string.IsNullOrEmpty(optionText))
                                    {
                                        question.Options.Add(new QuestionOptionImportDto
                                        {
                                            Text = optionText,
                                            IsCorrect = isCorrect
                                        });
                                    }
                                }
                            }

                            questions.Add(question);
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", $"Ошибка в строке {row}: {ex.Message}");
                        }
                    }
                }
            }

            // Сохранение вопросов
            int successCount = 0;
            int errorCount = 0;

            foreach (var questionDto in questions)
            {
                try
                {
                    var question = new Question
                    {
                        TestId = testId,
                        Text = questionDto.Text,
                        Type = questionDto.Type,
                        Points = questionDto.Points,
                        OrderIndex = _context.Questions.Count(q => q.TestId == testId) + 1
                    };

                    _context.Questions.Add(question);
                    await _context.SaveChangesAsync();

                    // Добавление вариантов ответов
                    if (questionDto.Options != null)
                    {
                        foreach (var optionDto in questionDto.Options)
                        {
                            var option = new QuestionOption
                            {
                                QuestionId = question.Id,
                                Text = optionDto.Text,
                                IsCorrect = optionDto.IsCorrect
                            };

                            _context.QuestionOptions.Add(option);
                        }

                        await _context.SaveChangesAsync();
                    }

                    successCount++;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Ошибка при импорте вопроса: {ex.Message}");
                    errorCount++;
                }
            }

            TempData["ImportResult"] = $"Импорт завершен. Успешно добавлено: {successCount}, Ошибок: {errorCount}";
            return RedirectToAction("Questions", new { id = testId });
        }

        private QuestionType ParseQuestionType(string type)
        {
            return type?.ToLower() switch
            {
                "один из многих" => QuestionType.SingleChoice,
                "многие из многих" => QuestionType.MultipleChoice,
                "короткий ответ" => QuestionType.ShortAnswer,
                "развернутый ответ" => QuestionType.Essay,
                _ => QuestionType.SingleChoice // По умолчанию
            };
        }

        [HttpGet]
        public IActionResult CreateManualQuestion(int testId)
        {
            var model = new QuestionViewModel
            {
                TestId = testId,
                Options = new List<QuestionOptionViewModel>
        {
            new QuestionOptionViewModel()
        }
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateManualQuestion(QuestionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var question = new Question
                {
                    TestId = model.TestId,
                    Text = model.Text,
                    Type = model.Type,
                    Points = model.Points,
                    OrderIndex = _context.Questions.Count(q => q.TestId == model.TestId) + 1
                };

                _context.Questions.Add(question);
                await _context.SaveChangesAsync();

                // Добавление вариантов ответов
                if (model.Type == QuestionType.SingleChoice ||
                    model.Type == QuestionType.MultipleChoice)
                {
                    foreach (var optionModel in model.Options)
                    {
                        var option = new QuestionOption
                        {
                            QuestionId = question.Id,
                            Text = optionModel.Text,
                            IsCorrect = optionModel.IsCorrect
                        };

                        _context.QuestionOptions.Add(option);
                    }

                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Questions", new { id = model.TestId });
            }

            return View(model);
        }
    }
}
