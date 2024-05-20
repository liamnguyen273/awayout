using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public abstract class CharacterAbility : MonoBehaviour
{
    public float cooldown;
    public PlayerController owner { get; set; }
    public CharacterShootingAnimation[] shootAnimations;
    [SerializeField] private float blockOnUseDuration = 0;
    System.Action<float> onReload;
    private float cooldownCounter;
    public Sprite skillUIImage;

    private void Awake()
    {
        owner = GetComponent<PlayerController>();
    }
    public void Shoot(System.Action<float> onReload)
    {
        if (owner.curCharacterCondition == PlayerController.CharacterConditions.Dead) return;
        this.onReload = onReload;
        if (cooldownCounter > 0) return;
        StartReload(cooldown);
        if (blockOnUseDuration > 0)
        {
            owner.TurnOffAim(blockOnUseDuration);
            owner.BlockCharacter(blockOnUseDuration);
        }
        StartCoroutine(RoutineUseAbility());
    }

    public void StartReload(float cooldown)
    {
        if (cooldown > 0)
        {
            cooldownCounter = cooldown;
            onReload?.Invoke(cooldown);
        }
    }

    public virtual void Update()
    {
        cooldownCounter -= Time.deltaTime;
    }

    private IEnumerator RoutineUseAbility()
    {
        if (owner.curCharacterCondition == PlayerController.CharacterConditions.Dead) yield break;
        float delayBeforeUse = 0;
        if (shootAnimations != null && shootAnimations.Length > 0)
        {
            int animIndex = Random.Range(0, shootAnimations.Length);
            delayBeforeUse = shootAnimations[animIndex].delayBeforeShoot;
            owner.characterModel.animator.Play(shootAnimations[animIndex].animName);
        }
        yield return new WaitForSeconds(delayBeforeUse);
        if (owner.curCharacterCondition == PlayerController.CharacterConditions.Dead) yield break;
        UseAbility();
    }
    public abstract void UseAbility();


}
