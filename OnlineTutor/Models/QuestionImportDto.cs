using OnlineTutor.Models;

public class QuestionImportDto
{
    public string Text { get; set; }
    public QuestionType Type { get; set; }
    public int Points { get; set; }
    public List<QuestionOptionImportDto> Options { get; set; }
}
