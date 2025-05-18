namespace OnlineTutor.Models.ViewModels
{
    public class StudentListViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Grade { get; set; }
        public string ClassName { get; set; }
        public bool IsVerified { get; set; }
    }
}
