using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APIHelper
{
    public static string ACCESS_TOKEN_TEST = "68e77e1475e644298dbf9c2d41540078";
    //2691ee6c351344719a986200d107b5ac
    //2691ee6c351344719a986200d107b5ab
    public static string HEADER_ACCESS_TOKEN = "x-access-token";
    public static string BASE_URL_PROD = "https://prod.roseon.finance/roseon/point-system/";
    public static string BASE_URL_STAGGING = "http://104.248.97.66:8081/roseon/point-system/";
    public static string BASE_URL_DEV = "http://104.248.97.66:8081/roseon/point-system/";
    public static string GET_USER_PROFILE = "user/profile";
    public static string GET_USER_PROFILE_FOR_GAME = "game";
    public static string POST_PLAY = "game/play";
    public static string POST_GAME_RESULT = "game/result";
    public static string POST_UNLOCK_CHARACTER = "user/asset/unlock";
    public static string GET_GAME_RESULT = "game/result";
    public static string GET_USER_ASSET = "user/asset";
    public static string gameId = "a_way_out";
    public static string PUT_CURRENT_ASSET = "game/current-asset";
    public static string GET_GAME_DETAILS = "game/details";
    public static string ASSET_RENT = "user/asset/rent";

}
