using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MoreMountains.Tools;
using System.IO;
using System;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Transform menuRoot;
    [SerializeField] private Transform popupRoot;
    [SerializeField] private Transform overlayRoot;

    private Dictionary<string, UIBase> screens = new Dictionary<string, UIBase>();
    private List<string> currentScreens = new List<string>();

    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UIManager>();
                if (_instance == null)
                {
                    var path = Path.Combine(UIData.RESOURCES_PATH, "UIManager");
                    var prefab = Resources.Load<GameObject>(path);
                    if (prefab == null)
                    {
                        Debug.Log("Cannot load UIManager: " + path);
                        return null;
                    }

                    var screenObj = Instantiate(prefab.gameObject);
                    _instance = screenObj.GetComponent<UIManager>();
                }
            }
            return _instance;
        }
    }
    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(this);
    }

    public void ShowMainMenu()
    {
        Close(UIData.MainMenu);

        ShowUI(UIData.MainMenu, new object[] { });
    }

    public void ShowGameplayMenu()
    {
        Close(UIData.GameplayMenu);
        ShowUI(UIData.GameplayMenu);
    }

    public void ShowLoadingScreen()
    {
        Close(UIData.LoadingScreen);
        ShowUI(UIData.LoadingScreen);
    }


    public void HideLoadingScreen()
    {
        Close(UIData.LoadingScreen);
    }
    public void HideCampaignPopup()
    {
        Close(UIData.CampaignPopup);
    }

    public void HideMainMenu()
    {
        Close(UIData.MainMenu);
    }
    public void ShowQuickLoadingScreen()
    {
        Close(UIData.QuickLoadingScreen);
        ShowUI(UIData.QuickLoadingScreen);
    }

    public void HideQuickLoadingScreen()
    {
        Close(UIData.QuickLoadingScreen);
    }

    public void ShowChooseModeMenu()
    {
        Close(UIData.ChooseModeMenu);
        ShowUI(UIData.ChooseModeMenu);
    }

    public void ShowCampaignPopup()
    {
        Close(UIData.CampaignPopup);
        ShowUI(UIData.CampaignPopup);
    }
    public void ShowStoreMenu()
    {
        Close(UIData.StoreMenu);
        ShowUI(UIData.StoreMenu);
    }
    public void ShowInventoryPopup()
    {
        Close(UIData.InventoryPopup);
        ShowUI(UIData.InventoryPopup);
    }
    public void ShowCreditsPopup()
    {
        Close(UIData.CreditsPopup);
        ShowUI(UIData.CreditsPopup);
    }

    public void ShowSoundAndMusicPopup()
    {
        Close(UIData.SoundAndMusicPopup);
        ShowUI(UIData.SoundAndMusicPopup);
    }

    public void ShowRewardedPopup(RewardType rewardType, Action callback, bool watchingAds)
    {
        Close(UIData.RewardedPopup);
        ShowUI(UIData.RewardedPopup, new object[] { rewardType, callback, watchingAds });
    }

    public UIRewardedPopup uIRewardedPopup
    {
        get
        {
            return LoadUI(UIData.RewardedPopup) as UIRewardedPopup;
        }

    }

    public UIMainMenu uIMainMenu
    {
        get
        {
            return LoadUI(UIData.MainMenu) as UIMainMenu;
        }

    }

    public void HideGameplayMenu()
    {
        Close(UIData.GameplayMenu);
    }
    public void ShowPausePopup()
    {
        Close(UIData.PausePopup);
        ShowUI(UIData.PausePopup);
    }

    public void ShowSettingsPopup()
    {
        Close(UIData.SettingsPopup);
        ShowUI(UIData.SettingsPopup);
    }

    public void ShowNotificationPopup(string content, Action confirm, string confirmText, Action cancel, string cancelText)
    {
        Close(UIData.NotificationPopup);
        ShowUI(UIData.NotificationPopup, new object[] { content, confirm, confirmText, cancel, cancelText });
    }
    public void ShowResultMenu(bool win, int coins)
    {
        Close(UIData.ResultMenu);
        ShowUI(UIData.ResultMenu, new object[] { win, coins });
    }

    public void ShowUI(string uiName, object[] data = null)
    {
        if (currentScreens.Contains(uiName))
        {
            // Debug.Log($"{name} already show!");
            return;
        }

        var screen = LoadUI(uiName);
        if (screen == null)
        {
            Debug.Log("cannot load new screen!");
            return;
        }

        currentScreens.Add(uiName);

        screen.gameObject.SetActive(true);
        screen.OnActived(data);

        screen.OnShown(data);

    }

    public void CloseAll()
    {
        foreach (var item in currentScreens)
        {
            if (!screens.ContainsKey(item)) continue;

            var popup = screens[item];
            if (popup == null) continue;

            popup.OnHidden();
            popup.gameObject.SetActive(false);
            popup.OnDeactived();
        }

        currentScreens.Clear();
    }

    public void Close(string name, bool doAnimation = false, Action callback = null)
    {
        if (currentScreens.Count == 0) return;

        UIBase popup = null;

        if (screens.ContainsKey(name)) popup = screens[name];
        if (currentScreens.Contains(name)) currentScreens.Remove(name);

        if (popup == null) return;


        popup.OnHidden();
        popup.gameObject.SetActive(false);
        popup.OnDeactived();

        callback?.Invoke();

    }

    private UIBase LoadUI(string name)
    {
        if (screens.ContainsKey(name))
        {
            // Debug.Log("load screen: " + name);
            return screens[name];
        }

        var path = Path.Combine(UIData.RESOURCES_PATH, name);
        var prefab = Resources.Load<GameObject>(path);
        if (prefab == null)
        {
            Debug.Log("Cannot load screen: " + path);
            return null;
        }

        var screenObj = Instantiate(prefab.gameObject);
        var screen = screenObj.GetComponent<UIBase>();
        screens.Add(name, screen);
        switch (screen.uiType)
        {
            case UIType.Menu:
                screenObj.transform.SetParent(menuRoot, false);
                break;
            case UIType.Popup:
                screenObj.transform.SetParent(popupRoot, false);
                break;
            case UIType.Overlay:
                screenObj.transform.SetParent(overlayRoot, false);
                break;
        }
        screenObj.name = name;
        return screen;
    }

    public UIGameplayMenu uIGameplayMenu
    {
        get
        {
            return LoadUI(UIData.GameplayMenu) as UIGameplayMenu;
        }

    }

    public UIInventoryPopup uIInventoryPopup
    {
        get
        {
            return LoadUI(UIData.InventoryPopup) as UIInventoryPopup;
        }

    }

    public UILoadingScreen uILoadingScreen
    {
        get
        {
            return LoadUI(UIData.LoadingScreen) as UILoadingScreen;
        }

    }

    public void ShowStarsPopup(int stars)
    {
        Close(UIData.StarsPopup);
        ShowUI(UIData.StarsPopup, new object[] { stars });
    }
}


[Serializable]
public enum UIType
{
    Menu, Popup, Overlay
}