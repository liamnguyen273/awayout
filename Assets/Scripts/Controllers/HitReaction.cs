using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
[System.Serializable]
public class HitReaction
{
    [SerializeField] private Color hitColor;
    [SerializeField] private GameObject prefabHitImpact;
    public void Hit(string value, Vector3 spawnPosition)
    {
        HitImpact hitEffect = PoolingManager.Instance.GetGameObject<HitImpact>(prefabHitImpact);
        hitEffect.PlayEffect(value, spawnPosition, hitColor);
       
    }
}