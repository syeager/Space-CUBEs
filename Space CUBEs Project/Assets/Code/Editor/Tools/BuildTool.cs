// Steve Yeager
// 4.27.2014

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
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
    private bool sendEmail = true;

    private bool first = true;

    #endregion

    #region Const Fields

    private const string BuildPath = "D:/Documents/GitHub/Space-CUBEs/Builds/PreAlpha/";
    private const string NewestBuildPath = "D:/Documents/Google Drive/Space CUBEs/Builds/";
    private const string APK = ".apk";

    private const string EmailYeager = "yeagz7@gmail.com";
    private const string EmailWilliams = "steve.f.yeager@gmail.com";
//    private const string EmailWilliams = "sam.erin.williams@gmail.com";
    private const string EmailPassword = "Yeager_7";

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
        sendEmail = EditorGUILayout.Toggle("Send Email", sendEmail);

        if (GUILayout.Button("Build"))
        {
            Build(String.Format("{0}.{1}.{2}", major, minor, patch), targetPC, sendEmail);
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
            Build(versionSegments[0] + "." + versionSegments[1] + "." + lastVersion, true, false);
        }
    }


    [MenuItem("Tools/Build %&b")]
    [UsedImplicitly]
    private static void OpenBuildEditor()
    {
        GetWindow<BuildTool>("Build");
    }


    private static void Build(string version, bool pc, bool email)
    {
        PlayerSettings.bundleVersion = version;

        // build
        string buildPath = BuildPath + PlayerSettings.productName + " " + version + APK;
        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray(),
                                  buildPath,
                                  BuildTarget.Android,
                                  BuildOptions.None);

        // move to Drive
        DirectoryInfo directory = new DirectoryInfo(NewestBuildPath);
        foreach (FileInfo file in directory.GetFiles())
        {
            file.Delete();
        }
        File.Copy(buildPath, NewestBuildPath + PlayerSettings.productName + " " + version + APK);

        // email
        if (email)
        {
            Thread thread = new Thread(SendEmail);
            thread.Start(new Dictionary<string, object> { { "build", PlayerSettings.productName + " " + version }, { "buildPath", buildPath } });
            thread.Join();
        }

        // return to PC
        if (pc && EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows);
        }
    }


    public static void SendEmail(object info)
    {
        Debug.Log("sending mail...");

        string build = (string)(info as Dictionary<string, object>)["build"];
        string buildPath = (string)(info as Dictionary<string, object>)["buildPath"];
        MailAddress fromAddress = new MailAddress(EmailYeager);
        MailAddress toAddress = new MailAddress(EmailWilliams);
        string subject = build;

        SmtpClient smtp = new SmtpClient()
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            Credentials = new NetworkCredential(fromAddress.Address, EmailPassword)
        };

        ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

        using (MailMessage message = new MailMessage(fromAddress, toAddress) { Subject = subject, IsBodyHtml = true })
        {
            // Create the file attachment for this e-mail message.
            Attachment data = new Attachment(buildPath, MediaTypeNames.Application.Octet);

            // Add time stamp information for the file.
            ContentDisposition disposition = data.ContentDisposition;
            disposition.CreationDate = File.GetCreationTime(buildPath);
            disposition.ModificationDate = File.GetLastWriteTime(buildPath);
            disposition.ReadDate = File.GetLastAccessTime(buildPath);

            // Add the file attachment to this e-mail message.
            message.Attachments.Add(data);

            smtp.SendCompleted += (sender, args) =>
            {
                if (args.Cancelled)
                {
                    Debug.LogError("Build Email Cancelled.");
                }
                else if (args.Error != null)
                {
                    Debug.LogException(args.Error);
                }
                else
                {
                    Debug.Log("Build Email Sent");
                }
            };
            smtp.Send(message);
        }
    }

    #endregion
}