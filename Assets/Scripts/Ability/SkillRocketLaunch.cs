using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillRocketLaunch : CharacterAbility
{
    public Weapon.WeaponPosition weaponPosition;
    public GameObject prefabRocket;
    public float attackRange;
    public override void UseAbility()
    {
        Rocket rocket = PoolingManager.Instance.GetGameObject<Rocket>(prefabRocket);

        Vector3 spawnPosition;
        if (weaponPosition == Weapon.WeaponPosition.LeftHand)
        {
            spawnPosition = owner.characterModel.leftHand.position;
        }
        else
        {
            spawnPosition = owner.characterModel.rightHand.position;
        }

        Vector3 target = owner.transform.position + (owner.aimPositionTransform.position - owner.transform.position).normalized * attackRange;
        target.y = spawnPosition.y;
        target.z = 0;
        spawnPosition.z = 0;
        rocket.MoveToTarget(owner, spawnPosition, target);
    }
}
