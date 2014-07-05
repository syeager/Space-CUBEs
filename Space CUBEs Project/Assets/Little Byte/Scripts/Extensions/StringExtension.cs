// Steve Yeager
// 5.11.2014

using System.Collections.Generic;
using System.Text;

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
}