// Little Byte Games

using System.Collections;
using System.Collections.Generic;

namespace LittleByte.Extensions
{
    public static class ListExtension
    {
        /// <summary>
        /// Create a new list setting the same value to each entry.
        /// </summary>
        /// <param name="list">List instance.</param>
        /// <param name="value">Value to give each entry.</param>
        /// <param name="count">How many entries to give the list.</param>
        public static void Initialize<T>(this List<T> list, T value, int count)
        {
            list.Clear();
            for (int i = 0; i < count; i++)
            {
                list.Add(value);
            }
        }

        /// <summary>
        /// Give all entries the same value.
        /// </summary>
        /// <param name="list">List instance.</param>
        /// <param name="value">Value to give each entry.</param>
        public static void SetAll<T>(this IList list, T value)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = value;
            }
        }

        /// <summary>
        /// Randomly return an entry.
        /// </summary>
        /// <typeparam name="T">List generic.</typeparam>
        /// <param name="list">List instance.</param>
        /// <returns>An entry in the list.</returns>
        public static T Random<T>(this IList<T> list)
        {
            return list[UnityEngine.Random.Range(0, list.Count - 1)];
        }
    }
}