using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public abstract class ItemBase : MonoBehaviour
{
    private bool itemUsed;
    public abstract void GetItem(PlayerController target);

    private void OnEnable()
    {
        transform.DOKill();
        transform.localScale = Vector3.one;
        itemUsed = false;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (itemUsed) return;
        PlayerController characterController = other.GetComponent<PlayerController>();
        if (characterController == null) return;
        if (!characterController.canLootItem) return;
        itemUsed = true;
        GetItem(characterController);
        transform.DOScale(Vector3.zero, 0.5f);
        transform.DOMoveY(transform.position.y + 2f, 0.5f).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
