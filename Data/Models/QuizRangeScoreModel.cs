using Data.Enum;

namespace Data.Models;

public class QuizRangeScoreCreateModel
{
    public int MinScore { get; set; } = 0;
    public int MaxScore { get; set; } = 0;
    public RangeScoreResult Result { get; set; }
}


