using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WeaponPropertiesEditor : MonoBehaviour
{
    [MenuItem("Assets/Create/Weapon Properties")]
    public static void CreateMyAsset()
    {
        WeaponProperties asset = ScriptableObject.CreateInstance<WeaponProperties>();

        AssetDatabase.CreateAsset(asset, "Assets/WeaponProperties.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}
