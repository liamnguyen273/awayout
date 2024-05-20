using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.SceneManagement;
using System;
using Random = UnityEngine.Random;
using System.Runtime.InteropServices;

public class GameManager : Singleton<GameManager>
{
#if UNITY_WEBGL
    [DllImport("__Internal")]
    public static extern void CloseApp();
#endif
    public GetUserProfileData currentUserProfileData;
    public GetUserProfileForGameData currentUserProfileForGameData;
    public GameSettings[] gameSettings;
    public ItemDatabase itemDatabase;
    public GeneralSettings generalSettings;
    public UserAssetData[] userAssetDatas;
    public GetGameResultData[] gameResult;
    public GameDetailsData gameDetails;
    private string cheatQueue = "";
    public static bool autoplay = false;
    public static bool godmod = false;
    private bool canRequestAmmoReward = true;
    public PlayGameData playGameData { get; set; }

    private const string demoWeapon = "handgun_L1";
    private const string demoSoldier = "liam";
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

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        ProcessCheatCode();
    }

    private void ProcessCheatCode()
    {
        foreach (char c in Input.inputString)
        {
            cheatQueue += c;
        }
        if (cheatQueue.Length > 1000)
            cheatQueue = "";
        if (cheatQueue.ToLower().Contains("autoplay"))
        {
            cheatQueue = "";
            autoplay = !autoplay;
            Debug.LogError("autoplay: " + autoplay);
        }

        if (cheatQueue.ToLower().Contains("godmod"))
        {
            cheatQueue = "";
            godmod = !godmod;
            Debug.LogError("godmod: " + godmod);
        }

        if (cheatQueue.ToLower().Contains("showreward"))
        {
            cheatQueue = "";
            AdsManager.Instance.ShowAds(() => { }, () => { });
        }

        if (cheatQueue.ToLower().Contains("showinter"))
        {
            cheatQueue = "";
            AdsManager.Instance.ShowInterAdsAndPauseGame();
        }

        if (cheatQueue.ToLower().Contains("s+"))
        {
            cheatQueue = "";
            Time.timeScale += 1;
        }

        if (cheatQueue.ToLower().Contains("s-"))
        {
            cheatQueue = "";
            Time.timeScale -= 1;
        }

        if (cheatQueue.ToLower().Contains("showads"))
        {
            cheatQueue = "";
            UIManager.Instance.ShowRewardedPopup(RewardType.Coins, AddRewardedCoins, true);
        }
    }
    private void Init()
    {
        StartCoroutine(RoutineInitGame());
    }

    private IEnumerator RoutineInitGame()
    {
        SetResolution();
        yield return new WaitForSeconds(0.1f);
        UIManager.Instance.ShowLoadingScreen();
        UIManager.Instance.uILoadingScreen.SetLoadingProgress(0, 0, "", null);
        GetUserProfile();

    }

    #region Step 1
    private void GetUserProfile()
    {
        UIManager.Instance.uILoadingScreen.SetLoadingProgress(0.1f, 0.5f, "", null);

        currentUserProfileData = new GetUserProfileData() { id = "123", name = "ahihi", point = 1 };
        GetUserProfileForGame();
        return;

        GameServices.Instance.GetUserProfile(
             (data) =>
             {
                 currentUserProfileData = data;
                 GetUserProfileForGame();
             },
             (err) =>
             {
                 UIManager.Instance.HideLoadingScreen();
                 UIManager.Instance.ShowNotificationPopup("Failed to start game! Please try again later!", () => { Init(); }, "OK", null, null);
                 Debug.LogError(err);

             });
    }
    #endregion

    #region Step 2
    private void GetUserProfileForGame()
    {
        UIManager.Instance.uILoadingScreen.SetLoadingProgress(0.2f, 0.5f, "", null);

        currentUserProfileForGameData = new GetUserProfileForGameData() { id = "123", currentAssetIds = new string[] { "assault_rifle_L5", "liam" }, playTurn = 100 };
        GetUserAsset();
        return;

        GameServices.Instance.GetUserProfileForGame(
           (data) =>
           {
               currentUserProfileForGameData = data;
               GetUserAsset();
           },
           (err) =>
           {
               UIManager.Instance.HideLoadingScreen();
               UIManager.Instance.ShowNotificationPopup("Failed to start game! Please try again later!", () => { Init(); }, "OK", null, null);
               Debug.LogError(err);
           });
    }
    #endregion



    #region Step 3
    private void GetUserAsset()
    {
        UIManager.Instance.uILoadingScreen.SetLoadingProgress(0.3f, 0.5f, "", null);

        userAssetDatas = new UserAssetData[] {
            new UserAssetData() {assetId = "assault_rifle_L5", status=true },
            new UserAssetData() {assetId = "liam", status=true } };
        CheckAssetIdsNull(); //Go to next step
        return;

        GameServices.Instance.GetUserAsset(
           (data) =>
           {
               userAssetDatas = data;
               CheckAssetIdsNull(); //Go to next step
           },
           (err) =>
           {
               UIManager.Instance.HideLoadingScreen();
               UIManager.Instance.ShowNotificationPopup("Failed to start game! Please try again later!", () => { Init(); }, "OK", null, null);
               Debug.LogError(err);
           });

    }
    #endregion


    #region Step 4
    public void CheckAssetIdsNull()
    {
        UpdatateItemDatabase(userAssetDatas);

        if (itemDatabase.CurrentSoldier != null && itemDatabase.CurrentWeapon != null)
        {
            GetGameResult(); //Go to next step
            return;
        }

        itemDatabase.SetDefaultAssetIds();

        if (itemDatabase.CurrentSoldier == null)
        {
            if (currentUserProfileData.point > 0)
            {
                string mes = "You don't have any characters right now. Would you like to hire (or buy) a character to save the world?";
                UIManager.Instance.ShowNotificationPopup(mes, RentCharacter, "YES", Init, "NO");
            }
            else
            {
                InitGameFailed("Failed to start the game. Please go to the marketplace to hire or buy a character and a weapon to play.");
            }
        }
        else if (itemDatabase.CurrentWeapon == null)
        {
            if (currentUserProfileData.point > 0)
            {
                string mes = "You cannot go to the war without weapons. Please go to rent (or buy) one to go to the war.";
                UIManager.Instance.ShowNotificationPopup(mes, RentWeapon, "YES", Init, "NO");
            }
            else
            {
                InitGameFailed("Failed to start the game. Please go to the marketplace to hire or buy a character and a weapon to play.");
            }
        }
        else
            UpdateCurrentAssetId(GetGameResult);

    }

    private void UpdatateItemDatabase(UserAssetData[] data)
    {
        for (int j = 0; j < itemDatabase.characterProperties.Length; j++)
        {
            itemDatabase.characterProperties[j].isOwner = false;
        }

        for (int j = 0; j < itemDatabase.weaponDatas.Length; j++)
        {
            itemDatabase.weaponDatas[j].isOwner = false;
        }


        for (int i = 0; i < data.Length; i++)
        {
            for (int j = 0; j < itemDatabase.characterProperties.Length; j++)
            {
                if (itemDatabase.characterProperties[j].assetId == data[i].assetId)
                {
                    itemDatabase.characterProperties[j].isOwner = (data[i].status == true || data[i].rentStatus);
                    goto endloop;
                }

            }

            for (int j = 0; j < itemDatabase.weaponDatas.Length; j++)
            {
                if (itemDatabase.weaponDatas[j].itemId == data[i].assetId)
                {
                    itemDatabase.weaponDatas[j].isOwner = (data[i].status == true || data[i].rentStatus);
                    goto endloop;
                }
            }

        endloop:
            continue;
        }

        itemDatabase.UpdateCurrentAssetIds(currentUserProfileForGameData.currentAssetIds);

    }

    private void RentCharacter()
    {
        GameServices.Instance.PostAssetRent(demoSoldier, () =>
        {
            GetUserAsset(); //Back to step 3
        }, (err) =>
        {
            InitGameFailed("Failed to start game! Please try again later!");
            Debug.LogError(err);
        });
    }

    private void RentWeapon()
    {
        GameServices.Instance.PostAssetRent(demoWeapon, () =>
        {
            GetUserAsset(); //Back to step 3
        }, (err) =>
        {
            InitGameFailed("Failed to start game! Please try again later!");
            Debug.LogError(err);
        });
    }

    #endregion

    #region Step 5
    private void GetGameResult()
    {
        UIManager.Instance.uILoadingScreen.SetLoadingProgress(0.4f, 0.5f, "", null);

        gameResult = new GetGameResultData[] { };
        GetGameDetails();
        return;

        GameServices.Instance.GetGameResult(
        (data) =>
        {
            gameResult = data;
            GetGameDetails();
        },
        (err) =>
        {
            UIManager.Instance.HideLoadingScreen();
            UIManager.Instance.ShowNotificationPopup("Failed to start game! Please try again later!", () => { Init(); }, "OK", null, null);
            Debug.LogError(err);
        });

    }
    #endregion

    #region Step 6
    private void GetGameDetails()
    {
        UIManager.Instance.uILoadingScreen.SetLoadingProgress(0.5f, 0.5f, "", null);


        gameDetails = new GameDetailsData();
        FinalStep();
        return;


        GameServices.Instance.GetGameDetails(
         (data) =>
         {
             gameDetails = data;
             FinalStep();
         },
         (err) =>
         {
             UIManager.Instance.HideLoadingScreen();
             UIManager.Instance.ShowNotificationPopup("Failed to start game! Please try again later!", () => { Init(); }, "OK", null, null);
             Debug.LogError(err);
         });

    }
    #endregion

    #region FinalStep
    private void FinalStep()
    {
        UIManager.Instance.uILoadingScreen.SetLoadingProgress(100, 1, "Init Game Completed...", () =>
        {
            UIManager.Instance.HideLoadingScreen();
            UIManager.Instance.ShowMainMenu();
        });
    }
    #endregion

    public void UpdateCurrentAssetId(Action callback)
    {
        List<string> newAssetIds = new List<string>();
        newAssetIds.Add(itemDatabase.CurrentSoldier.assetId);
        newAssetIds.Add(itemDatabase.CurrentWeapon.itemId);
        GameServices.Instance.PutUpdateCurrentAsset(newAssetIds.ToArray(), () =>
                {
                    GameServices.Instance.GetUserProfileForGame(
                    (data) =>
                    {
                        currentUserProfileForGameData = data;
                        callback.Invoke();
                    },
                    (err) =>
                    {
                        UIManager.Instance.HideLoadingScreen();
                        UIManager.Instance.ShowNotificationPopup("Failed to start game! Please try again later!", () => { StartCoroutine(RoutineInitGame()); }, "OK", null, null);
                        Debug.LogError(err);
                    });
                }, (err) =>
                {
                    callback.Invoke();
                });
    }

    public void LoadSceneGameplay(GameSettings gameSettings)
    {
        UIManager.Instance.ShowQuickLoadingScreen();

        UIManager.Instance.HideQuickLoadingScreen();
        playGameData = new PlayGameData();
        UIManager.Instance.HideCampaignPopup();
        StartCoroutine(RoutineLoadSceneGameplay(gameSettings));
        return;

        GameServices.Instance.PostPlayGame(currentUserProfileForGameData.currentAssetIds,
            (data) =>
            {
                UIManager.Instance.HideQuickLoadingScreen();
                playGameData = data;
                UIManager.Instance.HideCampaignPopup();
                StartCoroutine(RoutineLoadSceneGameplay(gameSettings));
            },
            (err) =>
            {
                string errorMessage = "Failed to start game!\nPlease Try again later!";
                if (err.errorCode.Contains("PLAY_TURN_NOT_ENOUGH"))
                {
                    errorMessage = err.message;
                }
                UIManager.Instance.HideQuickLoadingScreen();
                UIManager.Instance.ShowNotificationPopup(errorMessage, () => { }, "OK", null, "");
            });
    }

    private IEnumerator RoutineLoadSceneGameplay(GameSettings gameSettings)
    {
        UIManager.Instance.HideMainMenu();
        UIManager.Instance.ShowLoadingScreen();
        UIManager.Instance.uILoadingScreen.SetLoadingProgress(0, 0, "", null);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Gameplay", LoadSceneMode.Additive);
        UIManager.Instance.uILoadingScreen.SetLoadingProgress(50, 5, "", null);
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        asyncLoad = SceneManager.LoadSceneAsync(gameSettings.map.ToString(), LoadSceneMode.Additive);
        UIManager.Instance.uILoadingScreen.SetLoadingProgress(80, 5, "", null);
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(gameSettings.map.ToString()));
        UIManager.Instance.uILoadingScreen.SetLoadingProgress(100, 1, "", () =>
        {
            UIManager.Instance.HideLoadingScreen();
            canRequestAmmoReward = true;
            GameController.Instance.StartGame(gameSettings);
        });
    }

    public void LoadSceneHome()
    {
        UIManager.Instance.HideGameplayMenu();
        UIManager.Instance.ShowLoadingScreen();
        UIManager.Instance.uILoadingScreen.SetLoadingProgress(0, 0, "", null);
        UIManager.Instance.uILoadingScreen.SetLoadingProgress(50, 1, "", null);
        if (generalSettings.useAPI)
        {
            GameServices.Instance.GetUserProfileForGame(
           (data) =>
           {
               currentUserProfileForGameData = data;
               StartCoroutine(GoToScene("Home", () =>
               {
                   UIManager.Instance.uILoadingScreen.SetLoadingProgress(100, 1, "", () =>
                   {
                       UIManager.Instance.HideLoadingScreen();
                       UIManager.Instance.ShowMainMenu();
                       UIManager.Instance.ShowCampaignPopup();
                   });
               }));
           },
           (err) =>
           {
               UIManager.Instance.HideLoadingScreen();
               UIManager.Instance.ShowNotificationPopup("Failed to start game! Please try again later!", () => { Init(); }, "OK", null, null);
               Debug.LogError(err);
           });
        }


    }

    private void InitGameFailed(string mes)
    {
        UIManager.Instance.HideLoadingScreen();
        UIManager.Instance.ShowNotificationPopup(mes, () => { Init(); }, "OK", null, null);
    }

    private IEnumerator GoToScene(string sceneName, System.Action callback)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        callback?.Invoke();
    }
    private void SetResolution()
    {
#if UNITY_ANDROID

        int height = 640;
        int width = 1136;
        float baseRatio = 16.0f / 9;
        float ratio = Screen.height * 1.0f / Screen.width;

        float ram = SystemInfo.systemMemorySize;
        if (Mathf.Abs(baseRatio - ratio) > 0.02f)
        {
            width = (int)(height / ratio);
            if (Mathf.Abs(width - 1280) < 2)
            {
                width = 1280;
            }
            else if (Mathf.Abs(width - 853) < 2)
            {
                width = 853;
            }
            else if (Mathf.Abs(width - 960) < 2)
            {
                width = 960;
            }
        }
        Screen.SetResolution(width, height, true);
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
#elif UNITY_WEBGL
        Application.targetFrameRate = -1;
#endif

    }

    public bool ShowRewardPopupCoins()
    {
        if (!generalSettings.useAPI)
            return false;
        bool showAdsReward = Random.Range(0, 1f) <= generalSettings.showCoinsRewardedAdsRatio;
        if (showAdsReward)
        {
            UIManager.Instance.ShowRewardedPopup(RewardType.Coins, AddRewardedCoins, true);
        }
        return showAdsReward;
    }

    private void AddRewardedCoins()
    {

    }







    public void ShowRewardPopupAmmo()
    {
        if (!canRequestAmmoReward) return;
        canRequestAmmoReward = false;
        GameController.Instance.PauseGame();
        UIManager.Instance.ShowRewardedPopup(RewardType.Ammo, AddAmmo, true);
    }

    private void AddAmmo()
    {
        canRequestAmmoReward = true;
        GameController.Instance.ResumeGame();
        GameController.Instance.mainPlayerController.characterHandleWeapon.mainWeapon.AddAmmo(generalSettings.rewardedAmmo);
    }

    public void SetCurrentSoldier(SoldierProperties soldierProperties, Action callback)
    {
        UIManager.Instance.ShowQuickLoadingScreen();
        itemDatabase.CurrentSoldier = soldierProperties;
        UpdateCurrentAssetId(() =>
        {
            UIManager.Instance.HideQuickLoadingScreen();
            callback?.Invoke();
        });

    }

    public void SetCurrentWeapon(WeaponData weaponData, Action callback)
    {
        UIManager.Instance.ShowQuickLoadingScreen();
        itemDatabase.CurrentWeapon = weaponData;
        UpdateCurrentAssetId(() =>
        {
            UIManager.Instance.HideQuickLoadingScreen();
            callback?.Invoke();
        });

    }

    public void RequestQuitGame()
    {
        UIManager.Instance.ShowNotificationPopup("Are you sure you want to quit?",
            () => { QuitGame(); },
            "YES",
            () => { }, "NO");
    }

    public void QuitGame()
    {
#if UNITY_WEBGL
        CloseApp();
#else
        Application.Quit();
#endif
    }
}
