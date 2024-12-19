namespace Scaler.Core.Services.Account
{
    /// <summary>
    /// Represents errors that occured with user account related operations.
    /// </summary>
    public class UserAccountException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public UserAccountException() : base("A User Account Exception has occured.")
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public UserAccountException(string? message) : base(message)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public UserAccountException(string? message, Exception? innerException) : base(message, innerException)
        {

        }
    }
}
