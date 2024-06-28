using UnityEditor;
using UnityEngine;

public class ReSerializeScriptableObjects : EditorWindow
{
    [MenuItem("Tools/Re-serialize ScriptableObjects")]
    public static void ShowWindow()
    {
        GetWindow<ReSerializeScriptableObjects>("Re-serialize ScriptableObjects");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Re-serialize All ScriptableObjects"))
        {
            ReSerializeAllScriptableObjects();
        }
    }

    private static void ReSerializeAllScriptableObjects()
    {
        string[] guids = AssetDatabase.FindAssets("t:ScriptableObject");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ScriptableObject obj = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            if (obj != null)
            {
                EditorUtility.SetDirty(obj);
                AssetDatabase.SaveAssets();
                Debug.Log($"Re-serialized: {obj.name}");
            }
        }
        AssetDatabase.Refresh();
    }
}
