using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSoldier : MonoBehaviour
{
    [SerializeField] private GameObject tick;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI textSoldierName;
    private Button button;
    private SoldierProperties soldierProperties;
    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClickButton);
    }
    public void Init(SoldierProperties soldierProperties, string currentId)
    {
        this.soldierProperties = soldierProperties;
        tick.gameObject.SetActive(soldierProperties.assetId == currentId);
        image.sprite = (soldierProperties).characterImage;
        textSoldierName.text = (soldierProperties).characterName;
    }

    private void OnClickButton()
    {
        GameManager.Instance.SetCurrentSoldier(soldierProperties, () =>
        {
            UIManager.Instance.uIInventoryPopup.RefeshUI();
        });

    }


}
