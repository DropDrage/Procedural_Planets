using JetBrains.Annotations;

namespace Utils.Extensions
{
    public static class StringExtensions
    {
        [Pure]
        public static bool IsEmpty(this string @string) => @string.Length == 0;

        [Pure]
        public static bool IsNotEmpty(this string @string) => @string.IsEmpty();


        [Pure]
        public static bool IsNullOrEmpty([CanBeNull] this string @string) =>
            @string == null || @string.IsEmpty();

        [Pure]
        public static bool IsNotNullOrEmpty([CanBeNull] this string @string) =>
            !@string.IsNullOrEmpty();
    }
}
