namespace Scaler.Core.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T[]? NullIfEmpty<T>(this T[]? value) => value?.Length == 0 ? null : value;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T[]? EmptyIfNull<T>(this T[]? value) => value ?? [];
    }
}
