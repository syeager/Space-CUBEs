// Little Byte Games
// Author: Steve Yeager
// Created: 2014.08.01
// Edited: 2014.08.01

using System;
using System.Collections.Generic;

namespace LittleByte.Extensions
{
    /// <summary>
    /// Extension methods for IEnumerables.
    /// </summary>
    public static class IEnumerableExtension
    {
        /// <summary>
        /// Get all indeces of items that pass the predicate.
        /// </summary>
        /// <typeparam name="T">Generic type of collection.</typeparam>
        /// <param name="collection">Collection instance.</param>
        /// <param name="predicate">Predicate an item needs to pass.</param>
        /// <returns>List of indeces.</returns>
        public static IEnumerable<int> IndexesWhere<T>(this IEnumerable<T> collection, Predicate<T> predicate)
        {
            int index = 0;
            foreach (T element in collection)
            {
                if (predicate(element))
                {
                    yield return index;
                }
                index++;
            }
        }


        /// <summary>
        /// Get the index of the first item to pass the predicate.
        /// </summary>
        /// <typeparam name="T">Generic type of collection.</typeparam>
        /// <param name="collection">Collection instance.</param>
        /// <param name="predicate">Predicate an item needs to pass.</param>
        /// <returns>Index if an item is found or -1 if none pass.</returns>
        public static int IndexWhere<T>(this IEnumerable<T> collection, Predicate<T> predicate)
        {
            int index = 0;
            foreach (T element in collection)
            {
                if (predicate(element))
                {
                    return index;
                }
                index++;
            }

            return -1;
        }
    }
}