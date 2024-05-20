using MoreMountains.Tools;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class GameServices : Singleton<GameServices>
{
    public ServerType serverType;
    private const int requestTimeout = 5;
    private string tokenId;
    public override void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
#if UNITY_WEBGL
        URLParameters.Instance.RegisterOnDone((url) =>
        {
            url.SearchParameters.TryGetValue("token", out tokenId);
        });
#endif

        _instance = this;
        DontDestroyOnLoad(this);
        base.Awake();
        Debug.Log("ACCESS_TOKEN: " + ACCESS_TOKEN);
    }

    public void GetUserProfile(Action<GetUserProfileData> eventSuccess, Action<string> eventFailed, int retryTime = 5)
    {
        StartCoroutine(RoutineGetUserProfile(eventSuccess, eventFailed, retryTime));
    }
    private IEnumerator RoutineGetUserProfile(Action<GetUserProfileData> eventSuccess, Action<string> eventFailed, int retryTime)
    {
        var targetUrl = string.Concat(APIBASEURL, APIHelper.GET_USER_PROFILE);

        using (UnityWebRequest www = UnityWebRequest.Get(targetUrl))
        {
            www.timeout = requestTimeout;
            www.SetRequestHeader(APIHelper.HEADER_ACCESS_TOKEN, ACCESS_TOKEN);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                if (retryTime > 0)
                {
                    retryTime--;
                    yield return new WaitForSeconds(1f);
                    GetUserProfile(eventSuccess, eventFailed, retryTime);
                }
                else
                    eventFailed(www.error);
            }
            else
            {
                var jsonReturn = www.downloadHandler.text;
                var modelReturn = JsonUtility.FromJson<GetUserProfileResponse>(jsonReturn);
                eventSuccess(modelReturn.data);
            }
        }
    }

    public void GetGameDetails(Action<GameDetailsData> eventSuccess, Action<string> eventFailed, int retryTime = 5)
    {
        StartCoroutine(RoutinGetGameDetails(eventSuccess, eventFailed, retryTime));
    }
    private IEnumerator RoutinGetGameDetails(Action<GameDetailsData> eventSuccess, Action<string> eventFailed, int retryTime)
    {
        var targetUrl = string.Concat(APIBASEURL, APIHelper.GET_GAME_DETAILS);
        targetUrl = string.Concat(targetUrl, "?gameId=" + APIHelper.gameId);
        using (UnityWebRequest www = UnityWebRequest.Get(targetUrl))
        {
            www.timeout = requestTimeout;
            www.SetRequestHeader(APIHelper.HEADER_ACCESS_TOKEN, ACCESS_TOKEN);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                if (retryTime > 0)
                {
                    retryTime--;
                    yield return new WaitForSeconds(1f);
                    GetGameDetails(eventSuccess, eventFailed, retryTime);
                }
                else
                    eventFailed(www.error);
            }
            else
            {
                var jsonReturn = www.downloadHandler.text;
                var modelReturn = JsonUtility.FromJson<GetGameDetails>(jsonReturn);
                eventSuccess(modelReturn.data);
            }
        }
    }

    public void GetUserProfileForGame(Action<GetUserProfileForGameData> eventSuccess, Action<string> eventFailed, int retryTime = 5)
    {
        StartCoroutine(RoutineGetUserProfileForGame(eventSuccess, eventFailed, retryTime));
    }
    private IEnumerator RoutineGetUserProfileForGame(Action<GetUserProfileForGameData> eventSuccess, Action<string> eventFailed, int retryTime)
    {
        var targetUrl = string.Concat(APIBASEURL, APIHelper.GET_USER_PROFILE_FOR_GAME);
        targetUrl = Path.Combine(targetUrl, APIHelper.gameId);
        using (UnityWebRequest www = UnityWebRequest.Get(targetUrl))
        {
            www.timeout = requestTimeout;
            www.SetRequestHeader(APIHelper.HEADER_ACCESS_TOKEN, ACCESS_TOKEN);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                if (retryTime > 0)
                {
                    retryTime--;
                    yield return new WaitForSeconds(1);
                    GetUserProfileForGame(eventSuccess, eventFailed, retryTime);
                }
                else
                    eventFailed(www.error);
            }
            else
            {
                var jsonReturn = www.downloadHandler.text;
                var modelReturn = JsonUtility.FromJson<GetUserProfileForGameResponse>(jsonReturn);
                eventSuccess(modelReturn.data);
            }
        }
    }

    public void PostPlayGame(string[] assetIds, Action<PlayGameData> eventSuccess, Action<ErrorMessage> eventFailed, int retryTime = 5)
    {
        StartCoroutine(RoutinePostPlayGame(assetIds, eventSuccess, eventFailed, retryTime));
    }
    private IEnumerator RoutinePostPlayGame(string[] assetIds, Action<PlayGameData> eventSuccess, Action<ErrorMessage> eventFailed, int retryTime)
    {
        var targetUrl = string.Concat(APIBASEURL, APIHelper.POST_PLAY);
        string assetIdsField = JsonConvert.SerializeObject(assetIds);
        Dictionary<string, object> body = new Dictionary<string, object>() { ["gameId"] = APIHelper.gameId, ["assetIds"] = assetIds };
        string jsonBody = JsonConvert.SerializeObject(body);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        using (UnityWebRequest www = new UnityWebRequest(targetUrl, "POST"))
        {
            www.timeout = requestTimeout;
            www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            www.SetRequestHeader(APIHelper.HEADER_ACCESS_TOKEN, ACCESS_TOKEN);
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                ErrorMessage error = JsonUtility.FromJson<ErrorMessage>(www.downloadHandler.text);
                if (retryTime > 0 && !error.errorCode.Contains("PLAY_TURN_NOT_ENOUGH"))
                {
                    retryTime--;
                    yield return new WaitForSeconds(1);
                    PostPlayGame(assetIds, eventSuccess, eventFailed, retryTime);
                }
                else
                {

                    eventFailed(error);
                }

            }
            else
            {
                var jsonReturn = www.downloadHandler.text;
                var modelReturn = JsonUtility.FromJson<PostPlayGameResponse>(jsonReturn);
                eventSuccess(modelReturn.data);
            }
        }
    }
    public void GetUserAsset(Action<UserAssetData[]> eventSuccess, Action<string> eventFailed, int retryTime = 5)
    {
        StartCoroutine(RoutineGetUserAsset(eventSuccess, eventFailed, retryTime));
    }
    private IEnumerator RoutineGetUserAsset(Action<UserAssetData[]> eventSuccess, Action<string> eventFailed, int retryTime)
    {
        var targetUrl = string.Concat(APIBASEURL, APIHelper.GET_USER_ASSET);
        targetUrl = string.Concat(targetUrl, "?gameId=" + APIHelper.gameId);
        using (UnityWebRequest www = UnityWebRequest.Get(targetUrl))
        {
            www.timeout = requestTimeout;
            www.SetRequestHeader(APIHelper.HEADER_ACCESS_TOKEN, ACCESS_TOKEN);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                if (retryTime > 0)
                {
                    retryTime--;
                    yield return new WaitForSeconds(1);
                    GetUserAsset(eventSuccess, eventFailed, retryTime);
                }
                else
                {
                    eventFailed(www.error);
                }
            }
            else
            {
                var jsonReturn = www.downloadHandler.text;
                var modelReturn = JsonUtility.FromJson<GetUserAssetResponse>(jsonReturn);
                eventSuccess(modelReturn.data);
            }
        }
    }

    public void PutUpdateCurrentAsset(string[] assetIds, Action eventSuccess, Action<string> eventFailed, int retryTime = 5)
    {
        StartCoroutine(RoutinePutUpdateCurrentAsset(assetIds, eventSuccess, eventFailed, retryTime));
    }
    private IEnumerator RoutinePutUpdateCurrentAsset(string[] assetIds, Action eventSuccess, Action<string> eventFailed, int retryTime)
    {
        var targetUrl = string.Concat(APIBASEURL, APIHelper.PUT_CURRENT_ASSET);
        Dictionary<string, object> body = new Dictionary<string, object>()
        {
            ["gameId"] = APIHelper.gameId,
            ["currentAssetIds"] = string.Join(",", assetIds)
        };
        string jsonBody = JsonConvert.SerializeObject(body);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        using (UnityWebRequest www = new UnityWebRequest(targetUrl, "PUT"))
        {
            www.timeout = requestTimeout;
            www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            www.SetRequestHeader(APIHelper.HEADER_ACCESS_TOKEN, ACCESS_TOKEN);
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                if (retryTime > 0)
                {
                    retryTime--;
                    yield return new WaitForSeconds(1);
                    PutUpdateCurrentAsset(assetIds, eventSuccess, eventFailed, retryTime);
                }
                else
                {
                    eventFailed(www.error);
                }
            }
            else
            {
                var jsonReturn = www.downloadHandler.text;
                eventSuccess();
            }
        }
    }

    public void PostGameResult(BodyPlayGame bodyPlayGame, Action<PostGameResultData> eventSuccess, Action<string> eventFailed, int retryTime = 5)
    {
        StartCoroutine(RoutinePostGameResult(bodyPlayGame, eventSuccess, eventFailed, retryTime));
    }
    private IEnumerator RoutinePostGameResult(BodyPlayGame bodyPlayGame, Action<PostGameResultData> eventSuccess, Action<string> eventFailed, int retryTime)
    {
        var targetUrl = string.Concat(APIBASEURL, APIHelper.POST_GAME_RESULT);
        string jsonBody = JsonConvert.SerializeObject(bodyPlayGame);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        using (UnityWebRequest www = new UnityWebRequest(targetUrl, "POST"))
        {
            www.timeout = requestTimeout;
            www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            www.SetRequestHeader(APIHelper.HEADER_ACCESS_TOKEN, ACCESS_TOKEN);
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                if (retryTime > 0)
                {
                    retryTime--;
                    yield return new WaitForSeconds(1);
                    PostGameResult(bodyPlayGame, eventSuccess, eventFailed, retryTime);
                }
                else
                    eventFailed(www.downloadHandler.text);
            }
            else
            {
                var jsonReturn = www.downloadHandler.text;
                var modelReturn = JsonUtility.FromJson<PostGameResultResponse>(jsonReturn);
                eventSuccess(modelReturn.data);
            }
        }
    }

    public void PostUnlockCharacter(string assetId, Action eventSuccess, Action<string> eventFailed, int retryTime = 0)
    {
        StartCoroutine(RoutinePostUnlockCharacter(assetId, eventSuccess, eventFailed, retryTime));
    }
    private IEnumerator RoutinePostUnlockCharacter(string assetId, Action eventSuccess, Action<string> eventFailed, int retryTime)
    {
        var targetUrl = string.Concat(APIBASEURL, APIHelper.POST_UNLOCK_CHARACTER);
        Dictionary<string, object> body = new Dictionary<string, object>()
        {
            ["gameId"] = APIHelper.gameId,
            ["assetId"] = assetId
        };
        string jsonBody = JsonConvert.SerializeObject(body);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        using (UnityWebRequest www = new UnityWebRequest(targetUrl, "POST"))
        {
            www.timeout = requestTimeout;
            www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            www.SetRequestHeader(APIHelper.HEADER_ACCESS_TOKEN, ACCESS_TOKEN);
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                if (retryTime > 0)
                {
                    retryTime--;
                    yield return new WaitForSeconds(1);
                    PostUnlockCharacter(assetId, eventSuccess, eventFailed, retryTime);
                }
                else
                {
                    var jsonReturn = www.downloadHandler.text;
                    ErrorMessage error = JsonUtility.FromJson<ErrorMessage>(jsonReturn);
                    eventFailed.Invoke(error.message);
                }
            }
            else
            {
                var jsonReturn = www.downloadHandler.text;
                eventSuccess();
            }
        }
    }


    public void GetGameResult(Action<GetGameResultData[]> eventSuccess, Action<string> eventFailed, int retryTime = 5)
    {
        StartCoroutine(RoutineGetGameResult(eventSuccess, eventFailed, retryTime));
    }
    private IEnumerator RoutineGetGameResult(Action<GetGameResultData[]> eventSuccess, Action<string> eventFailed, int retryTime)
    {
        var targetUrl = string.Concat(APIBASEURL, APIHelper.GET_GAME_RESULT);

        //targetUrl = string.Concat(targetUrl, "?page=0&size=100");
        targetUrl += "/" + APIHelper.gameId;
        Debug.Log(targetUrl);
        using (UnityWebRequest www = UnityWebRequest.Get(targetUrl))
        {
            www.timeout = requestTimeout;
            www.SetRequestHeader(APIHelper.HEADER_ACCESS_TOKEN, ACCESS_TOKEN);
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                if (retryTime > 0)
                {
                    retryTime--;
                    yield return new WaitForSeconds(1);
                    GetGameResult(eventSuccess, eventFailed, retryTime);
                }
                else
                    eventFailed(www.downloadHandler.text);
            }
            else
            {
                var jsonReturn = www.downloadHandler.text;
                var modelReturn = JsonUtility.FromJson<GetGameResultResponse>(jsonReturn);
                eventSuccess(modelReturn.data);
            }
        }
    }

    public void PostAssetRent(string assetId, Action eventSuccess, Action<string> eventFailed, int retryTime = 0)
    {
        StartCoroutine(RoutinePostAssetLoan(assetId, eventSuccess, eventFailed, retryTime));
    }
    private IEnumerator RoutinePostAssetLoan(string assetId, Action eventSuccess, Action<string> eventFailed, int retryTime)
    {
        var targetUrl = string.Concat(APIBASEURL, APIHelper.ASSET_RENT);
        Dictionary<string, object> body = new Dictionary<string, object>()
        {
            ["gameId"] = APIHelper.gameId,
            ["assetId"] = assetId,
        };
        string jsonBody = JsonConvert.SerializeObject(body);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        using (UnityWebRequest www = new UnityWebRequest(targetUrl, "POST"))
        {
            www.timeout = requestTimeout;
            www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            www.SetRequestHeader(APIHelper.HEADER_ACCESS_TOKEN, ACCESS_TOKEN);
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                if (retryTime > 0)
                {
                    retryTime--;
                    yield return new WaitForSeconds(1);
                    PostUnlockCharacter(assetId, eventSuccess, eventFailed, retryTime);
                }
                else
                {
                    var jsonReturn = www.downloadHandler.text;
                    ErrorMessage error = JsonUtility.FromJson<ErrorMessage>(jsonReturn);
                    eventFailed.Invoke(error.message);
                }
            }
            else
            {
                var jsonReturn = www.downloadHandler.text;
                eventSuccess();
            }
        }
    }
    private string APIBASEURL
    {
        get
        {
            switch (serverType)
            {
                case ServerType.PROD:
                    return APIHelper.BASE_URL_PROD;
                case ServerType.STAGGING:
                    return APIHelper.BASE_URL_STAGGING;
                case ServerType.DEV:
                    return APIHelper.BASE_URL_DEV;
                default:
                    return APIHelper.BASE_URL_PROD;
            }
        }
    }

    private string ACCESS_TOKEN
    {
        get
        {
#if UNITY_EDITOR
            return APIHelper.ACCESS_TOKEN_TEST;
#endif


#if UNITY_WEBGL

            return tokenId;

#endif

            return APIHelper.ACCESS_TOKEN_TEST;
        }

    }



}

[System.Serializable]
public enum ServerType
{
    PROD, STAGGING, DEV
}

[System.Serializable]
public class ErrorMessage
{
    public string errorCode;
    public string message;
}