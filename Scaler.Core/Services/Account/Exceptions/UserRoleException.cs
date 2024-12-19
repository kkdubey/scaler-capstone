namespace Scaler.Core.Services.Account
{
    /// <summary>
    /// Represents errors that occured with user role related operations.
    /// </summary>
    public class UserRoleException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public UserRoleException() : base("A User Role Exception has occured.")
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public UserRoleException(string? message) : base(message)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public UserRoleException(string? message, Exception? innerException) : base(message, innerException)
        {

        }
    }
}
