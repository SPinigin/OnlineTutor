using System.ComponentModel.DataAnnotations;

namespace OnlineTutor.Models
{
    public class StudentAssignment
    {
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        public virtual User Student { get; set; }

        [Required]
        public int AssignmentId { get; set; }

        public virtual Assignment Assignment { get; set; }

        [Required]
        public AssignmentStatus Status { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public string Answer { get; set; }

        public int? Score { get; set; }

        public string TeacherComment { get; set; }

        public enum AssignmentStatus
        {
            Assigned,
            InProgress,
            Submitted,
            Graded
        }
    }
}
