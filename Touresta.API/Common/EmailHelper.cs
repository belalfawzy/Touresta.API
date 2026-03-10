namespace RAFIQ.API.Common
{
    public static class EmailHelper
    {
        /// <summary>
        /// Returns the base Gmail address by stripping the +alias portion.
        /// Only applies to gmail.com domains. Non-Gmail emails are returned unchanged.
        /// Example: belal+test@gmail.com → belal@gmail.com
        /// </summary>
        public static string GetBaseEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return email;

            var atIndex = email.IndexOf('@');
            if (atIndex < 0)
                return email;

            var localPart = email[..atIndex];
            var domain = email[(atIndex + 1)..];

            if (!domain.Equals("gmail.com", StringComparison.OrdinalIgnoreCase))
                return email;

            var plusIndex = localPart.IndexOf('+');
            if (plusIndex < 0)
                return email;

            return localPart[..plusIndex] + "@" + domain;
        }
    }
}
