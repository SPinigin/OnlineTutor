using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using OnlineTutor.Data;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace OnlineTutor.Models
{
    public class Test
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название теста обязательно")]
        [Display(Name = "Название теста")]
        public string Title { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Инструкции")]
        public string Instructions { get; set; }

        [Required(ErrorMessage = "Предмет обязателен")]
        [Display(Name = "Предмет")]
        public string Subject { get; set; }

        [Display(Name = "Тема")]
        public string Topic { get; set; }

        [Required]
        [Display(Name = "Дата создания")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Время на выполнение (минуты)")]
        public int? TimeLimit { get; set; }

        [Display(Name = "Проходной балл (%)")]
        [Range(0, 100, ErrorMessage = "Проходной балл должен быть от 0 до 100")]
        public int? PassingScore { get; set; }

        [Display(Name = "Опубликован")]
        public bool IsPublished { get; set; }

        // Внешний ключ
        [Required]
        public int TeacherId { get; set; }  // Изменено с string на int

        // Навигационные свойства
        public virtual User Teacher { get; set; }
        public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
        public virtual ICollection<TestAssignment> TestAssignments { get; set; } = new List<TestAssignment>();

        public int? TopicId { get; set; }
        public virtual TestTopic? Topic2 { get; set; }

        public int? GroupId { get; set; }
        public virtual TestGroup? Group { get; set; }

        //public void AutoGroup()
        //{
        //    // Логика группировки
        //    if (Questions.Average(q => q.Points) <= 2)
        //    {
        //        GroupId = GetGroupIdByNameAndType("Низкая сложность", TestGroupType.Difficulty);
        //    }
        //    else if (Questions.Average(q => q.Points) <= 4)
        //    {
        //        GroupId = GetGroupIdByNameAndType("Средняя сложность", TestGroupType.Difficulty);
        //    }
        //    else
        //    {
        //        GroupId = GetGroupIdByNameAndType("Высокая сложность", TestGroupType.Difficulty);
        //    }

        //    // Группировка по типу ответов
        //    var answerTypes = Questions.Select(q => q.Type).Distinct().ToList();
        //    if (answerTypes.Count == 1)
        //    {
        //        GroupId = GetGroupIdByNameAndType(
        //            answerTypes[0].ToString(),
        //            TestGroupType.AnswerType
        //        );
        //    }
        //}

        //public interface ITestGroupService
        //{
        //    int? GetGroupIdByNameAndType(string name, TestGroupType type);
        //}

    }
}
