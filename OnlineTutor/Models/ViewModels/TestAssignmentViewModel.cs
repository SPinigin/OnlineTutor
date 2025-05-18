using System.ComponentModel.DataAnnotations;

namespace OnlineTutor.Models.ViewModels
{
    public class TestAssignmentViewModel
    {
        public int Id { get; set; }
        public int TestId { get; set; }

        [Display(Name = "Название теста")]
        public string TestTitle { get; set; }

        [Display(Name = "Срок выполнения")]
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }

        [Display(Name = "Назначить классу")]
        public int? ClassId { get; set; }

        [Display(Name = "Назначить ученику")]
        public int StudentId { get; set; }

        public List<Class> AvailableClasses { get; set; } = new List<Class>();
        public List<User> AvailableStudents { get; set; } = new List<User>();
    }
}
