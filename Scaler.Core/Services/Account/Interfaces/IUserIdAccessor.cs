namespace Scaler.Core.Services.Account
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUserIdAccessor
    {
        /// <summary>
        /// GetCurrentUserId
        /// </summary>
        /// <returns></returns>
        string? GetCurrentUserId();
    }
}
