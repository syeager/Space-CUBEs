// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.05.23
// Edited: 2014.05.27

using UnityEditor;

/// <summary>
/// Creates Job Manager singleton.
/// </summary>
public class JobManagerCreator : Creator<JobManager>
{
    #region Creator Overrides

    [MenuItem("GameObject/Singletons/Job Manager", false, 3)]
    public static void Create()
    {
        Create("_JobManager");
    }

    #endregion
}