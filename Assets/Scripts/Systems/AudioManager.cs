using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using DG.Tweening;
using UnityEngine.Audio;
public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioClip[] mainMusics;
    public bool sfxIsOn = true;
    public bool musicIsOn = true;
    private AudioSource audioSource;
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
        audioSource = GetComponent<AudioSource>();
        StopMainMusic(true);

        musicIsOn = PlayerPrefs.GetInt("musicIsOn", 1) == 1;
        sfxIsOn = PlayerPrefs.GetInt("sfxIsOn", 1) == 1;


    }
    private void Start()
    {
        SetMusic(musicIsOn);
        SetSFX(sfxIsOn);

    }
    public void SetSFX(bool isOn)
    {
        sfxIsOn = isOn;
        PlayerPrefs.SetInt("sfxIsOn", isOn ? 1 : 0);
        float volume = 0;
        if (!isOn) volume = -80;
        mixer.SetFloat("Sfx", volume);


    }

    public void SetMusic(bool isOn)
    {
        musicIsOn = isOn;
        PlayerPrefs.SetInt("musicIsOn", isOn ? 1 : 0);
        float volume = 0;
        if (!isOn) volume = -80;
        mixer.SetFloat("Music", volume);
    }

    public void StopMainMusic(bool force = false)
    {
        audioSource.DOKill();
        if (force) audioSource.volume = 0;
        else audioSource.DOFade(0, 2f);
    }

    public void PlaypMainMusic()
    {
        audioSource.clip = mainMusics[Random.Range(0, mainMusics.Length)];
        audioSource.Play();
        audioSource.DOKill();
        audioSource.DOFade(0.5f, 2f);
    }

}
