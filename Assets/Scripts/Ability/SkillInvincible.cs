using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillInvincible : CharacterAbility
{
    public float invincibleDuration;
    public GameObject invincibleEffect;

    private void OnEnable()
    {
        invincibleEffect.gameObject.SetActive(false);
    }
    public override void UseAbility()
    {
        owner.health.SetInvincible(invincibleDuration);
        StartCoroutine(RoutineDisableEffect(invincibleDuration));
    }

    private IEnumerator RoutineDisableEffect(float duration)
    {
        invincibleEffect.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        invincibleEffect.gameObject.SetActive(false);
    }
}
