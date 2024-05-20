using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PostPlayGameResponse
{
    public PlayGameData data;
    public int status;
}

[System.Serializable]
public class PlayGameData
{
    public string id;
    public string userId;
    public string gameId;
    public int map;
    public int level;
    public int score;
    public int playTurn;
    public string lastPlayDate;
    public int turnPlayed;
    public int maxMap;
    public int maxLevel;
}

