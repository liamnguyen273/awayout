using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBase : MonoBehaviour
{

    public UIType uiType;
    public virtual string ScreenName { get { return gameObject.name; } }
    public virtual void OnActived(object[] data)
    {

    }

    public virtual void OnDeactived()
    {

    }
    public virtual void OnShown(object[] data) { }
    public virtual void OnHidden() { }
    public virtual void OnClose()
    {
        UIManager.Instance.Close(ScreenName);
    }


}
