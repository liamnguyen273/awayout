using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIStarsPopup : UIBase
{
    public GameObject[] imageStars;
    public Button buttonClose;
    public TextMeshProUGUI textTitle;

    private void Awake()
    {
        buttonClose.onClick.AddListener(() => { OnClose(); });
    }

    public override void OnShown(object[] data)
    {
        int stars = (int)data[0];

        for (int i = 0; i < imageStars.Length; i++)
        {
            imageStars[i].gameObject.SetActive(i < stars);
        }
        if (stars > 0)
            textTitle.text = "Congratulations!";
        else
            textTitle.text = "Mission Failed";
        base.OnShown(data);

    }
}
