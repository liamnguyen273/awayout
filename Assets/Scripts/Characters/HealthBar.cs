using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
public class HealthBar : MonoBehaviour
{
    public Slider foregroundBar;
    public Slider delayedBar;
    public TextMeshProUGUI textHealth;
    public bool showText = false;
    public bool onlyShowOnHit = true;
    public bool updateUI = false;
    private void Start()
    {
        textHealth.gameObject.SetActive(showText);
        gameObject.SetActive(!onlyShowOnHit);
    }
    private void Update()
    {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.back, Camera.main.transform.rotation * Vector3.down);
    }
    public void SetHealthPercentage(float healthPercentage, float value, float maxValue, bool force = false)
    {
        if (updateUI)
            UIManager.Instance.uIGameplayMenu.SetHealth(healthPercentage, value, maxValue);
        StopAllCoroutines();
        textHealth.text = value.ToString();
        if (force)
        {
            foregroundBar.DOKill();
            delayedBar.DOKill();
            foregroundBar.value = healthPercentage;
            delayedBar.value = healthPercentage;
            gameObject.SetActive(!onlyShowOnHit);
        }
        else
        {
            foregroundBar.DOKill();
            bool damaged = healthPercentage < foregroundBar.value;
            if (damaged)
            {
                if (onlyShowOnHit)
                {
                    gameObject.SetActive(true);
                    StartCoroutine(RoutineDisable());
                }
                delayedBar.DOKill();
                delayedBar.DOValue(healthPercentage, 2f).SetEase(Ease.InExpo);
            }
            foregroundBar.DOValue(healthPercentage, 0.5f).OnComplete(() =>
            {
                if (!damaged)
                {
                    delayedBar.DOKill();
                    delayedBar.value = healthPercentage;
                }
            });

        }

    }

    private IEnumerator RoutineDisable()
    {
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
        transform.DOKill();
    }
}
