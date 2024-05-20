using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameSettingsEditor : MonoBehaviour
{
    [MenuItem("Assets/Create/Game Settings")]
    public static void CreateMyAsset()
    {
        GameSettings asset = ScriptableObject.CreateInstance<GameSettings>();

        AssetDatabase.CreateAsset(asset, "Assets/GameSettings.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}
