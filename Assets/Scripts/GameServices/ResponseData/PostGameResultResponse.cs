using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PostGameResultResponse
{
    public PostGameResultData data;
}

[System.Serializable]
public class PostGameResultData
{
    public string id;
    public string userId;
    public string gameId;
    public string playtime;
    public int map;
    public int level;
    public int before;
    public int score;
    public int after;
    public RewardAsset[] rewardAssets;
}

[System.Serializable]
public class RewardAsset
{
    public string id;
    public string name;
    public string[] gameIds;
    public string image;
    public string path;
    public int maxCard;
    public float rentPoint;
    public float winRate;
    public string description;
    public string mainAssetId;
}
