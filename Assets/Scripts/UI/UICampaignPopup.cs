using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UICampaignPopup : UIBase
{
    [SerializeField] private Transform mapContent;
    [SerializeField] private GameObject prefabButtonMap;
    [SerializeField] private Button buttonBack;
    private void Awake()
    {
        buttonBack.onClick.AddListener(() => { OnClose(); });
    }

    public override void OnShown(object[] data)
    {
        GameSettings[] gameSettings = GameManager.Instance.gameSettings;
        base.OnShown(data);
        DisableButton();
        bool newMap = false;
        string mapUnlocked = PlayerPrefs.GetString("MapUnlocked", "");

        for (int i = 0; i < gameSettings.Length; i++)
        {
            UIButtonMap button = PoolingManager.Instance.GetGameObject<UIButtonMap>(prefabButtonMap, mapContent);
            button.gameObject.SetActive(true);
            button.Init(gameSettings[i]);
            if (button.isUnlocked && !mapUnlocked.Contains(gameSettings[i].mapName))
            {
                if (gameSettings[i].coinsToUnlock > 1)
                    newMap = true;
                mapUnlocked += gameSettings[i].mapName;
            }
            button.transform.SetAsLastSibling();
        }

        PlayerPrefs.SetString("MapUnlocked", mapUnlocked);
        if (newMap)
        {
            UIManager.Instance.ShowNotificationPopup("Congratulations. You have unlocked a new map", () => { }, "OK", null, "");
        }
    }

    private void DisableButton()
    {
        PoolingManager.Instance.DisablePool(prefabButtonMap);
    }

}
