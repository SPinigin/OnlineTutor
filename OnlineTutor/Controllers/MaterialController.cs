using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineTutor.Data;
using OnlineTutor.Models;
using OnlineTutor.Models.ViewModels;

namespace OnlineTutor.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class MaterialController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _env;

        public MaterialController(ApplicationDbContext context,
                                  UserManager<User> userManager,
                                  IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        // GET: /Material
        public async Task<IActionResult> Index(int? topicId)
        {
            var currentTeacher = await _userManager.GetUserAsync(User);
            var topics = await _context.MaterialTopics
                .Include(t => t.Materials.Where(m => m.TeacherId == currentTeacher.Id))
               .ToListAsync();

            var materials = topics
                .Where(t => !topicId.HasValue || t.Id == topicId)
                .SelectMany(t => t.Materials)
                .OrderByDescending(m => m.UploadedAt)
                .ToList();

            ViewBag.Topics = topics;
            ViewBag.SelectedTopic = topicId;

            return View(materials);
        }

        // GET: /Material/CreateTopic
        public IActionResult CreateTopic() => View();

        // POST: /Material/CreateTopic
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTopic(MaterialTopic model)
        {
            if (ModelState.IsValid)
            {
                _context.MaterialTopics.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: /Material/Create
        public async Task<IActionResult> Create()
        {
            var tm = new MaterialViewModel
            {
                Topics = await _context.MaterialTopics.ToListAsync(),
                Classes = await _context.Classes.ToListAsync(),
                Students = (await _userManager.GetUsersInRoleAsync("Student")).ToList()
            };
            return View(tm);
        }

        // POST: /Material/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MaterialViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var teacher = await _userManager.GetUserAsync(User);

                string filePath = null;
                if (vm.File != null && vm.File.Length > 0)
                {
                    var uploads = Path.Combine(_env.WebRootPath, "uploads", "materials");
                    Directory.CreateDirectory(uploads);
                    var fileName = $"{Guid.NewGuid()}_{vm.File.FileName}";
                    filePath = Path.Combine("uploads/materials", fileName);
                    using var stream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create);
                    await vm.File.CopyToAsync(stream);
                }

                var material = new Material
                {
                    Title = vm.Title,
                    Description = vm.Description,
                    Type = vm.Type,
                    FileUrl = filePath ?? vm.FileUrl,
                    TopicId = vm.TopicId,
                    ClassId = vm.ClassId,
                    StudentId = vm.StudentId,
                    TeacherId = teacher.Id
                };

                _context.Materials.Add(material);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            vm.Topics = await _context.MaterialTopics.ToListAsync();
            vm.Classes = await _context.Classes.ToListAsync();
            vm.Students = (await _userManager.GetUsersInRoleAsync("Student")).ToList();
            return View(vm);
        }

        // GET: /Material/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var material = await _context.Materials
                .Include(m => m.Topic)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (material == null) return NotFound();
            return View(material);
        }

        // POST: /Material/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var material = await _context.Materials.FindAsync(id);
            if (material != null)
            {
                _context.Materials.Remove(material);
                await _context.SaveChangesAsync();
                // Можно удалить файл с диска при желании
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
