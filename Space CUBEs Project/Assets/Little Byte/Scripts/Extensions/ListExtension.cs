// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.04.28
// Edited: 2014.06.20

using System.Collections;
using System.Collections.Generic;

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
}