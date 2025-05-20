using OnlineTutor.Models;
using System.ComponentModel.DataAnnotations;

public class TestGroup
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Название группы")]
    public string Name { get; set; }

    [Required]
    [Display(Name = "Тип группировки")]
    public TestGroupType Type { get; set; }

    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();
}