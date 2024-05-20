using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    public Vector3 damageOffset;
    public DamageArea damageArea;

    public override void WeaponUse()
    {
        muzzleEffect.transform.SetParent(owner.transform);
        muzzleEffect.transform.localPosition = damageOffset;
        muzzleEffect.transform.rotation = owner.transform.rotation;
        Vector3 target = owner.aimPositionTransform.position + Random.insideUnitSphere;
        Vector3 direction = (target - owner.transform.position).normalized;
        target = owner.transform.position + direction * shootDistance;
        target.z = 0;
        damageArea.transform.position = owner.transform.position + damageOffset;
        damageArea.transform.rotation = owner.transform.rotation;
        damageArea.ActiveDamageArea(damage, 0.1f, owner);
        base.WeaponUse();
    }

}
