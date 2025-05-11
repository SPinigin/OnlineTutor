namespace OnlineTutor.Models
{
    public class Assignment
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime DueDate { get; set; }
        public AssignmentType Type { get; set; }
        public int ClassId { get; set; }
        public virtual Class Class { get; set; }
        public virtual ICollection<StudentAssignment> StudentAssignments { get; set; }
        public virtual ICollection<AssignmentMaterial> Materials { get; set; }
    }

    public enum AssignmentType
    {
        Test,
        Essay,
        ReadingMaterial,
        CreativeTask
    }
}
