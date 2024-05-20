using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class GeneralSettingsEditor : MonoBehaviour
{
    [MenuItem("Assets/Create/General Settings")]
    public static void CreateMyAsset()
    {
        GeneralSettings asset = ScriptableObject.CreateInstance<GeneralSettings>();

        AssetDatabase.CreateAsset(asset, "Assets/GeneralSettings.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}
