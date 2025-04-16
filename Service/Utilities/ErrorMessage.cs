

namespace Service.Utilities;

public static class ErrorMessage
{
    //User
    public static string UserExist = "Username or email already exists.";
    public static string InvalidAccount = "Invalid email or password.";

    //Quiz
    public static string QuizExist = "Quiz existed!";
    public static string QuizNotExist = "Quiz not found!";
    public static string QuizTypeOnlyMulChoice = "Only multiple choice questions are allowed in a quiz-type quiz.";
    public static string MulChoiceMustHaveAnswer = "Multiple choice questions must have at least one answer.";
    

    //Others
    public static string IdNotExist = "ID does not exist.";
    public static string AccessTokenFail = "Access token failed!";
    public static string Unauthorize = "User is not authorized.";
}
