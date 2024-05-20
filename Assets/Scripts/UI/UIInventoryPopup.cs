using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIInventoryPopup : UIBase
{
    [SerializeField] private GameObject prefabButtonWeapon;
    [SerializeField] private GameObject prefabButtonSoldier;
    [SerializeField] private Transform contentWeapons;
    [SerializeField] private Transform contentSoldier;
    [SerializeField] private Button buttonBack;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Transform statsContent;
    [SerializeField] private TextMeshProUGUI textDescription;
    [SerializeField] private Toggle toggleWeapon;
    [SerializeField] private Toggle toggleSoldier;
    private List<ButtonWeapon> buttonWeapons = new List<ButtonWeapon>();
    private List<ButtonSoldier> buttonSoldiers = new List<ButtonSoldier>();
    private void Awake()
    {
        buttonBack.onClick.AddListener(() => { OnClose(); });


    }
    public override void OnShown(object[] data)
    {
        base.OnShown(data);
        RefeshUI();
    }
    private void Update()
    {
        playerController.transform.Rotate(Vector3.up * 100 * Time.deltaTime);
    }

    public void RefeshUI()
    {
        DisableButtons();
        WeaponData[] weaponData = GameManager.Instance.itemDatabase.weaponDatas;

        for (int i = 0; i < weaponData.Length; i++)
        {
            if (!weaponData[i].isOwner) continue;
            ButtonWeapon buttonWeapon = PoolingManager.Instance.GetGameObject<ButtonWeapon>(prefabButtonWeapon, contentWeapons);
            buttonWeapon.gameObject.SetActive(true);
            buttonWeapon.Init(weaponData[i], GameManager.Instance.itemDatabase.CurrentWeapon.itemId);
            buttonWeapons.Add(buttonWeapon);
            buttonWeapon.transform.SetAsLastSibling();
        }

        SoldierProperties[] characterProperties = GameManager.Instance.itemDatabase.characterProperties;
        for (int i = 0; i < characterProperties.Length; i++)
        {
            if (!characterProperties[i].isOwner) continue;
            ButtonSoldier buttonSoldier = PoolingManager.Instance.GetGameObject<ButtonSoldier>(prefabButtonSoldier, contentSoldier);
            buttonSoldier.gameObject.SetActive(true);
            buttonSoldier.Init(characterProperties[i], GameManager.Instance.itemDatabase.CurrentSoldier.assetId);
            buttonSoldiers.Add(buttonSoldier);
            buttonSoldier.transform.SetAsLastSibling();
        }

        if (toggleSoldier.isOn)
        {
            textDescription.text = GameManager.Instance.itemDatabase.CurrentSoldier.description;
        }

        if (toggleWeapon.isOn)
        {
            textDescription.text = GameManager.Instance.itemDatabase.CurrentWeapon.description;
        }
        playerController.InitPlayer(GameManager.Instance.itemDatabase.CurrentWeapon, GameManager.Instance.itemDatabase.CurrentSoldier);
    }
    private void DisableButtons()
    {
        foreach (ButtonWeapon buttonWeapon in buttonWeapons)
        {
            buttonWeapon.gameObject.SetActive(false);
        }
        buttonWeapons = new List<ButtonWeapon>();

        foreach (ButtonSoldier buttonSoldier in buttonSoldiers)
        {
            buttonSoldier.gameObject.SetActive(false);
        }
        buttonSoldiers = new List<ButtonSoldier>();
    }
}
