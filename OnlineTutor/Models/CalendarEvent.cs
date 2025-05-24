using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineTutor.Models
{
    /// <summary>
    /// Событие в календаре
    /// </summary>
    public class CalendarEvent
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public DateTime Start { get; set; }

        public DateTime? End { get; set; }

        /// <summary>
        /// Цвет фона (css)
        /// </summary>
        [StringLength(20)]
        public string BackgroundColor { get; set; }

        /// <summary>
        /// Цвет рамки/текста
        /// </summary>
        [StringLength(20)]
        public string BorderColor { get; set; }

        /// <summary>
        /// Может ли событие растягиваться
        /// </summary>
        public bool AllDay { get; set; }
    }
}
