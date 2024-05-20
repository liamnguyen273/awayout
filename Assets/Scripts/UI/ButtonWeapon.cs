using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ButtonWeapon : MonoBehaviour
{
    [SerializeField] private GameObject tick;
    [SerializeField] private Image card;
    [SerializeField] private Image imageWeapon;
    [SerializeField] private TextMeshProUGUI textWeaponName;
    [SerializeField] private Sprite[] cards;
    [SerializeField] private GameObject[] stars;
    private WeaponData weaponData;
    private Button button;
    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClickButton);
    }
    public void Init(WeaponData weaponData, string currentItemId)
    {
        this.weaponData = weaponData;
        tick.gameObject.SetActive(weaponData.itemId == currentItemId);
        card.sprite = cards[(int)weaponData.weaponRarity];
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].gameObject.SetActive(i <= (int)weaponData.weaponRarity);
        }
        imageWeapon.sprite = weaponData.itemImage;
        textWeaponName.text = weaponData.itemName;
    }

    private void OnClickButton()
    {
        GameManager.Instance.SetCurrentWeapon(weaponData, () =>
        {
            UIManager.Instance.uIInventoryPopup.RefeshUI();
        });
    }
}
