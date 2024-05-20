using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
public class UILoadingScreen : UIBase
{
    [SerializeField] private Slider sliderLoading;
    [SerializeField] private TextMeshProUGUI textProgress;
    [SerializeField] private TextMeshProUGUI textContent;
    [SerializeField] private TextMeshProUGUI textLoading;
    public override void OnShown(object[] data)
    {
        base.OnShown(data);
        StartCoroutine(TextAnim());
    }

    IEnumerator TextAnim()
    {
        textLoading.text = "LOADING";
        yield return new WaitForSeconds(0.5f);
        textLoading.text = "LOADING.";
        yield return new WaitForSeconds(0.5f);
        textLoading.text = "LOADING..";
        yield return new WaitForSeconds(0.5f);
        textLoading.text = "LOADING...";
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(TextAnim());
    }

    public void SetLoadingProgress(float progress, float duration, string content, System.Action callback)
    {
        sliderLoading.DOKill();
        textContent.text = content;
        if (sliderLoading.value > progress || duration == 0)
        {
            sliderLoading.value = progress;
            textProgress.text = Mathf.RoundToInt(progress) + "%";
            callback?.Invoke();
        }
        else
        {
            sliderLoading.DOValue(progress, duration).OnUpdate(() =>
            {
                textProgress.text = Mathf.RoundToInt(progress) + "%";
            }).OnComplete(() =>
            {
                callback?.Invoke();
            });
        }
    }
}
