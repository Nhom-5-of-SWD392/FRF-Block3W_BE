using Data.Entities;
using Data.Enum;
using System.Numerics;

namespace Data.Models;

public class QuizQueryModel : QueryStringParameters
{
    public QuizQueryModel()
    {
        OrderBy = "Name";
    }
    public string? Search { get; set; }
}

public class QuizViewModel : BaseModel
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public QuizType Type { get; set; }
    public int TotalQuestion { get; set; }
}

public class QuizCreateModel
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public QuizType Type { get; set; }
}

public class CreateQuizRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public QuizType Type { get; set; }
    public List<QuizRangeScoreAddToQuiz> QuizRangeScore { get; set; } = new();
    public List<CreateQuizQuestionRequest> Questions { get; set; } = new();
}

public class CreateQuizQuestionRequest
{
    public string QuestionText { get; set; } = string.Empty;
    public QuestionType Type { get; set; }
    public List<CreateQuizAnswerRequest>? Answers { get; set; }
}

public class CreateQuizAnswerRequest
{
    public string AnswerText { get; set; } = string.Empty;
    public int Score { get; set; }
}

public class QuizDetailResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public QuizType Type { get; set; }
    public List<QuizRangeScoreResponse> QuizRangeScore { get; set; } = new();
    public List<QuizQuestionResponse> Questions { get; set; } = new();

}

public class QuizRangeScoreResponse
{
    public Guid Id { get; set; }    
    public int MinScore { get; set; } = 0;
    public int MaxScore { get; set; } = 0;
    public string Result { get; set; } = string.Empty;
}

public class QuizQuestionResponse
{
    public Guid Id { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public QuestionType Type { get; set; }
    public List<QuizAnswerResponse>? Answers { get; set; }
}

public class QuizAnswerResponse
{
    public Guid Id { get; set; }
    public string? AnswerText { get; set; }
    public int Score { get; set; }
}

public class SubmitQuizRequest
{
    public Guid QuizId { get; set; }

    public List<SubmitAnswerItem> Answers { get; set; } = new();
}

public class SubmitAnswerItem
{
    public Guid QuestionId { get; set; }
    public Guid? AnswerId { get; set; } 
    public string? EssayAnswer { get; set; }
}

public class EvaluateQuizRequest
{
    public Guid QuizResultId { get; set; }

    public List<QuizResultDetails> Answers { get; set; } = new();
}

public class QuizResultDetails
{
    public Guid? AnswerId { get; set; }
    public double EvaluationScore { get; set; }
}

public class QuizRangeScoreAddToQuiz
{
    public int MinScore { get; set; } = 0;
    public int MaxScore { get; set; } = 0;
    public string Result { get; set; } = string.Empty;
}

