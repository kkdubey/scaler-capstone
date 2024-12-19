using Scaler.Core.Authorization.Requirements;

namespace Scaler.Core.Authorization
{
    /// <summary>
    /// Operation Policy to allow adding, viewing, updating and deleting records.
    /// </summary>
    public static class UserAccountManagementOperations
    {
        /// <summary>
        /// 
        /// </summary>
        public const string CreateOperationName = "Create";
        /// <summary>
        /// 
        /// </summary>
        public const string ReadOperationName = "Read";
        /// <summary>
        /// 
        /// </summary>
        public const string UpdateOperationName = "Update";
        /// <summary>
        /// 
        /// </summary>
        public const string DeleteOperationName = "Delete";

        /// <summary>
        /// 
        /// </summary>
        public static readonly UserAccountAuthorizationRequirement CreateOperationRequirement = new(CreateOperationName);
        /// <summary>
        /// 
        /// </summary>
        public static readonly UserAccountAuthorizationRequirement ReadOperationRequirement = new(ReadOperationName);
        /// <summary>
        /// 
        /// </summary>
        public static readonly UserAccountAuthorizationRequirement UpdateOperationRequirement = new(UpdateOperationName);
        /// <summary>
        /// 
        /// </summary>
        public static readonly UserAccountAuthorizationRequirement DeleteOperationRequirement = new(DeleteOperationName);
    }
}
