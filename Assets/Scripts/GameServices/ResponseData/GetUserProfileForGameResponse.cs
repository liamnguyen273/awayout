using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[System.Serializable]
public class GetUserProfileForGameResponse
{
    public GetUserProfileForGameData data;
    public int status;
}

[System.Serializable]
public class GetUserProfileForGameData
{
    public string id;
    public string userId;
    public string gameId;
    public int map;
    public int level;
    public int star;
    public int score;
    public string[] currentAssetIds;
    public int playTurn;
    public string lastPlayDate;
    public int maxMap;
    public int maxLevel;

    public bool CheckSelected(string assetId)
    {
        return Array.FindIndex(currentAssetIds, (id) => id == assetId) != -1;
    }

}
