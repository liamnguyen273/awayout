using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;
using DG.Tweening;
public class HitImpact : MonoBehaviour
{
    public TextMeshProUGUI text;
    private CinemachineImpulseSource cameraShakeSource;

    public void PlayEffect(string text, Vector3 spawnPosition, Color hitColor)
    {
        this.text.text = text;
        if (cameraShakeSource == null)
            cameraShakeSource = GetComponent<CinemachineImpulseSource>();
        if (cameraShakeSource != null) cameraShakeSource.GenerateImpulse();
        transform.position = spawnPosition;
        gameObject.SetActive(true);
        transform.DOKill();
        this.text.color = hitColor;
        float duration = Random.Range(0.3f, 0.5f);
        transform.DOMoveY(spawnPosition.y + 3f, duration).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            this.text.DOColor(new Color(hitColor.r, hitColor.g, hitColor.b, 0), 0.2f).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        });
    }
}
