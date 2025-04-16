using Data.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models;

public class QuizAnswerCreateModel
{
    public string? AnswerText { get; set; }
    public int Score { get; set; } = 0;
    public Guid QuizQuestionId { get; set; }
    public Guid QuizId { get; set; }
}
