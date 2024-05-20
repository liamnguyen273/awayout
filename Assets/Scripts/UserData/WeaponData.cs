using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponData : ItemData
{
    public GameObject weaponPrefab;
    public WeaponRarity weaponRarity;
    public bool isOwner { get; set; }
}

public enum WeaponRarity
{
    COMMON, UNCOMMON, RARE, EPIC, LEGENDARY
}
