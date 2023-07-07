using HarmonyLib;
using NeosModLoader;
using System;
using System.IO;
using Steamworks;

namespace SaveScreenshotToDifferentDirectory
{
    public class SaveScreenshotToDifferentDirectory : NeosMod
    {
        internal const string VERSION = "1.0.0";
            
        public override string Name => "SaveScreenshotToDifferentDirectory";
        public override string Author => "PLYSHKA";
        public override string Version => VERSION;
        public override string Link => "https://github.com/Meister1593/SaveScreenshotToDifferentDirectory";

        public override void OnEngineInit()
        {
            Config = GetConfiguration();
            new Harmony("net.plyshka.SaveScreenshotToDifferentDirectory").PatchAll();
        }

        private static ModConfiguration Config;

        [AutoRegisterConfigKey]
        public static ModConfigurationKey<string> ScreenshotPath =
            new("Path to screenshots", "Path to screenshots folder in OS", () => "");

        [HarmonyPatch(typeof(SteamScreenshots), nameof(SteamScreenshots.AddVRScreenshotToLibrary))]
        public class SteamScreenshots_AddVRScreenshotToLibrary_Patch
        {
            static void Prefix(EVRScreenshotType eType,
                string pchFilename,
                string pchVRFilename)
            {
                string dateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                string screenshotPath =
                    $"{Config.GetValue(ScreenshotPath)}/{dateTime}_1.jpg"; // Steam screenshot name format
                if (string.IsNullOrEmpty(Config.GetValue(ScreenshotPath)))
                {
                    return;
                }

                Warn(screenshotPath);
                File.Copy(pchFilename, screenshotPath);
            }
        }
    }
}