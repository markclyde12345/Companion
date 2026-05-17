namespace MentalWellness.Shared.Helpers
{
    public static class Validator
    {
        public static bool IsValidEmail(string email)
        {
            return !string.IsNullOrWhiteSpace(email) && email.Contains("@");
        }

        public static bool IsStrongPassword(string password)
        {
            return password.Length >= 6;
        }
    }
}