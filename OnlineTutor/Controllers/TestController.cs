using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineTutor.Data;
using OnlineTutor.Models;
using OnlineTutor.Models.ViewModels;

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

        // GET: /Test - главная страница модуля тестов
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            // Получаем статистику по разным типам тестов
            var spellingTestsCount = await _context.SpellingTests
                .Where(t => t.TeacherId == currentUser.Id)
                .CountAsync();

            var regularTestsCount = await _context.Tests
                .Where(t => t.TeacherId == currentUser.Id)
                .CountAsync();

            var model = new TestModuleIndexViewModel
            {
                SpellingTestsCount = spellingTestsCount,
                RegularTestsCount = regularTestsCount
            };

            return View(model);
        }

        // GET: /Test/Regular - обычные тесты (существующий функционал)
        public async Task<IActionResult> Regular()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var tests = await _context.Tests
                .Where(t => t.TeacherId == currentUser.Id)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();

            return View(tests);
        }

        // Остальные методы для обычных тестов остаются без изменений
        // Просто добавьте их сюда из старого TestController...

        // GET: /Test/CreateRegular
        public IActionResult CreateRegular()
        {
            return View(new TestViewModel());
        }

        // POST: /Test/CreateRegular
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRegular(TestViewModel model)
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

                return RedirectToAction(nameof(EditRegular), new { id = test.Id });
            }

            return View(model);
        }

        // GET: /Test/EditRegular/5
        public async Task<IActionResult> EditRegular(int id)
        {
            var test = await _context.Tests
                .Include(t => t.Questions)
                .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (test == null)
            {
                return NotFound();
            }

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

        // Добавьте остальные методы для обычных тестов с префиксом Regular...
    }

    // Методы для тестов правописания

// GET: /Test/Spelling
public async Task<IActionResult> Spelling()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var tests = await _context.SpellingTests
                .Where(t => t.TeacherId == currentUser.Id)
                .Include(t => t.Words)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();

            return View(tests);
        }

        // GET: /Test/CreateSpelling
        public IActionResult CreateSpelling()
        {
            return View(new SpellingTestViewModel());
        }

        // POST: /Test/CreateSpelling
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSpelling(SpellingTestViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);

                var test = new SpellingTest
                {
                    Title = model.Title,
                    Description = model.Description,
                    Instructions = model.Instructions,
                    TimeLimit = model.TimeLimit,
                    IsPublished = model.IsPublished,
                    CreatedDate = DateTime.Now,
                    TeacherId = currentUser.Id
                };

                _context.SpellingTests.Add(test);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(EditSpelling), new { id = test.Id });
            }

            return View(model);
        }

        // GET: /Test/EditSpelling/5
        public async Task<IActionResult> EditSpelling(int id)
        {
            var test = await _context.SpellingTests
                .Include(t => t.Words)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (test == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (test.TeacherId != currentUser.Id)
            {
                return Forbid();
            }

            var model = new SpellingTestViewModel
            {
                Id = test.Id,
                Title = test.Title,
                Description = test.Description,
                Instructions = test.Instructions,
                TimeLimit = test.TimeLimit,
                IsPublished = test.IsPublished
            };

            return View(model);
        }

        // GET: /Test/SpellingWords/5
        public async Task<IActionResult> SpellingWords(int id)
        {
            var test = await _context.SpellingTests
                .Include(t => t.Words.OrderBy(w => w.OrderIndex))
                .FirstOrDefaultAsync(t => t.Id == id);

            if (test == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (test.TeacherId != currentUser.Id)
            {
                return Forbid();
            }

            ViewBag.TestId = id;
            ViewBag.TestTitle = test.Title;

            return View(test.Words.ToList());
        }

        // GET: /Test/AddSpellingWord/5
        public IActionResult AddSpellingWord(int testId)
        {
            var model = new SpellingWordViewModel
            {
                TestId = testId,
                OrderIndex = 1
            };

            return View(model);
        }

        // POST: /Test/AddSpellingWord
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSpellingWord(SpellingWordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var test = await _context.SpellingTests.FindAsync(model.TestId);
                if (test == null)
                {
                    return NotFound();
                }

                var currentUser = await _userManager.GetUserAsync(User);
                if (test.TeacherId != currentUser.Id)
                {
                    return Forbid();
                }

                var word = new SpellingWord
                {
                    TestId = model.TestId,
                    WordWithGap = model.WordWithGap,
                    CorrectLetter = model.CorrectLetter,
                    FullWord = model.FullWord,
                    Hint = model.Hint,
                    OrderIndex = model.OrderIndex
                };

                _context.SpellingWords.Add(word);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(SpellingWords), new { id = model.TestId });
            }

            return View(model);
        }

        // GET: /Test/ImportSpellingWords
        public IActionResult ImportSpellingWords()
        {
            ViewBag.Tests = _context.SpellingTests.ToList();
            return View();
        }

        // POST: /Test/ImportSpellingWords
        [HttpPost]
        public async Task<IActionResult> ImportSpellingWords(IFormFile excelFile, int testId)
        {
            // Код импорта остается тот же, что был в SpellingTestController
            // ...
            return RedirectToAction("SpellingWords", new { id = testId });
        }

        // Добавьте остальные методы для тестов правописания...

    }
