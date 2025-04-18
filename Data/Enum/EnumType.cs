namespace Data.Enum
{
    public class EnumType
    {
    }

    public enum UserRole
    {
        Member = 0,
        Administrator = 1
    }

    public enum Gender
    {
        Male = 0,
        Female = 1,
        Other = 2
    }

    public enum ApplicationStatus
	{
		Pending = 0,
		Approved = 1,
		Rejected = 2
	}

    public enum QuestionType
    {
        Essay = 0,
        MultipleChoice = 1
    }

    public enum RangeScoreResult
    {
        Excellent = 0,
        Good = 1,
        Average = 2,
        Poor = 3,
        Bad = 4,
        None = 5
    }

    public enum QuizResultStatus
    {
        Pending = 0,
        Completed = 1
    }

    public enum PostStatus
	{
		Pending = 0,
		Approved = 1,
		Rejected = 2,
        EditedPendingApproval = 3
    }

    public enum MediaType
    {
        Image = 0,
        Video = 1,
    }

    public enum QuizType
    {
        Interview = 0,
        Quiz = 1,
    }

    public enum ReactionType
    {
        Love = 0,
        Happy = 1,
        Sad = 2,
        Bad = 3,
        Like = 4
    }
}
