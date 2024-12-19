namespace Scaler.Core.Services.Account
{
    /// <summary>
    /// 
    /// </summary>
    public class UserNotFoundException : UserAccountException
    {
        /// <summary>
        /// 
        /// </summary>
        public UserNotFoundException() : base("Unable to find the requested User.")
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public UserNotFoundException(string? message) : base(message)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public UserNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {

        }
    }
}
