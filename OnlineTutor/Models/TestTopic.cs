using OnlineTutor.Models;
using System.ComponentModel.DataAnnotations;

public class TestTopic
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Название темы")]
    public string Name { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Предмет")]
    public string Subject { get; set; }

    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();
}
