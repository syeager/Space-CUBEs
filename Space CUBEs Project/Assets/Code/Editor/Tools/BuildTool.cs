// Steve Yeager
// 4.27.2014

using System;
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

    private bool first = true;

    #endregion


    #region EditorWindow Overrides

    [UsedImplicitly]
    private void OnGUI()
    {
        GUI.SetNextControlName("Major");
        major = EditorGUILayout.IntField("Major", major);
        minor = EditorGUILayout.IntField("Minor", minor);
        patch = EditorGUILayout.IntField("Patch", patch);

        if (GUILayout.Button("Build"))
        {
            Build(String.Format("{0}.{1}.{2}", major, minor, patch));
            Close();
        }

        if (first)
        {
            GUI.FocusControl("Major");
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
            Build(versionSegments[0] + "." + versionSegments[1] + "." + lastVersion);
        }
    }


    [MenuItem("Tools/Build %&b")]
    [UsedImplicitly]
    private static void OpenBuildEditor()
    {
        GetWindow<BuildTool>("Build");
    }


    private static void Build(string version)
    {
        string oldVersion = PlayerSettings.bundleVersion;
        PlayerSettings.bundleVersion = version;
        string path = EditorUserBuildSettings.GetBuildLocation(BuildTarget.Android).Replace(oldVersion, version);

        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray(),
                                  path,
                                  BuildTarget.Android,
                                  BuildOptions.ShowBuiltPlayer);
    }

    #endregion
}