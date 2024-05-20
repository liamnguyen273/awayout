using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GetUserProfileResponse
{
    public GetUserProfileData data;
    public int status;
}


[System.Serializable]
public class GetUserProfileData
{
    public string id;
    public string name;
    public string avatar;
    public int point;
    public int extraSpin;
}