using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class ButtonInputUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public UnityAction onClick;
    public UnityAction onHold;
    private Button button;
    public Image buttonImage;
    public Image cooldownCircle;
    private bool clicked = false;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.interactable = true;
        if (cooldownCircle != null)
            cooldownCircle.gameObject.SetActive(false);
    }

    public void AddListener(UnityAction onClick)
    {
        this.onClick = onClick;
    }

    private void Update()
    {
        if (Input.touchCount == 0)
        {
            clicked = false;
        }
        if (clicked)
        {
            clicked = false;
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (Input.GetTouch(i).position.x > Screen.width / 2)
                {
                    clicked = true;
                }
            }
            onHold?.Invoke();
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (clicked) return;
        clicked = true;
        onClick?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        clicked = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        clicked = false;
    }
    public void StartCooldown(float duration)
    {
        clicked = false;
        cooldownCircle.fillAmount = 1;
        cooldownCircle.DOFillAmount(0, duration).OnComplete(() =>
        {
            button.interactable = true;
            cooldownCircle.gameObject.SetActive(false);
        }).SetEase(Ease.Linear).SetDelay(0.2f).OnStart(() =>
        {
            button.interactable = false;
            cooldownCircle.gameObject.SetActive(true);
        });
    }

    public void ResetCooldown()
    {
        StopAllCoroutines();
        transform.DOKill();
        clicked = false;
        button.interactable = true;
        if (cooldownCircle != null)
            cooldownCircle.gameObject.SetActive(false);
    }


}
