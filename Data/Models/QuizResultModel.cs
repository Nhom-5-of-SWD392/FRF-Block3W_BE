using Data.Enum;

namespace Data.Models;

public class QuizResultQueryModel : QueryStringParameters
{
    public QuizResultQueryModel()
    {
    }
    public string? Search { get; set; }
}

public class QuizResultView : BaseModel
{
    public QuizzModel Quizz { get; set; } = new ();
    public RangeScoreResult Result { get; set; }
    public QuizResultStatus Status { get; set; }
    public double FinalScore { get; set; }
    public List<QuizResultDetail> Details { get; set; } = new ();
}

public class QuizResultDetail
{
    public QuestionModel Question { get; set; } = new ();
    public int MaxScoreForAnswer { get; set; }
    public string? SelectedAnswerText { get; set; }
    public string? EssayAnswerText { get; set; }
    public double EvaluationScore { get; set; }
}

public class QuestionModel
{
    public Guid Id { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public QuestionType QuestionType { get; set; }
}

public class QuizzModel
{
    public Guid QuizId { get; set; }
    public string QuizName { get; set; } = string.Empty;
    public QuizType Type { get; set; }
}

public class EvaluateEssayRequest
{
    public Guid QuizResultId { get; set; }
    public Dictionary<Guid, double> EssayScores { get; set; } = new();
}





