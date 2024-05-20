using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemData
{
    public string itemId;
    public string itemName;
    public Sprite itemImage;
    [TextArea(15, 20)]
    public string description;
}
