using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GetGameDetails 
{
    public  GameDetailsData data;
    public int status;
}

[System.Serializable]
public class GameDetailsData
{
    public string gameId;
    public string name;
    public string logo;
    public string banner;
    public string description;
    public string genre;
    public string about;
    public string howToPlay;
    public string url;
    public int maxLevel;
    public int oneStarPoint;
    public int twoStarPoint;
    public int threeStarPoint;
}