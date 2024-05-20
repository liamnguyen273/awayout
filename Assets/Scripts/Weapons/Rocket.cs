using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Rocket : MonoBehaviour
{
    public GameObject prefabExplodeEffect;
    public DamageArea damageArea;
    public float speed;
    public float damage;
    private PlayerController owner;
    public void MoveToTarget(PlayerController owner, Vector3 spawnPosition, Vector3 target)
    {
        this.owner = owner;
        float distance = Vector3.Distance(spawnPosition, target);
        float duration = distance / speed;
        transform.position = spawnPosition;
        transform.transform.LookAt(target);
        gameObject.SetActive(true);
        transform.DOMove(target, duration).SetEase(Ease.InQuart).OnComplete(() =>
            {
                Explode();
            });

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player.teamId != owner.teamId)
            {
                transform.DOKill();
                Explode();
            }
        }
    }
    private void Explode()
    {
        ParticleSystem explodeEffect = PoolingManager.Instance.GetGameObject<ParticleSystem>(prefabExplodeEffect);
        explodeEffect.transform.position = transform.position;
        explodeEffect.gameObject.SetActive(true);
        damageArea.ActiveDamageArea(damage, 0.1f, owner, () =>
        {
            gameObject.SetActive(false);
        });


    }
}
