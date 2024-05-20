using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using System;
using System.Runtime.InteropServices;

public class AdsManager : Singleton<AdsManager>
{
#if UNITY_WEBGL
    [DllImport("__Internal")]
    public static extern void ShowRewardedAds();


    [DllImport("__Internal")]
    public static extern void ShowRewardedAdsCoins(string coins);

    [DllImport("__Internal")]
    public static extern void ShowInterAds();
#endif
    private Action showAdsSuccessfulCallback;
    private Action showAdsFailedCallBack;
    public override void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(this);
        base.Awake();
    }

    public void ShowAds(Action successful, Action failed)
    {
        Time.timeScale = 0;
        showAdsSuccessfulCallback = successful;
        showAdsFailedCallBack = failed;

#if UNITY_WEBGL
        Debug.Log("ShowRewardedAds");
        ShowRewardedAds();
#else
        OnShowAdsSuccess();
#endif
    }

    public void ShowAdsCoins(string coins, Action successful, Action failed)
    {
        Time.timeScale = 0;
        showAdsSuccessfulCallback = successful;
        showAdsFailedCallBack = failed;

#if UNITY_WEBGL
        Debug.Log("ShowRewardedAdsCoins");
        ShowRewardedAdsCoins(coins);
#else
        OnShowAdsSuccess();
#endif
    }

    public void ShowInterAdsAndPauseGame()
    {
        Time.timeScale = 0;
        showAdsSuccessfulCallback = null;
        showAdsFailedCallBack = null;
#if UNITY_WEBGL
        ShowInterAds();
#endif
    }

    public void OnShowAdsSuccess()
    {
        Time.timeScale = 1;
        Debug.Log("Unity OnShowAdsSuccess");
        showAdsSuccessfulCallback?.Invoke();
        ClearEvents();
    }

    public void OnShowAdsFailed()
    {
        Time.timeScale = 1;
        Debug.Log("Unity OnShowAdsFailed");
        showAdsFailedCallBack?.Invoke();
        ClearEvents();
    }

    private void ClearEvents()
    {
        showAdsSuccessfulCallback = null;
        showAdsFailedCallBack = null;
    }
}
