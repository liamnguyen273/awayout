using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : ScriptableObject
{
    public WeaponData[] weaponDatas;
    public SoldierProperties[] characterProperties;

    public WeaponData CurrentWeapon { get; set; }

    public SoldierProperties CurrentSoldier { get; set; }

    public void UpdateCurrentAssetIds(string[] currentAssetIds)
    {
        CurrentWeapon = null;
        CurrentSoldier = null;
        for (int i = 0; i < currentAssetIds.Length; i++)
        {
            for (int j = 0; j < weaponDatas.Length; j++)
            {
                if (weaponDatas[j].itemId == currentAssetIds[i] && weaponDatas[j].isOwner)
                {
                    CurrentWeapon = weaponDatas[j];
                }
            }

            for (int j = 0; j < characterProperties.Length; j++)
            {
                if (characterProperties[j].assetId == currentAssetIds[i] && characterProperties[j].isOwner)
                {
                    CurrentSoldier = characterProperties[j];
                }
            }

        }
    }

    public void SetDefaultAssetIds()
    {
        if (CurrentWeapon == null)
        {
            for (int i = 0; i < weaponDatas.Length; i++)
            {
                if (weaponDatas[i].isOwner)
                {
                    CurrentWeapon = weaponDatas[i];
                    break;
                }
            }

        }

        if (CurrentSoldier == null)
        {
            for (int i = 0; i < characterProperties.Length; i++)
            {
                if (characterProperties[i].isOwner)
                {
                    CurrentSoldier = characterProperties[i];
                    break;
                }
            }
        }

    }
}
