using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineTutor.Data;
using OnlineTutor.Models;
using OnlineTutor.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineTutor.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class ClassController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public ClassController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Class
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var classes = await _context.Classes
                .Include(c => c.Students)
                .Where(c => c.TeacherId == currentUser.Id)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(classes);
        }

        // GET: /Class/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var classItem = await _context.Classes
                .Include(c => c.Students)
                .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(c => c.Id == id && c.TeacherId == currentUser.Id);

            if (classItem == null)
            {
                return NotFound();
            }

            // Получаем всех учеников, которые еще не в этом классе
            var studentsInClass = classItem.Students.Select(s => s.UserId).ToList();
            var availableStudents = await _userManager.GetUsersInRoleAsync("Student");
            var studentsToAdd = availableStudents
                .Where(s => !studentsInClass.Contains(s.Id))
                .ToList();

            var model = new ClassDetailsViewModel
            {
                Class = classItem,
                AvailableStudents = studentsToAdd
            };

            return View(model);
        }

        // GET: /Class/Create
        public IActionResult Create()
        {
            return View(new ClassViewModel());
        }

        // POST: /Class/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClassViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);

                var classItem = new Class
                {
                    Name = model.Name,
                    Year = model.Year,
                    TeacherId = currentUser.Id
                };

                _context.Classes.Add(classItem);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: /Class/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var classItem = await _context.Classes
                .FirstOrDefaultAsync(c => c.Id == id && c.TeacherId == currentUser.Id);

            if (classItem == null)
            {
                return NotFound();
            }

            var model = new ClassViewModel
            {
                Id = classItem.Id,
                Name = classItem.Name,
                Year = classItem.Year
            };

            return View(model);
        }

        // POST: /Class/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClassViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var currentUser = await _userManager.GetUserAsync(User);
                    var classItem = await _context.Classes
                        .FirstOrDefaultAsync(c => c.Id == id && c.TeacherId == currentUser.Id);

                    if (classItem == null)
                    {
                        return NotFound();
                    }

                    classItem.Name = model.Name;
                    classItem.Year = model.Year;

                    _context.Update(classItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassExists(id))
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

        // GET: /Class/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var classItem = await _context.Classes
                .FirstOrDefaultAsync(c => c.Id == id && c.TeacherId == currentUser.Id);

            if (classItem == null)
            {
                return NotFound();
            }

            return View(classItem);
        }

        // POST: /Class/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var classItem = await _context.Classes
                .FirstOrDefaultAsync(c => c.Id == id && c.TeacherId == currentUser.Id);

            if (classItem == null)
            {
                return NotFound();
            }

            _context.Classes.Remove(classItem);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: /Class/AddStudent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStudent(int classId, int studentId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var classItem = await _context.Classes
                .FirstOrDefaultAsync(c => c.Id == classId && c.TeacherId == currentUser.Id);

            if (classItem == null)
            {
                return NotFound();
            }

            var student = await _userManager.FindByIdAsync(studentId.ToString());
            if (student == null)
            {
                return NotFound();
            }

            // Проверяем, есть ли у студента профиль, если нет - создаем
            var studentProfile = await _context.StudentProfiles
                .FirstOrDefaultAsync(sp => sp.UserId == studentId);

            if (studentProfile == null)
            {
                return NotFound("Профиль студента не найден");
            }

            // Проверяем, не добавлен ли студент уже в класс
            if (studentProfile.ClassId == classId)
            {
                return RedirectToAction(nameof(Details), new { id = classId });
            }

            // Обновляем профиль студента
            studentProfile.ClassId = classId;
            _context.Update(studentProfile);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = classId });
        }

        // POST: /Class/RemoveStudent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveStudent(int classId, int studentId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var classItem = await _context.Classes
                .FirstOrDefaultAsync(c => c.Id == classId && c.TeacherId == currentUser.Id);

            if (classItem == null)
            {
                return NotFound();
            }

            var studentProfile = await _context.StudentProfiles
                .FirstOrDefaultAsync(sp => sp.UserId == studentId && sp.ClassId == classId);

            if (studentProfile == null)
            {
                return NotFound();
            }

            // Удаляем студента из класса
            studentProfile.ClassId = null;
            _context.Update(studentProfile);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = classId });
        }

        private bool ClassExists(int id)
        {
            return _context.Classes.Any(e => e.Id == id);
        }
    }
}
