using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Weapon : MonoBehaviour
{
    [Header("ID")]
    public string WeaponName;
    public enum TriggerModes { SemiAuto, Auto }
    public enum WeaponStates { WeaponIdle, WeaponDelayBeforeUse, WeaponUse, WeaponDelayBetweenUses, WeaponReload, WeaponOutOfAmmo }
    public TriggerModes TriggerMode = TriggerModes.Auto;
    public WeaponStates curWeaponState { get; set; }
    public enum WeaponPosition { LeftHand, RightHand }
    public float timeBetweenUses = 1f;
    public WeaponPosition weaponPosition = WeaponPosition.LeftHand;
    [Header("Magazine")]
    public int magazineSize = 30;
    public int maxMagazineSize = 120;
    public float reloadTime = 2f;
    public int ammoConsumedPerShot = 1;
    public float durationPerShot = 0;
    public float timeOutStopShoot = 1;
    public int currentAmmoLoaded { get; private set; } = 0;
    public int ammoUsed { get; private set; } = 0;

    private float reloadCounter;
    private float delayBeforeUseCounter;
    private float delayBetweenUseCounter;

    public float damageCaused = 10;
    [SerializeField] private float damageSpreadPercentage = 0;

    public float damage
    {
        get
        {
            return Mathf.Round(damageCaused + damageCaused * Random.Range(0, damageSpreadPercentage));
        }
    }

    public AudioClip[] shootAudioClips;
    public AudioClip reloadAudioClip;
    public PlayerController owner { get; set; }
    public CharacterAbility characterAbility { get; set; }

    [SerializeField] private CharacterShootingAnimation[] shootAnimations;
    public float shootDistance = 10f;

    public ParticleSystem muzzleEffect;

    public float blockOnUseDuration = 0;


    public void FirstInit(PlayerController owner, CharacterAbility characterAbility)
    {
        this.characterAbility = characterAbility;
        this.owner = owner;
        ammoUsed = 0;
        currentAmmoLoaded = magazineSize;
        ResetWeaponState();
    }

    public void ResetWeaponState()
    {
        curWeaponState = WeaponStates.WeaponIdle;
        switch (weaponPosition)
        {
            case WeaponPosition.LeftHand:
                transform.SetParent(owner.characterModel.leftHand);
                break;
            case WeaponPosition.RightHand:
                transform.SetParent(owner.characterModel.rightHand);
                break;
        }
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

    }

    public virtual void Update()
    {
        ProcessWeaponState();
    }

    private void ProcessWeaponState()
    {
        if (owner == null) return;
        if (owner.curCharacterCondition == PlayerController.CharacterConditions.Dead) return;

        switch (curWeaponState)
        {
            case WeaponStates.WeaponIdle:
                ProcessWeaponIdle();
                break;
            case WeaponStates.WeaponDelayBeforeUse:
                ProcessWeaponDelayBeforeUse();
                break;
            case WeaponStates.WeaponUse:
                ProcessWeaponUse();
                break;
            case WeaponStates.WeaponReload:
                ProcessWeaponReload();
                break;
            case WeaponStates.WeaponDelayBetweenUses:
                ProcessWeaponDelayBetweenUse();
                break;
            case WeaponStates.WeaponOutOfAmmo:
                ProcessWeaponOutOfAmmo();
                break;

        }

    }

    private void ProcessWeaponIdle()
    {
        if (owner.curCharacterCondition == PlayerController.CharacterConditions.Dead) return;
    }

    private void ProcessWeaponDelayBeforeUse()
    {
        if (owner.curCharacterCondition == PlayerController.CharacterConditions.Dead) return;
        delayBeforeUseCounter -= Time.deltaTime;
        if (delayBeforeUseCounter > 0) return;
        curWeaponState = WeaponStates.WeaponUse;
    }

    private void ProcessWeaponUse()
    {
        if (owner.curCharacterCondition == PlayerController.CharacterConditions.Dead) return;
        if (currentAmmoLoaded >= ammoConsumedPerShot)
        {
            currentAmmoLoaded -= ammoConsumedPerShot;
            ammoUsed += ammoConsumedPerShot;
            StartCoroutine(RoutineUseWeapon());
        }
    }

    private void StartReload()
    {
        curWeaponState = WeaponStates.WeaponReload;
        reloadCounter = reloadTime;
        characterAbility.StartReload(reloadTime);
        if (reloadAudioClip != null)
        {
            owner.PlayAudioClip(reloadAudioClip);
        }
    }

    private void ProcessWeaponReload()
    {
        if (owner.curCharacterCondition == PlayerController.CharacterConditions.Dead) return;
        reloadCounter -= Time.deltaTime;
        if (reloadCounter <= 0)
        {
            currentAmmoLoaded = magazineSize;
            curWeaponState = WeaponStates.WeaponIdle;
        }
    }

    private void ProcessWeaponDelayBetweenUse()
    {
        if (owner.curCharacterCondition == PlayerController.CharacterConditions.Dead) return;
        delayBetweenUseCounter -= Time.deltaTime;
        if (delayBetweenUseCounter <= 0)
        {
            if (ammoUsed >= maxMagazineSize)
            {
                curWeaponState = WeaponStates.WeaponOutOfAmmo;
            }
            else
            {
                if (currentAmmoLoaded < ammoConsumedPerShot)
                {
                    StartReload();
                }
                else
                {
                    curWeaponState = WeaponStates.WeaponIdle;
                }
            }

        }
    }
    private void ProcessWeaponOutOfAmmo()
    {
        if (owner.curCharacterCondition == PlayerController.CharacterConditions.Dead) return;

        if (ammoUsed < maxMagazineSize)
        {
            StartReload();
        }


    }

    public bool canUse
    {
        get
        {
            if (owner.curCharacterCondition != PlayerController.CharacterConditions.Normal) return false;
            if (curWeaponState == WeaponStates.WeaponIdle) return true;
            return false;
        }
    }
    public void Shoot()
    {
        if (owner.curCharacterCondition == PlayerController.CharacterConditions.Dead) return;
        if (curWeaponState == WeaponStates.WeaponIdle)
        {
            int index = Random.Range(0, shootAnimations.Length);
            owner.characterModel.animator.Play(shootAnimations[index].animName);
            delayBeforeUseCounter = shootAnimations[index].delayBeforeShoot;
            curWeaponState = WeaponStates.WeaponDelayBeforeUse;
        }
    }
    public virtual void WeaponUse()
    {
        if (owner.curCharacterCondition == PlayerController.CharacterConditions.Dead) return;
        curWeaponState = WeaponStates.WeaponDelayBetweenUses;
        delayBetweenUseCounter = timeBetweenUses;
        muzzleEffect.Play();
        owner.PlayAudioClip(shootAudioClips[Random.Range(0, shootAudioClips.Length)]);
        owner.BlockCharacter(blockOnUseDuration);
    }

    public IEnumerator RoutineUseWeapon()
    {
        if (owner.curCharacterCondition == PlayerController.CharacterConditions.Dead) yield break;
        for (int i = 0; i < ammoConsumedPerShot; i++)
        {
            WeaponUse();
            yield return new WaitForSeconds(durationPerShot);
        }
    }

    public void AddAmmo(int value)
    {
        ammoUsed -= value;
        ammoUsed = Mathf.Max(0, ammoUsed);
    }


}
