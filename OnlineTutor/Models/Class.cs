namespace OnlineTutor.Models
{
    public class Class
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public string TeacherId { get; set; }
        public virtual User Teacher { get; set; }
        public virtual ICollection<User> Students { get; set; }
        public virtual ICollection<Assignment> Assignments { get; set; }
    }
}
