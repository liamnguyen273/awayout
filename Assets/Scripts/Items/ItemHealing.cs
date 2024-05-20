using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHealing : ItemBase
{
    public float healValue = 100f;
    public GameObject effectPrefab;
    public override void GetItem(PlayerController target)
    {
        target.health.Healing(healValue);
        ParticleSystem effect = PoolingManager.Instance.GetGameObject<ParticleSystem>(effectPrefab);
        effect.transform.position = target.transform.position + Vector3.up * 0.1f;
        effect.gameObject.SetActive(true);
        effect.Play();

    }
}
