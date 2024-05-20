using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CharacterPropertiesEditor : MonoBehaviour
{
    [MenuItem("Assets/Create/Character Properties")]
    public static void CreateCharacterProperties()
    {
        CharacterProperties asset = ScriptableObject.CreateInstance<CharacterProperties>();

        AssetDatabase.CreateAsset(asset, "Assets/CharacterProperties.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }

    [MenuItem("Assets/Create/Soldier Properties")]
    public static void CreateSoldierProperties()
    {
        SoldierProperties asset = ScriptableObject.CreateInstance<SoldierProperties>();

        AssetDatabase.CreateAsset(asset, "Assets/SoldierProperties.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}
