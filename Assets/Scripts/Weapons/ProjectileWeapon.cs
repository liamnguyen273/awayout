using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : Weapon
{
    [Header("Projectiles")]
    public int projectilesPerShot = 1;
    public float spread = 0;
    public float bulletSpeed = 100;
    public Transform spawnPositionTransform;
    public GameObject projectilePrefab;
    public int gunIndex;
    private Vector3 spawnPosition;
    public override void Update()
    {
        spawnPosition = spawnPositionTransform.position;
        base.Update();
    }
    public override void WeaponUse()
    {
        base.WeaponUse();
        for (int i = 0; i < projectilesPerShot; i++)
        {
            Projectile projectile = PoolingManager.Instance.GetGameObject<Projectile>(projectilePrefab);
            projectile.gameObject.SetActive(true);
            Vector3 target = owner.aimPositionTransform.position + Random.insideUnitSphere * spread;
            target = spawnPosition + (target - spawnPosition).normalized * shootDistance;
            target.y += (1 - i % 2 * 2) * (Mathf.Ceil(i / 2f));
            target.z = 0;
            projectile.MoveToTarget(owner, spawnPosition, target, damage, bulletSpeed);
        }
    }
}
