using Data.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class QuizRangeScore : BaseEntities
{
    public int MinScore { get; set; } = 0;
    public int MaxScore { get; set; } = 0;
    public RangeScoreResult Result { get; set; }

    //Foreign Keys
    public Guid QuizId { get; set; }
    [ForeignKey("QuizId")]
    public Quiz? Quiz { get; set; }
}
