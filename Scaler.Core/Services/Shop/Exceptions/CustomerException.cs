namespace Scaler.Core.Services.Shop
{
    /// <summary>
    /// Represents errors that occur with customer operations.
    /// </summary>
    public class CustomerException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerException" /> class.
        /// </summary>
        public CustomerException() : base("Customer Exception occured.")
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public CustomerException(string? message) : base(message)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public CustomerException(string? message, Exception? innerException) : base(message, innerException)
        {

        }
    }
}
