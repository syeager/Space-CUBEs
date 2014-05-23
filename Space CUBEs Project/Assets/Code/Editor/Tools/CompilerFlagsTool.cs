// Steve Yeager
// 5.22.2014

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annotations;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Allows quick toggling of compiler flags.
/// </summary>
public class CompilerFlagsTool : EditorWindow
{
    #region Static Fields

    /// <summary>List of all current flags.</summary>
    private static List<string> flagNames;

    /// <summary>Statuses of the current flags.</summary>
    private static List<bool> flagsStatuses;

    /// <summary>New flag being added.</summary>
    private static string newString = "";

    #endregion

    #region Const Fields

    /// <summary>String separating the flags.</summary>
    private const char FlagSep = ';';

    /// <summary>Postfix for turning a flag off.</summary>
    private const string OffPostFix = "_OFF";

    #endregion

    #region Editor Overrides

    [UsedImplicitly]
    [MenuItem("Tools/Compiler Flags &C")]
    private static void Init()
    {
        GetWindow<CompilerFlagsTool>("Compiler Flags");
    }


    [UsedImplicitly]
    private void OnGUI()
    {
        // compiling
        if (EditorApplication.isCompiling)
        {
            GUILayout.Label("Compiling...");
            return;
        }

        // regrab flags
        if (flagNames == null)
        {
            GetFlags();
        }

        // force focus
        EditorGUI.FocusTextInControl("NewFlag");

        // flags
        for (int i = 0; i < flagNames.Count; i++)
        {
            GUILayout.BeginHorizontal();
            {
                // flag
                flagsStatuses[i] = EditorGUILayout.Toggle(flagNames[i], flagsStatuses[i]);
                // delete
                if (GUILayout.Button("-", EditorStyles.miniButton, GUILayout.Width(40f)))
                {
                    flagNames.RemoveAt(i);
                    flagsStatuses.RemoveAt(i);
                    return;
                }
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }

        // new flag
        GUILayout.Space(10f);
        GUILayout.BeginHorizontal();
        {
            GUI.SetNextControlName("NewFlag");
            newString = EditorGUILayout.TextField(newString);
            if (GUILayout.Button("Add") || (Event.current.isKey && Event.current.keyCode == KeyCode.Return))
            {
                if (!string.IsNullOrEmpty(newString) && !flagNames.Contains(newString))
                {
                    AddFlag();
                    Repaint();
                }
            }
        }
        GUILayout.EndHorizontal();

        // buttons
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        {
            // save
            if (GUILayout.Button("Save"))
            {
                SetFlags();
            }
            // reset
            if (GUILayout.Button("Reset"))
            {
                GetFlags();
            }
        }
        GUILayout.EndHorizontal();
    }

    #endregion

    #region Static Methods

    /// <summary>
    /// Get compiler flags from PlayerSettings.
    /// </summary>
    private static void GetFlags()
    {
        flagNames = new List<string>();
        flagsStatuses = new List<bool>();

        flagNames = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(FlagSep).ToList();
        for (int i = 0; i < flagNames.Count; i++)
        {
            if (flagNames[i].Contains(OffPostFix))
            {
                flagNames[i] = flagNames[i].Replace(OffPostFix, string.Empty);
                flagsStatuses.Add(false);
            }
            else
            {
                flagsStatuses.Add(true);
            }
        }
    }


    /// <summary>
    /// Give PlayerSettings new compiler flags.
    /// </summary>
    private static void SetFlags()
    {
        StringBuilder flags = new StringBuilder();
        for (int i = 0; i < flagNames.Count; i++)
        {
            flags.Append(flagNames[i]);
            if (!flagsStatuses[i])
            {
                flags.Append(OffPostFix);
            }
            flags.Append(FlagSep);
        }

        flagNames = null;
        flagsStatuses = null;
        PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, flags.ToString());
    }


    /// <summary>
    /// Add a new flag to the cached flag list.
    /// </summary>
    private static void AddFlag()
    {
        flagNames.Add(newString);
        flagsStatuses.Add(true);
        newString = string.Empty;
    }

    #endregion
}