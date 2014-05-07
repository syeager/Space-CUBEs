// Steve Yeager
// 4.27.2014

using System.Collections.Generic;

public static class ListExtension
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="list"></param>
    /// <param name="value"></param>
    /// <param name="count"></param>
    public static void Initialize<T>(this List<T> list, T value, int count)
    {
        list.Clear();
        for (int i = 0; i < count; i++)
        {
            list.Add(value);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="list"></param>
    /// <param name="value"></param>
    public static void SetAll<T>(this List<T> list, T value)
    {
        for (int i = 0; i < list.Count; i++)
        {
            list[i] = value;
        }
    }
}