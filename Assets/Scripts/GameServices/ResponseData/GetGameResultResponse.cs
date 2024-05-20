using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GetGameResultResponse
{
    public GetGameResultData[] data;
    public int status;
}



[System.Serializable]
public class GetGameResultData
{
    public int map;
    public LevelStar[] stars;

}

[System.Serializable]
public class LevelStar
{
    public int level;
    public int star;
}
