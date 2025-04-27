using Data.Enum;

namespace Data.Models;

public class QuizQuestionCreateModel
{
    public string QuestionText { get; set; } = string.Empty;
    public QuestionType Type { get; set; }
    public Guid QuizId { get; set; }
}
