using Data.Enum;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class Quiz : BaseEntities
{
    [Required(ErrorMessage = "Quiz name is required")]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    [Required(ErrorMessage = "Quiz type is required")]
    public QuizType Type { get; set; }

    public IList<QuizRangeScore>? QuizRangeScores { get; set; }
    public IList<QuizAnswer>? QuizAnswers { get; set; }
    public IList<QuizQuestion>? QuizQuestions { get; set; }
    public IList<QuizDetail>? QuizDetails { get; set; }
    public IList<QuizResult>? QuizResults { get; set; }
}
