using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(ItemDatabase))]
public class ItemDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Sort"))
        {
            SortByName();
            SortByRarity();
        }

        if (GUILayout.Button("Sort by Name"))
        {
            SortByName();
        }

        if (GUILayout.Button("Sort by Rarity"))
        {
            SortByRarity();
        }
        DrawDefaultInspector();
    }



    [MenuItem("Assets/Create/Item Database")]
    public static void CreateMyAsset()
    {
        ItemDatabase asset = ScriptableObject.CreateInstance<ItemDatabase>();

        AssetDatabase.CreateAsset(asset, "Assets/ItemDatabse.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }

    public void SortByName()
    {
        ItemDatabase asset = (ItemDatabase)target;

        for (int i = 0; i < asset.weaponDatas.Length - 1; i++)
        {
            for (int j = i; j < asset.weaponDatas.Length; j++)
            {
                if (asset.weaponDatas[i].itemId.CompareTo(asset.weaponDatas[j].itemId) > 0)
                {
                    WeaponData temp = asset.weaponDatas[i];
                    asset.weaponDatas[i] = asset.weaponDatas[j];
                    asset.weaponDatas[j] = temp;
                }
            }
        }
    }

    public void SortByRarity()
    {
        ItemDatabase asset = (ItemDatabase)target;

        for (int i = 0; i < asset.weaponDatas.Length - 1; i++)
        {
            for (int j = i; j < asset.weaponDatas.Length; j++)
            {
                if ((int)asset.weaponDatas[i].weaponRarity > (int)asset.weaponDatas[j].weaponRarity)
                {
                    WeaponData temp = asset.weaponDatas[i];
                    asset.weaponDatas[i] = asset.weaponDatas[j];
                    asset.weaponDatas[j] = temp;
                }
            }
        }
    }

}
