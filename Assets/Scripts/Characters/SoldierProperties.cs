using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierProperties : CharacterProperties
{
    public GameObject characterPrefab;
    public string characterName;
    public string assetId;
    public Sprite characterImage;
    public GameObject soldierSkillPrefab;
    public bool isOwner { get; set; }
    [TextArea(15, 20)]
    public string description;
}
