

namespace Service.Utilities;

//public static class ErrorMessage
//{
//    //User
//    public static string UserExist = "Username or email already exists.";
//    public static string UserNotFound = "User not found.";
//    public static string InvalidAccount = "Sai tài khoản hoặc mật khẩu. Vui lòng kiểm tra lại."; //Invalid username or password.
//    public static string AlreadyApplyModerator = "You have already applied to be a moderator.";
//    public static string RequestNotFound = "Requester not found.";
//    public static string RequestAlreadyProcessed = "Request already processed.";

//    //Quiz
//    public static string QuizExist = "Quiz existed!";
//    public static string QuizNotExist = "Quiz not found!";
//    public static string QuizTypeOnlyMulChoice = "Only multiple choice questions are allowed in a quiz-type quiz.";
//    public static string MulChoiceMustHaveAnswer = "Multiple choice questions must have at least one answer.";
//    public static string QuizResultNotFound = "Quiz result not found";
//    public static string QuizResultNotFoundOrEvaluated = ("Result not found or already evaluated");
//    public static string OnlyEssayCanGrade = "Only essay questions are graded.";

//    //Question
//    public static string QuestionNotFound = "Question not found.";

//    //QuizRangeScore
//    public static string MinCantGreaterMax = "Min Score cannot be greater than Max Score.";

//    //Post
//    public static string PostNotFound = "Post not found.";
//    public static string PostNotMatchWithUser = "Post does not match with user.";

//	//Media
//	public static string MediaNotFound = "Media not found.";

//    //Topic
//    public static string TopicNotFound = "Topic not found.";
//    public static string TopicNotExist = "One or more topics do not exist in the system.";

//	//Favorite
//	public static string FavoriteNotFound = "Favorite not found.";

//    //Comment
//    public static string OnlyMemberCanComment = "Only registered members can comment.";
//    public static string ParentCommentNotFound = "Parent comment not found.";
//    public static string CommentNotFound = "Comment not found.";
//    public static string OnlyUpdateOwnComment = "You can only edit your own comment.";
//    public static string OnlyDeleteOwnComment = "You can only delete your own comment.";

//    //Others
//    public static string IdNotExist = "ID does not exist.";
//    public static string AccessTokenFail = "Access token failed!";
//    public static string Unauthorize = "User is not authorized.";
//    public static string NoVideo = "No Video uploaded";
//    public static string UnsupportedFile = "Unsupported file type. Only images and videos are allowed.";
//    public static string OnlyAllowImage = "Only image allowed.";
//	public static string OnlyAdminAndModerator = "Only Moderator or Admin can perform this operation";
//}

public static class ErrorMessage
{
    //User
    public static string UserExist = "Tên người dùng hoặc email đã tồn tại.";
    public static string UserNotFound = "Không tìm thấy người dùng.";
    public static string InvalidAccount = "Sai tài khoản hoặc mật khẩu. Vui lòng kiểm tra lại.";
    public static string AlreadyApplyModerator = "Bạn đã gửi yêu cầu trở thành người kiểm duyệt.";
    public static string RequestNotFound = "Không tìm thấy người gửi yêu cầu.";
    public static string RequestAlreadyProcessed = "Yêu cầu đã được xử lý.";

    //Quiz
    public static string QuizExist = "Bài kiểm tra đã tồn tại!";
    public static string QuizNotExist = "Không tìm thấy bài kiểm tra!";
    public static string QuizTypeOnlyMulChoice = "Chỉ được phép sử dụng câu hỏi trắc nghiệm trong bài kiểm tra loại Quiz.";
    public static string MulChoiceMustHaveAnswer = "Câu hỏi trắc nghiệm phải có ít nhất một câu trả lời.";
    public static string QuizResultNotFound = "Không tìm thấy kết quả bài kiểm tra.";
    public static string QuizResultNotFoundOrEvaluated = "Không tìm thấy kết quả hoặc kết quả đã được chấm.";
    public static string OnlyEssayCanGrade = "Chỉ câu hỏi tự luận mới được chấm điểm.";

    //Question
    public static string QuestionNotFound = "Không tìm thấy câu hỏi.";

    //QuizRangeScore
    public static string MinCantGreaterMax = "Điểm tối thiểu không thể lớn hơn điểm tối đa.";
    public static string NotOverlap = "Điểm của các bài kiểm tra không được trùng nhau.";
    public static string QuizRangeScoreNotFound = "Phạm vi điểm không tìm thấy.";

    //Post
    public static string PostNotFound = "Không tìm thấy bài viết.";
    public static string PostNotMatchWithUser = "Bài viết không khớp với người dùng.";

    //Media
    public static string MediaNotFound = "Không tìm thấy Media.";

    //Topic
    public static string TopicNotFound = "Không tìm thấy chủ đề.";
    public static string TopicNotExist = "Một hoặc nhiều chủ đề không tồn tại trong hệ thống.";

    //Favorite
    public static string FavoriteNotFound = "Không tìm thấy mục yêu thích.";

    //Ingredient
    public static string IngredientNotFound = "Không tìm thấy nguyên liệu.";

    //Instruction
    public static string InstructionNotFound = "Không tìm thấy hướng dẫn.";

    //Comment
    public static string OnlyMemberCanComment = "Chỉ thành viên đã đăng ký mới có thể bình luận.";
    public static string ParentCommentNotFound = "Không tìm thấy bình luận cha.";
    public static string CommentNotFound = "Không tìm thấy bình luận.";
    public static string OnlyUpdateOwnComment = "Bạn chỉ có thể sửa bình luận của chính mình.";
    public static string OnlyDeleteOwnComment = "Bạn chỉ có thể xóa bình luận của chính mình.";

    //Mail
    public static string RecipientNotExist = "Người nhận mail không được trống.";
    public static string EmailNotExist = "Email không tồn tại.";

    //Others
    public static string IdNotExist = "ID không tồn tại.";
    public static string AccessTokenFail = "Lỗi xác thực Access Token!";
    public static string Unauthorize = "Người dùng không có quyền truy cập.";
    public static string NoVideo = "Chưa tải lên video.";
    public static string UnsupportedFile = "Định dạng tệp không được hỗ trợ. Chỉ chấp nhận hình ảnh và video.";
    public static string OnlyAllowImage = "Chỉ chấp nhận hình ảnh.";
    public static string OnlyAdminAndModerator = "Chỉ Quản trị viên hoặc Người kiểm duyệt mới được phép thao tác này.";
    public static string ValidatePassword = "Password does not meet the required complexity standards:\n" +
                                            "- At least 8 characters long\n" +
                                            "- Include UPPERCASE and lowercase letters\n" +
                                            "- At least one digit\n" +
                                            "- At least one special character @#$%^&*!_";
}

