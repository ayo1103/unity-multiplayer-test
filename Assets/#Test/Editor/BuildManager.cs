using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace WebMultiplayerTest
{
    public static class BuildManager
    {
        private const string DedicatedServerBuildPath = "Builds/DedicatedServer/server.x86_64";
        private const string WebGLBuildPath = "Builds/WebGL";

        [MenuItem("Build/Dedicated Server")]
        public static void BuildServer()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneLinux64);
            var playerDataBuilder = AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilder;
            AddressableAssetSettings.CleanPlayerContent(playerDataBuilder);
            AddressableAssetSettings.BuildPlayerContent();

            var buildOptions = new BuildPlayerOptions
            {
                scenes = GetScenes(),
                locationPathName = DedicatedServerBuildPath,
                target = BuildTarget.StandaloneLinux64,
                subtarget = (int) StandaloneBuildSubtarget.Server
            };

            var report = BuildPipeline.BuildPlayer(buildOptions);
            LogBuildResult(report, "Linux Dedicated Server");
        }

        [MenuItem("Build/WebGL Client")]
        public static void BuildClient()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
            var playerDataBuilder = AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilder;
            AddressableAssetSettings.CleanPlayerContent(playerDataBuilder);
            AddressableAssetSettings.BuildPlayerContent();

            var buildOptions = new BuildPlayerOptions
            {
                scenes = GetScenes(),
                locationPathName = WebGLBuildPath,
                target = BuildTarget.WebGL
            };

            var report = BuildPipeline.BuildPlayer(buildOptions);
            LogBuildResult(report, "WebGL");
            GenerateVersionJsonFile(report);
        }

        private static void GenerateVersionJsonFile(BuildReport report)
        {
            string version = Application.version;
            string json = $"{{ \"version\": \"{version}\" }}";
            string path = Path.Combine(report.summary.outputPath, "Build/version.json");

            if (!Directory.Exists(Path.Combine(report.summary.outputPath, "Build")))
            {
                Directory.CreateDirectory(Path.Combine(report.summary.outputPath, "Build"));
            }

            File.WriteAllText(path, json);
            Debug.Log($"Generated version.json with version {version} at {path}");
        }

        private static string[] GetScenes()
        {
            var scenes = new string[EditorBuildSettings.scenes.Length];
            for (int i = 0; i < scenes.Length; i++)
            {
                scenes[i] = EditorBuildSettings.scenes[i].path;
            }

            return scenes;
        }

        private static void LogBuildResult(BuildReport report, string buildName)
        {
            if (report.summary.result != BuildResult.Succeeded)
            {
                Debug.LogError($"{buildName} Build failed!");
            }
            else
            {
                Debug.Log($"{buildName} Build succeeded!");
            }
        }
    }
}