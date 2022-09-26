using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Random = UnityEngine.Random;

namespace Utils.Extensions
{
    public static class CollectionExtensions
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // ReSharper disable once UseMethodAny.2
        public static bool IsEmpty<T>([NotNull] this IEnumerable<T> enumerable) => enumerable.Count() == 0;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotEmpty<T>([NotNull] this IEnumerable<T> enumerable) => !enumerable.IsEmpty();


        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty<T>([CanBeNull] this IEnumerable<T> enumerable) =>
            enumerable == null || enumerable.IsEmpty();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotNullOrEmpty<T>([CanBeNull] this IEnumerable<T> enumerable) =>
            !enumerable.IsNullOrEmpty();


        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LastIndex(this ICollection list) => list.Count - 1;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LastIndex<T>(this IEnumerable<T> list) => list.Count() - 1;


        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetRandomItem<T>(this IList<T> list) => list[Random.Range(0, list.Count)];


        public static T[] GetRow<T>(this T[,] array, int row)
        {
            var cols = array.GetUpperBound(1) + 1;
            var result = new T[cols];
            var size = Marshal.SizeOf<T>();

            Buffer.BlockCopy(array, row * cols * size, result, 0, cols * size);

            return result;
        }
    }
}
