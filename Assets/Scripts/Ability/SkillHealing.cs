using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHealing : CharacterAbility
{
    public float healValue;

    public ParticleSystem healingEffect;
    public override void UseAbility()
    {
        healingEffect.transform.position = owner.transform.position;
        healingEffect.transform.rotation = Quaternion.LookRotation(Vector3.up);
        healingEffect.Play();
        owner.health.Healing(healValue);
    }
}