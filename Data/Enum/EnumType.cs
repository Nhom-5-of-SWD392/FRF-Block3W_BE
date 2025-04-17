namespace Data.Enum
{
    public class EnumType
    {
    }

    public enum UserRole
    {
        Member,
        Administrator
    }

    public enum Gender
    {
        Male,
        Female,
        Other
    }

    public enum ApplicationStatus
	{
		Pending,
		Approved,
		Rejected
	}

    public enum QuestionType
    {
        Essay,
        MultipleChoice
    }

    public enum RangeScoreResult
    {
        Excellent,
        Good,
        Average,
        Poor,
        Bad,
        None
    }

    public enum QuizResultStatus
    {
        Pending,
        Completed
    }

    public enum PostStatus
	{
		Pending,
		Approved,
		Rejected,
        EditedPendingApproval
    }

    public enum MediaType
    {
        Image,
        Video,
    }

    public enum QuizType
    {
        Interview,
        Quiz,
    }

    public enum ReactionType
    {
        Love,
        Happy,
        Sad,
        Bad,
        Like
    }
}
