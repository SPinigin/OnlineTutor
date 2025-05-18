using System.ComponentModel.DataAnnotations;

namespace OnlineTutor.Models.ViewModels
{
    public class StudentListViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }

        [Display(Name = "Телефон")]
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Grade { get; set; }
        public bool IsVerified { get; set; }
    }
}
