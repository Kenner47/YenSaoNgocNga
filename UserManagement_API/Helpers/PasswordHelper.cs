using Microsoft.AspNetCore.Identity;

namespace UserManagement_API.Helpers
{
    public class PasswordHelper
    {
        private readonly IPasswordHasher<object> _passwordHasher;

        public PasswordHelper()
        {
            _passwordHasher = new PasswordHasher<object>();
        }

        /// <summary>
        /// Hash password using ASP.NET Core Identity with salt
        /// </summary>
        /// <param name="password">Plain text password</param>
        /// <returns>Hashed password with salt</returns>
        public string HashPassword(string password)
        {
            return _passwordHasher.HashPassword(null, password);
        }

        /// <summary>
        /// Verify password against hashed password with salt
        /// </summary>
        /// <param name="password">Plain text password</param>
        /// <param name="hashedPassword">Hashed password with salt to compare</param>
        /// <returns>True if password matches</returns>
        public bool VerifyPassword(string password, string hashedPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(null, hashedPassword, password);
            return result == PasswordVerificationResult.Success ||
                   result == PasswordVerificationResult.SuccessRehashNeeded;
        }
    }

    /// <summary>
    /// Static version for easier usage without dependency injection
    /// </summary>
    public static class PasswordHelperStatic
    {
        private static readonly IPasswordHasher<object> _passwordHasher = new PasswordHasher<object>();

        /// <summary>
        /// Hash password using ASP.NET Core Identity with salt
        /// </summary>
        /// <param name="password">Plain text password</param>
        /// <returns>Hashed password with salt</returns>
        public static string HashPassword(string password)
        {
            return _passwordHasher.HashPassword(null, password);
        }

        /// <summary>
        /// Verify password against hashed password with salt
        /// </summary>
        /// <param name="password">Plain text password</param>
        /// <param name="hashedPassword">Hashed password with salt to compare</param>
        /// <returns>True if password matches</returns>
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(null, hashedPassword, password);
            return result == PasswordVerificationResult.Success ||
                   result == PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}