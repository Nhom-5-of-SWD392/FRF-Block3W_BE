namespace Data.Models;

public class QuizAnswerModel
{
    public string? AnswerText { get; set; }
    public int Score { get; set; } = 0;
    public Guid QuizQuestionId { get; set; }
    public Guid QuizId { get; set; }
}
