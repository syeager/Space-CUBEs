// Little Byte Games
// Author: Steve Yeager
// Created: 2014.05.11
// Edited: 2014.08.24

using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// Extension methods for strings.
/// </summary>
public static class StringExtension
{
    /// <summary>
    /// Combine all entries into one string.
    /// </summary>
    /// <param name="strings">List of strings to join.</param>
    /// <param name="separator">Value to separate each entry.</param>
    /// <returns>Combined string.</returns>
    public static string Join(this IEnumerable<string> strings, string separator)
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (string s in strings)
        {
            stringBuilder.Append(s);
            stringBuilder.Append(separator);
        }

        return stringBuilder.ToString();
    }


    /// <summary>
    /// Combine all entries into one string.
    /// </summary>
    /// <param name="strings">List of strings to join.</param>
    /// <param name="separator">Value to separate each entry.</param>
    /// <returns>Combined string.</returns>
    public static string Join(this IEnumerable<string> strings, char[] separator)
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (string s in strings)
        {
            stringBuilder.Append(s);
            stringBuilder.Append(separator);
        }

        return stringBuilder.ToString();
    }


    /// <summary>
    /// Combine all entries into one string.
    /// </summary>
    /// <param name="strings">List of strings to join.</param>
    /// <param name="separator">Value to separate each entry.</param>
    /// <returns>Combined string.</returns>
    public static string Join(this IEnumerable<string> strings, char separator)
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (string s in strings)
        {
            stringBuilder.Append(s);
            stringBuilder.Append(separator);
        }

        return stringBuilder.ToString();
    }


    /// <summary>
    /// Add spaces to a camel cased string.
    /// </summary>
    /// <param name="str">String instance.</param>
    /// <returns>String with spaces injected.</returns>
    public static string SplitCamelCase(this string str)
    {
        const string pattern1 = @"(\P{Ll})(\P{Ll}\p{Ll})";
        const string pattern2 = @"(\p{Ll})(\P{Ll})";
        const string replacement = "$1 $2";

        return Regex.Replace(Regex.Replace(str, pattern1, replacement), pattern2, replacement);
    }
}