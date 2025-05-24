using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineTutor.Data;
using OnlineTutor.Models;

namespace OnlineTutor.Controllers
{
    [Authorize]  // Только авторизованные пользователи могут видеть календарь
    public class CalendarController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CalendarController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Отображение страницы календаря
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Возвращает JSON-список событий для FullCalendar
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetEvents(DateTime? start, DateTime? end)
        {
            // На вход FullCalendar передаёт start и end для оптимизации загрузки
            var query = _context.CalendarEvents.AsQueryable();
            if (start.HasValue) query = query.Where(e => e.End == null ? e.Start >= start.Value : e.End >= start.Value);
            if (end.HasValue) query = query.Where(e => e.Start <= end.Value);

            var events = await query
                .Select(e => new
                {
                    id = e.Id,
                    title = e.Title,
                    start = e.Start.ToString("s"),
                    end = e.End.HasValue ? e.End.Value.ToString("s") : null,
                    allDay = e.AllDay,
                    backgroundColor = e.BackgroundColor,
                    borderColor = e.BorderColor
                })
                .ToListAsync();

            return Json(events);
        }

        /// <summary>
        /// Пример: создаём новое событие через AJAX
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] CalendarEvent evt)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.CalendarEvents.Add(evt);
            await _context.SaveChangesAsync();
            return Ok(new { evt.Id });
        }

        // Аналогично можно добавить EditEvent и DeleteEvent по AJAX
    }
}
