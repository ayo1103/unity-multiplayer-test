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

        [MenuItem("Build/All")]
        public static void BuildAll()
        {
            BuildAddressablesForTarget(BuildTarget.StandaloneLinux64);
            BuildLinuxDedicatedServer();

            BuildAddressablesForTarget(BuildTarget.WebGL);
            BuildWebGL();
        }

        [MenuItem("Build/Linux Dedicated Server")]
        public static void BuildLinuxDedicatedServer()
        {
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

        private static void BuildAddressablesForTarget(BuildTarget target)
        {
            // Set the build target
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildPipeline.GetBuildTargetGroup(target), target);

            // Clean and build addressable assets
            var playerDataBuilder = AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilder;
            AddressableAssetSettings.CleanPlayerContent(playerDataBuilder);
            AddressableAssetSettings.BuildPlayerContent();

            Debug.Log($"Addressables built for {target}");
        }

        [MenuItem("Build/WebGL")]
        public static void BuildWebGL()
        {
            var buildOptions = new BuildPlayerOptions
            {
                scenes = GetScenes(),
                locationPathName = WebGLBuildPath,
                target = BuildTarget.WebGL
            };

            var report = BuildPipeline.BuildPlayer(buildOptions);
            LogBuildResult(report, "WebGL");
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