

namespace Service.Utilities
{
    public static class ErrorMessage
    {
        //Role
        public static string RoleExist = "Role already exists.";

        //Permission
        public static string PermissionExist = "Permission already exists.";

        //User
        public static string UserExist = "Username or email already exists.";
        public static string InvalidAccount = "Invalid email or password.";

        //Others
        public static string IdNotExist = "ID does not exist.";
        public static string AccessTokenFail = "Access token failed!";
    }

}
