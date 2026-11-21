namespace CampusEventManagement
{
    static class Session
    {
        public static int UserId;
        public static string FullName;
        public static string Username;
        public static string Role;
        public static bool IsLoggedIn { get { return UserId > 0; } }
        public static void Clear() { UserId = 0; FullName = Username = Role = null; }
    }
}