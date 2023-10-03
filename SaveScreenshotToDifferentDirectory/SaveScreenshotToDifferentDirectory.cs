using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using FrooxEngine;
using ResoniteModLoader;
using Steamworks;

namespace SaveScreenshotToDifferentDirectory
{
    public class SaveScreenshotToDifferentDirectory : ResoniteMod
    {
        internal const string VERSION = "2.0.0";
        public override string Name => "SaveScreenshotToDifferentDirectory";
        public override string Author => "Meister1593";
        public override string Version => VERSION;
        public override string Link => "https://github.com/Meister1593/SaveScreenshotToDifferentDirectory";

        public override void OnEngineInit()
        {
            _config = GetConfiguration();
            new Harmony("net.meister1593.SaveScreenshotToDifferentDirectory").PatchAll();
        }

        private static ModConfiguration _config;

        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<string> ScreenshotPath =
            new("Path to screenshots", "Path to screenshots folder in OS", () => "");

        [HarmonyPatch(typeof(SteamScreenshots), nameof(SteamScreenshots.AddVRScreenshotToLibrary))]
        public class SteamScreenshots_AddVRScreenshotToLibrary_Patch
        {
            private static void Prefix(EVRScreenshotType eType,
                string pchFilename,
                string pchVRFilename)
            {
                var dateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                var screenshotPath =
                    $"{_config.GetValue(ScreenshotPath)}/{dateTime}_1.jpg"; // Steam screenshot name format
                if (string.IsNullOrEmpty(_config.GetValue(ScreenshotPath)))
                {
                    return;
                }

                Msg($"Path to screenshot saved: {screenshotPath}");
                File.Copy(pchFilename, screenshotPath);
            }
        }
    }
}