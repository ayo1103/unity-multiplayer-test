using System.Collections;
using System.IO;
using Unity.EditorCoroutines.Editor;
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
            EditorCoroutineUtility.StartCoroutineOwnerless(BuildAllRoutine());
        }

        [MenuItem("Build/Linux Dedicated Server")]
        public static void BuildLinuxDedicatedServer()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(BuildLinuxDedicatedServerRoutine());
        }

        [MenuItem("Build/WebGL")]
        public static void BuildWebGL()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(BuildWebGLRoutine());
        }

        private static IEnumerator BuildAllRoutine()
        {
            yield return BuildLinuxDedicatedServerRoutine();
            yield return BuildWebGLRoutine();
        }

        private static IEnumerator BuildWebGLRoutine()
        {
            yield return BuildAddressablesForTarget(BuildTarget.WebGL);

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

        private static IEnumerator BuildLinuxDedicatedServerRoutine()
        {
            Debug.Log($"BuildLinuxDedicatedServerRoutine -- Start");
            yield return BuildAddressablesForTarget(BuildTarget.StandaloneLinux64);
            yield return new EditorWaitForSeconds(1);
            
            var buildOptions = new BuildPlayerOptions
            {
                scenes = GetScenes(),
                locationPathName = DedicatedServerBuildPath,
                target = BuildTarget.StandaloneLinux64,
                subtarget = (int) StandaloneBuildSubtarget.Server
            };
            
            var report = BuildPipeline.BuildPlayer(buildOptions);
            LogBuildResult(report, "Linux Dedicated Server");
            Debug.Log($"BuildLinuxDedicatedServerRoutine -- End");
        }

        private static IEnumerator BuildAddressablesForTarget(BuildTarget target)
        {
            Debug.Log($"BuildAddressablesForTarget = {target}");

            // Set the build target
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildPipeline.GetBuildTargetGroup(target), target);

            // Wait until the build target has switched
            while (EditorUserBuildSettings.activeBuildTarget != target)
            {
                yield return null;
            }

            Debug.Log($"EditorUserBuildSettings.activeBuildTarget = {target}");

            // Clean and build addressable assets
            var playerDataBuilder = AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilder;
            AddressableAssetSettings.CleanPlayerContent(playerDataBuilder);
            AddressableAssetSettings.BuildPlayerContent();
            Debug.Log($"Addressables built for {target}");
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