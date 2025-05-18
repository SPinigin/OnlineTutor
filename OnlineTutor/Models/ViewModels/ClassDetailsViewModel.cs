namespace OnlineTutor.Models.ViewModels
{
    public class ClassDetailsViewModel
    {
        public Class Class { get; set; }
        public List<User> AvailableStudents { get; set; } = new List<User>();
    }
}
