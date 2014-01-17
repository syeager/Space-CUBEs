// Steve Yeager
// 1.11.2014

using UnityEngine;

/// <summary>
/// Holds static methods for general jobs.
/// </summary>
public static class Utility
{
    #region Public Methods

    /// <summary>
    /// Converts string to Vector3.
    /// </summary>
    /// <param name="vectorString">String to convert. Must be in format (#, #, #)</param>
    /// <returns>Vector3 representation of the string.</returns>
    public static Vector3 ParseV3(string vectorString)
    {
        Debug.Log(vectorString);
        Vector3 vector;
        vectorString = vectorString.Substring(1, vectorString.Length-2).Replace(" ", "");
        string[] split = vectorString.Split(',');
        vector.x = float.Parse(split[0]);
        vector.y = float.Parse(split[1]);
        vector.z = float.Parse(split[2]);

        return vector;
    }

    #endregion
}