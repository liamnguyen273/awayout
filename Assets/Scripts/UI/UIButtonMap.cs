using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIButtonMap : MonoBehaviour
{
    private Button button;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI textMapName;
    [SerializeField] private GameObject iconLocked;
    private GameSettings gameSettings;
    public void Init(GameSettings gameSettings)
    {
        button = GetComponent<Button>();
        textMapName.text = gameSettings.mapName;
        image.sprite = gameSettings.mapOnSprite;
        this.gameSettings = gameSettings;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            GameManager.Instance.LoadSceneGameplay(gameSettings);
        });


        bool unlocked = GameManager.Instance.currentUserProfileForGameData.score >= gameSettings.coinsToUnlock;
        iconLocked.gameObject.SetActive(!unlocked);
        button.interactable = unlocked;
    }

    public bool isUnlocked
    {
        get
        {
            return !iconLocked.gameObject.activeSelf;
        }
    }
}
