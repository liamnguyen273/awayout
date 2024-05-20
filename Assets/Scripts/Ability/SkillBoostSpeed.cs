using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBoostSpeed : CharacterAbility
{
    public float boostSpeedDuration;
    public float boostSpeedPercentage;
    public GameObject boostSpeedEffect;
    public DamageArea damageArea;
    public float damage;
    bool boost = false;
    private void OnEnable()
    {
        boostSpeedEffect.gameObject.SetActive(false);
    }

    public override void Update()
    {
        base.Update();
        if (!boost) return;
        if (Time.frameCount % 20 == 0)
        {
            damageArea.ActiveDamageArea(damage, 0.01f, owner);
        }
    }
    public override void UseAbility()
    {
        owner.characterMovement.BoostSpeed(boostSpeedDuration, boostSpeedPercentage);
        StartCoroutine(RoutineDisableEffect(boostSpeedDuration));
    }

    private IEnumerator RoutineDisableEffect(float duration)
    {
        boost = true;
        boostSpeedEffect.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        boostSpeedEffect.gameObject.SetActive(false);
        boost = false;
    }
}
