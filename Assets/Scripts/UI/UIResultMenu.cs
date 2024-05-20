using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIResultMenu : UIBase
{
    public GameObject winGroup;
    public GameObject loseGroup;
    public Button buttonBack;
    public Button buttonBackInLoseGroup;
    public TextMeshProUGUI textResult;
    public TextMeshProUGUI textSoldier;
    public TextMeshProUGUI textComander;

    private void Awake()
    {
        buttonBack.onClick.AddListener(() =>
        {
            GameManager.Instance.LoadSceneHome();
            OnClose();
        });

        buttonBackInLoseGroup.onClick.AddListener(() =>
        {
            GameManager.Instance.LoadSceneHome();
            OnClose();
        });
    }

    public override void OnShown(object[] data)
    {
        base.OnShown(data);
        bool win = (bool)data[0];
        int coins = (int)data[1];
        buttonBackInLoseGroup.gameObject.SetActive(false);
        winGroup.SetActive(win);
        loseGroup.SetActive(!win);
        StartCoroutine(RoutineShowRewardedVideo());
        StartCoroutine(RoutineSetActiveButtonBack());

    }

    private IEnumerator RoutineShowRewardedVideo()
    {
        yield return new WaitForSeconds(1f);
        bool isShowAdsReward = GameManager.Instance.ShowRewardPopupCoins();
        if (!isShowAdsReward)
        {
            AdsManager.Instance.ShowInterAdsAndPauseGame();
        }
    }
    private IEnumerator RoutineSetActiveButtonBack()
    {
        yield return new WaitForSeconds(2f);
        buttonBackInLoseGroup.gameObject.SetActive(true);
    }
}
