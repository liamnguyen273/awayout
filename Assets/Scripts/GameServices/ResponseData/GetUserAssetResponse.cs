using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GetUserAssetResponse
{
    public UserAssetData[] data;
}

[System.Serializable]
public class UserAssetData
{
    public string assetId;
    public string userAssetId;
    public string assetName;
    public ItemType itemType
    {
        get
        {
            System.Enum.TryParse(type, out ItemType _itemType);
            return _itemType;

        }
    }
    public string type;
    public string tokenId;
    public string[] gameIds;
    public string expiredAt;
    public int card;
    public bool status;
    public int maxCard;
    public string path;
    public int rentPoin;
    public bool rentStatus;
    public string image;
    public string description;
    public string selling;
    public string mainAssetId;
}

