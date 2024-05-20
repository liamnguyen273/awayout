using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UINotificationPopup : UIBase
{
    public TextMeshProUGUI textContent;
    public Button buttonConfirm;
    public TextMeshProUGUI textConfirm;
    public Button buttonCancel;
    public TextMeshProUGUI textCancel;
    public override void OnShown(object[] data)
    {
        string content = (string)data[0];
        textContent.text = content;
        Action confirm = (Action)data[1];
        string confirmText = (string)data[2];
        if (confirm != null)
        {
            buttonConfirm.gameObject.SetActive(true);
            textConfirm.text = confirmText;
            buttonConfirm.onClick.RemoveAllListeners();
            buttonConfirm.onClick.AddListener(() => { confirm?.Invoke(); OnClose(); });
        }
        else buttonConfirm.gameObject.SetActive(false);

        Action cancel = (Action)data[3];
        string cancelText = (string)data[4];
        if (cancel != null)
        {
            buttonCancel.gameObject.SetActive(true);
            textCancel.text = cancelText;
            buttonCancel.onClick.RemoveAllListeners();
            buttonCancel.onClick.AddListener(() => { cancel?.Invoke(); OnClose(); });
        }
        else buttonCancel.gameObject.SetActive(false);

        base.OnShown(data);
    }
}
