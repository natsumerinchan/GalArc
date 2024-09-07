﻿using GalArc.GUI;
using Log;
using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using GalArc.Properties;

namespace GalArc.Controller
{
    internal class UpdateVersion
    {

        internal static string currentVersion = Resource.Global.CurrentVersion;

        internal static string latestVersion = Resources.ab_lb_LatestVer;

        private const string versionPath = "version.txt";

        private const string versionUrl = "https://raw.githubusercontent.com/detached64/GalArc/master/docs/version.txt";

        internal static bool isNewVerExist = false;

        internal static void InitVersion()
        {
            AboutWindow.Instance.ab_lbCurrentVer.Text = currentVersion;
        }

        internal static void DownloadVersion()
        {
            if (File.Exists(versionPath))
            {
                File.Delete(versionPath);
            }

            try
            {
                using (System.Net.WebClient webClient = new System.Net.WebClient())
                {
                    webClient.DownloadFile(versionUrl, versionPath);
                }
            }
            catch (Exception)
            {
                LogUtility.ShowCheckError();
            }
        }
        internal static string OpenVersion()
        {
            if (!File.Exists(versionPath))
            {
                return "Unknown";
            }
            else
            {
                latestVersion = File.ReadAllText(versionPath);
                return latestVersion;
            }
        }
        internal static void CompareVersion(string latestVersion)
        {
            isNewVerExist = false;
            if (latestVersion == "Unknown" || !latestVersion.Contains("."))
            {
                return;
            }
            if (File.Exists(versionPath))
            {
                File.Delete(versionPath);
            }

            string[] parts1 = currentVersion.Split('.');
            string[] parts2 = latestVersion.Split('.');

            for (int i = 0; i < Math.Max(parts1.Length, parts2.Length); i++)
            {
                int num1 = (i < parts1.Length) ? int.Parse(parts1[i]) : 0;
                int num2 = (i < parts2.Length) ? int.Parse(parts2[i]) : 0;

                if (num1 < num2)
                {
                    isNewVerExist = true;
                    break;
                }
            }
        }

        internal static async Task UpdateProgram()
        {
            await Task.Run(() =>
            {
                DownloadVersion();
                CompareVersion(OpenVersion());
            });
        }

    }
}
