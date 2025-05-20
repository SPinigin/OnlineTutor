using System.ComponentModel.DataAnnotations;

public enum TestGroupType
{
    [Display(Name = "По сложности")]
    Difficulty,

    [Display(Name = "По типу ответа")]
    AnswerType,

    [Display(Name = "По теме")]
    Topic
}
