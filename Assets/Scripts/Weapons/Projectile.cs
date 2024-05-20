using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Projectile : MonoBehaviour
{
    public GameObject explodePrefab;
    private PlayerController owner;
    public LayerMask layerMask;
    private float damageCaused;
    public void MoveToTarget(PlayerController owner, Vector3 spawnPosition, Vector3 target, float damageCaused, float bulletSpeed)
    {
        this.damageCaused = damageCaused;
        this.owner = owner;
        transform.DOKill();
        transform.LookAt(target);
        transform.position = spawnPosition;
        float duration = Vector3.Distance(target, spawnPosition) / bulletSpeed;
        target.z = 0;
        transform.DOMove(target, duration).SetDelay(Time.deltaTime).SetEase(Ease.Linear).OnUpdate(() =>
        {

            RaycastHit hit;
            float castDistance = Mathf.Min(bulletSpeed * Time.deltaTime * 2f, Vector3.Distance(target, transform.position));

            Vector3 projPostion = transform.position;
            projPostion.z = 0;
            Vector3 direction = target - projPostion;
            if (Physics.Raycast(projPostion - direction.normalized, direction.normalized, out hit, castDistance, layerMask))
            {
                Health hitHealth = hit.collider.GetComponent<Health>();
                if (hitHealth != null
                && hitHealth.owner.curCharacterCondition != PlayerController.CharacterConditions.Dead
                && hitHealth.owner.teamId != owner.teamId)
                {

                    MoveToHitPoint(hitHealth, hit.point, bulletSpeed);

                }
            }

        }).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
    }

    private void MoveToHitPoint(Health hitHealth, Vector3 hitPoint, float bulletSpeed)
    {
        transform.DOKill();
        hitPoint.z = transform.position.z;
        float duration = Vector3.Distance(hitPoint, transform.position) / bulletSpeed;
        transform.DOMove(hitPoint, duration).OnComplete(() =>
        {
            hitHealth.Hit(damageCaused);
            Explode(hitPoint);
        }).SetEase(Ease.Linear);
    }
    private void Explode(Vector3 position)
    {
        Transform explode = PoolingManager.Instance.GetGameObject<Transform>(explodePrefab);
        explode.position = position;
        explode.gameObject.SetActive(true);
        transform.DOKill();
        gameObject.SetActive(false);
    }
}
