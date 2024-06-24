using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class GenerateVersionJson : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        string version = Application.version;
        string json = $"{{ \"version\": \"{version}\" }}";
        string path = Path.Combine(report.summary.outputPath, "Build/version.json");

        File.WriteAllText(path, json);
        Debug.Log($"Generated version.json with version {version} at {path}");
    }
}