using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillGrenade : CharacterAbility
{
    public Weapon.WeaponPosition weaponPosition;
    public GameObject prefabGrenade;
    public float attackRange;
    public override void UseAbility()
    {
        Grenade grenade = PoolingManager.Instance.GetGameObject<Grenade>(prefabGrenade);
        Vector3 target = owner.transform.position + (owner.aimPositionTransform.position - owner.transform.position).normalized * attackRange;
        target.y = owner.transform.position.y;
        Vector3 spawnPosition;
        if (weaponPosition == Weapon.WeaponPosition.LeftHand)
        {
            spawnPosition = owner.characterModel.leftHand.position;
        }
        else
        {
            spawnPosition = owner.characterModel.rightHand.position;
        }
        grenade.MoveToTarget(owner, spawnPosition, target);
    }
}
