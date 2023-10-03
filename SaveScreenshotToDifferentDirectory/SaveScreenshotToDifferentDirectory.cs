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

        [HarmonyPatch(typeof(PhotoCaptureManager))]
        [HarmonyPatch("TakePhoto")]
        public static class PhotoCaptureManager_TakePhoto_Patch
        {
            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var foundMassUsageMethod = false;
                var startIndex = -1;
                var endIndex = -1;

                var codes = new List<CodeInstruction>(instructions);
                for (var i = 0; i < codes.Count; i++)
                {
                    if (codes[i].opcode == OpCodes.Ldarg_0)
                    {
                        if (foundMassUsageMethod)
                        {
                            Msg("END " + i);

                            endIndex = i; // include current 'ret'
                            break;
                        }

                        Msg("START " + (i + 1));

                        startIndex = i + 1; // exclude current 'call'

                        for (var j = startIndex; j < codes.Count; j++)
                        {
                            if (codes[j].opcode == OpCodes.Call)
                                break;
                            var strOperand = (codes[j].operand as MethodInfo)?.Name;
                            if (strOperand == "PlayCaptureSound")
                            {
                                foundMassUsageMethod = true;
                                break;
                            }
                        }
                    }
                }

                if (startIndex > -1 && endIndex > -1)
                {
                    // we cannot remove the first code of our range since some jump actually jumps to
                    // it, so we replace it with a no-op instead of fixing that jump (easier).
                    codes[startIndex].opcode = OpCodes.Nop;
                    codes.RemoveRange(startIndex + 1, endIndex - startIndex - 1);
                }

                return codes.AsEnumerable();
            }
        }
    }
}