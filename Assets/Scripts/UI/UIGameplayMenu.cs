using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UIGameplayMenu : UIBase
{
    public Image reloadCircle;
    public TextMeshProUGUI textTime;
    public GameObject bossFightgroup;
    public Image topBoss;
    public Image bottomBoss;
    public TextMeshProUGUI textBoss;

    public GameObject playerGroup;

    public Slider sliderHealth;
    public TextMeshProUGUI textHealth;
    public Slider sliderAmmo;
    public TextMeshProUGUI textAmmo;
    public Image avatarImage;
    public TextMeshProUGUI textUserName;
    public GameObject textOutOfAmmo;

    public override void OnShown(object[] data)
    {
        base.OnShown(data);
        reloadCircle.gameObject.SetActive(false);
        InputManager.Instance.HideUI();
        playerGroup.gameObject.SetActive(false);
        textOutOfAmmo.SetActive(false);
        textUserName.text = GameManager.Instance.currentUserProfileData.name;
        HideBossFight();
    }

    public void ShowPlayerStats()
    {
        playerGroup.gameObject.SetActive(true);
    }
    public void StartReload(float duration)
    {
        reloadCircle.DOKill();
        reloadCircle.gameObject.SetActive(true);
        reloadCircle.fillAmount = 0;
        reloadCircle.DOFillAmount(1, duration).OnComplete(() =>
        {
            reloadCircle.gameObject.SetActive(false);
        }).SetEase(Ease.Linear);
    }


    public void ShowBossFight()
    {
        bossFightgroup.gameObject.SetActive(true);
        topBoss.color = new Color(topBoss.color.r, topBoss.color.g, topBoss.color.b, 0);
        bottomBoss.color = new Color(bottomBoss.color.r, bottomBoss.color.g, bottomBoss.color.b, 0);
        textBoss.color = new Color(textBoss.color.r, textBoss.color.g, textBoss.color.b, 0);
        float fadeInDuration = 0.5f;
        textBoss.DOFade(1, fadeInDuration);
        bottomBoss.DOFade(1, fadeInDuration);
        topBoss.DOFade(1, fadeInDuration);
    }
    public void HideBossFight()
    {
        bossFightgroup.gameObject.SetActive(false);
    }
    public void SetTimeLeft(float time)
    {
        System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(time);
        string timeText = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        textTime.text = timeText;
    }

    public void SetHealth(float fillPercentage, float value, float maxValue)
    {
        sliderHealth.DOKill();
        textHealth.text = value.ToString() + "/" + maxValue.ToString();
        if (sliderHealth.value > fillPercentage)
        {
            sliderHealth.DOValue(fillPercentage, 1f);
        }
        else
        {
            sliderHealth.value = fillPercentage;
        }
    }

    public void SetAmmo(float value, float maxValue)
    {
        sliderAmmo.DOKill();
        textOutOfAmmo.gameObject.SetActive(value <= 0);
        textAmmo.text = value.ToString() + "/" + maxValue.ToString();
        float fillPercentage = value / maxValue;
        if (sliderAmmo.value > fillPercentage)
        {
            sliderAmmo.DOValue(fillPercentage, 1f);
        }
        else
        {
            sliderAmmo.value = fillPercentage;
        }
    }


}
