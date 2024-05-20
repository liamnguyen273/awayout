using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
public class UIRewardedPopup : UIBase
{
    public GameObject groupCoins;
    public Button buttonClaimCoins;
    public Button buttonClaimAdsCoins;
    public Button buttonCancelCoins;


    public GameObject groupAmmo;
    public Button buttonClaimAmmo;
    public Button buttonClaimAdsAmmo;
    public Button buttonCancelAmmo;
    public Button buttonRetry;

    private Action callbackClaim;

    private void Awake()
    {
        buttonCancelCoins.onClick.AddListener(() => { OnClose(); });
        buttonCancelAmmo.onClick.AddListener(() => { OnClose(); });
        buttonClaimAdsAmmo.onClick.AddListener(OnClickButtonAdsClaim);
        buttonClaimAdsCoins.onClick.AddListener(OnClickButtonClaim);
        buttonRetry.onClick.AddListener(OnClickButtonRetry);
    }
    public override void OnShown(object[] data)
    {
        RewardType rewardType = (RewardType)data[0];
        callbackClaim = (Action)data[1];
        bool watchingAds = (bool)data[2];

        groupCoins.gameObject.SetActive(rewardType == RewardType.Coins);
        groupAmmo.gameObject.SetActive(rewardType == RewardType.Ammo);


        buttonClaimCoins.gameObject.SetActive(!watchingAds);
        buttonClaimAdsCoins.gameObject.SetActive(watchingAds);
        buttonClaimAmmo.gameObject.SetActive(!watchingAds);
        buttonClaimAdsAmmo.gameObject.SetActive(watchingAds);
        base.OnShown(data);
    }

    private void OnClickButtonClaim()
    {
        AdsManager.Instance.ShowAdsCoins(GameManager.Instance.generalSettings.rewardedCoins.ToString(),
            ShowAdsSuccess, ShowAdsFailed);
        OnClose();
    }

    private void OnClickButtonRetry()
    {
        GameController.Instance.EndGame();
        OnClose();
    }

    private void OnClickButtonAdsClaim()
    {
        AdsManager.Instance.ShowAds(
            ShowAdsSuccess, ShowAdsFailed);
        OnClose();

    }

    public void ShowAdsSuccess()
    {
        Debug.Log("ShowAdsSuccess");
        callbackClaim?.Invoke();
        OnClose();
    }

    public void ShowAdsFailed()
    {
        UIManager.Instance.ShowNotificationPopup("Failed to show rewarded video. Please try again later.", () => { }, "OK", null, "");
    }

}



[System.Serializable]
public enum RewardType
{
    Coins, Ammo
}