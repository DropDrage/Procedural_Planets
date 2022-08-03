using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Utils.Extensions
{
    public static class CollectionExtensions
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty<T>([NotNull] this IEnumerable<T> enumerable) => enumerable.Count() == 0;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotEmpty<T>([NotNull] this IEnumerable<T> enumerable) => !enumerable.IsEmpty();


        [Pure]
        public static bool IsNullOrEmpty<T>([CanBeNull] this IEnumerable<T> enumerable) =>
            enumerable == null || enumerable.IsEmpty();

        [Pure]
        public static bool IsNotNullOrEmpty<T>([CanBeNull] this IEnumerable<T> enumerable) =>
            !enumerable.IsNullOrEmpty();


        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LastIndex(this ICollection list) => list.Count - 1;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LastIndex<T>(this IEnumerable<T> list) => list.Count() - 1;
    }
}
