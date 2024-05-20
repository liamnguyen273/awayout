using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UICreditsPopup : UIBase
{
    [SerializeField] private Button buttonClose;
    private void Awake()
    {
        buttonClose.onClick.AddListener(() => { OnClose(); });
    }
    public override void OnShown(object[] data)
    {
        base.OnShown(data);
    }
}
