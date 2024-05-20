using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
public class UIMainMenu : UIBase
{
    [Header("Start Group")]
    [SerializeField] private GameObject startGroup;
    [SerializeField] private Button buttonStart;

    [Header("Main Group")]
    [SerializeField] private GameObject mainGroup;
    [SerializeField] private TextMeshProUGUI textCoins;
    [SerializeField] private Button buttonCampaign;
    [SerializeField] private Button buttonInventory;
    [SerializeField] private Button buttonCredits;
    [SerializeField] private Button buttonSettings;
    [SerializeField] private TMP_InputField userName;


    private void Awake()
    {
        buttonStart.onClick.AddListener(OnClickButtonStart);
        buttonCampaign.onClick.AddListener(OnClickButtonCampaign);
        buttonInventory.onClick.AddListener(OnClickButtonInventory);
        buttonCredits.onClick.AddListener(OnClickButtonCredits);
        buttonSettings.onClick.AddListener(() => { UIManager.Instance.ShowSettingsPopup(); });
        userName.interactable = !GameManager.Instance.generalSettings.useAPI;
        userName.onSubmit.AddListener((text) =>
        {
            GameManager.Instance.currentUserProfileData.name = text;
            PlayerPrefs.SetString("Player Name", GameManager.Instance.currentUserProfileData.name);
        });

    }
    public override void OnShown(object[] data)
    {
        base.OnShown(data);
        startGroup.SetActive(true);
        mainGroup.SetActive(false);
        AudioManager.Instance.PlaypMainMusic();
        RefreshScreen();
    }

    public void RefreshScreen()
    {
        textCoins.text = GameManager.Instance.currentUserProfileForGameData.score.ToString();
        userName.text = GameManager.Instance.currentUserProfileData.name;
    }

    private void OnClickButtonStart()
    {
        startGroup.SetActive(false);
        mainGroup.SetActive(true);
    }

    private void OnClickButtonCampaign()
    {
        UIManager.Instance.ShowCampaignPopup();
    }

    private void OnClickButtonInventory()
    {
        UIManager.Instance.ShowInventoryPopup();
    }

    private void OnClickButtonCredits()
    {
        UIManager.Instance.ShowCreditsPopup();
    }
}
