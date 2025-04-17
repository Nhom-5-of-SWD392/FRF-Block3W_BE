

namespace Service.Utilities;

public static class ErrorMessage
{
    //User
    public static string UserExist = "Username or email already exists.";
    public static string UserNotFound = "User not found.";
    public static string InvalidAccount = "Invalid username or password.";
    public static string AlreadyApplyModerator = "You have already applied to be a moderator.";
    public static string RequestNotFound = "Requester not found.";
    public static string RequestAlreadyProcessed = "Request already processed.";

    //Quiz
    public static string QuizExist = "Quiz existed!";
    public static string QuizNotExist = "Quiz not found!";
    public static string QuizTypeOnlyMulChoice = "Only multiple choice questions are allowed in a quiz-type quiz.";
    public static string MulChoiceMustHaveAnswer = "Multiple choice questions must have at least one answer.";
    public static string QuizResultNotFound = "Quiz result not found";
    public static string QuizResultNotFoundOrEvaluated = ("Result not found or already evaluated");
    public static string OnlyEssayCanGrade = "Only essay questions are graded.";

    //Question
    public static string QuestionNotFound = "Question not found.";

    //QuizRangeScore
    public static string MinCantGreaterMax = "Min Score cannot be greater than Max Score.";


    //Others
    public static string IdNotExist = "ID does not exist.";
    public static string AccessTokenFail = "Access token failed!";
    public static string Unauthorize = "User is not authorized.";
    public static string NoVideo = "No Video uploaded";
}
