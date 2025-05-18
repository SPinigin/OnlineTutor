namespace OnlineTutor.Models
{
    public class StudentImportDto
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Grade { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
