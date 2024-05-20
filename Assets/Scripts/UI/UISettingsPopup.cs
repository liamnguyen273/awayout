using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISettingsPopup : UIBase
{
    public Toggle toggleMusic;
    public Toggle toggleSFX;
    public Button buttonClose;
    public Button buttonQuitGame;

    public void Awake()
    {
        buttonClose.onClick.AddListener(() => { OnClose(); });
        buttonQuitGame.onClick.AddListener(() => { GameManager.Instance.RequestQuitGame(); });
    }

    public override void OnShown(object[] data)
    {
        base.OnShown(data);
        toggleMusic.onValueChanged.RemoveAllListeners();
        toggleSFX.onValueChanged.RemoveAllListeners();

        toggleMusic.isOn = AudioManager.Instance.musicIsOn;
        toggleSFX.isOn = AudioManager.Instance.sfxIsOn;

        toggleMusic.onValueChanged.AddListener((value) => { AudioManager.Instance.SetMusic(value); });
        toggleSFX.onValueChanged.AddListener((value) => { AudioManager.Instance.SetSFX(value); });
    }
}
