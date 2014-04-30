// Steve Yeager
// 4.27.2014

using System;
using System.IO;
using System.Linq;
using Annotations;
using UnityEditor;
using UnityEngine;

public class BuildTool : EditorWindow
{
    #region Private Fields

    private int major;
    private int minor;
    private int patch;

    private bool targetPC = true;

    private bool first = true;

    #endregion

    #region Const Fields

    private const string BuildPath = "D:/Documents/GitHub/Space-CUBEs/Builds/PreAlpha/";
    private const string NewestBuildPath = "D:/Documents/Google Drive/Space CUBEs/Builds/";

    #endregion


    #region EditorWindow Overrides

    [UsedImplicitly]
    private void OnGUI()
    {
        major = EditorGUILayout.IntField("Major", major);
        minor = EditorGUILayout.IntField("Minor", minor);
        GUI.SetNextControlName("Version");
        patch = EditorGUILayout.IntField("Patch", patch);

        targetPC = EditorGUILayout.Toggle("Return to PC", targetPC);

        if (GUILayout.Button("Build"))
        {
            Build(String.Format("{0}.{1}.{2}", major, minor, patch), targetPC);
            Close();
        }

        if (first)
        {
            GUI.FocusControl("Version");
            first = false;
            string[] versionSegments = PlayerSettings.bundleVersion.Split('.');
            major = int.Parse(versionSegments[0]);
            minor = int.Parse(versionSegments[1]);
            patch = int.Parse(versionSegments[2]);
        }
    }

    #endregion

    #region Private Methods

    [MenuItem("Tools/Build Quick &b")]
    [UsedImplicitly]
    private static void QuickBuild()
    {
        using (var timer = new SpeedTimer("Built Player"))
        {
            string version = PlayerSettings.bundleVersion;
            string[] versionSegments = version.Split('.');
            int lastVersion = int.Parse(versionSegments[2]) + 1;
            Build(versionSegments[0] + "." + versionSegments[1] + "." + lastVersion, true);
        }
    }


    [MenuItem("Tools/Build %&b")]
    [UsedImplicitly]
    private static void OpenBuildEditor()
    {
        GetWindow<BuildTool>("Build");
    }


    private static void Build(string version, bool pc)
    {
        PlayerSettings.bundleVersion = version;

        // build
        string buildPath = BuildPath + PlayerSettings.productName + " " + version;
        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray(),
                                  buildPath,
                                  BuildTarget.Android,
                                  BuildOptions.ShowBuiltPlayer);

        // move to Drive
        DirectoryInfo directory = new DirectoryInfo(NewestBuildPath);
        foreach (FileInfo file in directory.GetFiles())
        {
            file.Delete();
        }
        File.Copy(buildPath, NewestBuildPath + PlayerSettings.productName + " " + version);

        // return to PC
        if (pc && EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows);
        }
    }

    #endregion
}