using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterHandleWeapon : CharacterAbility
{
    public bool updateOnUI = false;
    public Weapon curWeapon { get; private set; }
    public GameObject prefabMainWeapon;
    public GameObject prefabSecondWeapon;
    public Weapon mainWeapon { get; set; }
    public Weapon secondWeapon { get; private set; }

    public override void Update()
    {
        base.Update();
        if (updateOnUI && mainWeapon != null)
        {
            int currentAmmo = mainWeapon.maxMagazineSize - mainWeapon.ammoUsed;
            UIManager.Instance.uIGameplayMenu.SetAmmo(currentAmmo, mainWeapon.maxMagazineSize);
            if (currentAmmo == 0)
            {
                GameManager.Instance.ShowRewardPopupAmmo();
            }

        }

    }

    public void InitWeapon(WeaponData mainWeaponData)
    {
        prefabMainWeapon = mainWeaponData.weaponPrefab;
        InitWeapon();
    }
    public void InitWeapon()
    {
        if (mainWeapon != null) return;
        mainWeapon = PoolingManager.Instance.GetGameObject<Weapon>(prefabMainWeapon, owner.transform);
        mainWeapon.transform.localScale = Vector3.one;
        mainWeapon.gameObject.SetActive(true);
        mainWeapon.FirstInit(owner, this);
        if (prefabSecondWeapon != null)
        {
            secondWeapon = PoolingManager.Instance.GetGameObject<Weapon>(prefabSecondWeapon, owner.transform);
            secondWeapon.FirstInit(owner, this);
        }

    }
    public override void UseAbility()
    {
        if (curWeapon.curWeaponState != Weapon.WeaponStates.WeaponIdle) return;
        FindEnemyInDistanceUseMeleeWeapon();
        curWeapon.Shoot();
    }
    private void FindEnemyInDistanceUseMeleeWeapon()
    {
        if (secondWeapon == null) return;
        List<PlayerController> playerController = GameController.Instance.GetActiveEnemies(owner.teamId);
        if (playerController == null) return;
        for (int i = 0; i < playerController.Count; i++)
        {
            if (playerController[i].curCharacterCondition == PlayerController.CharacterConditions.Dead) continue;
            float distance = Vector3.Distance(owner.transform.position, playerController[i].transform.position);
            if (distance <= owner.characterProperties.rangeUseSecondWeapon)
            {
                EquipToSecondWeapon();
                StartCoroutine(RoutineSetMainWeapon(secondWeapon.blockOnUseDuration));
                return;
            }
        }
    }

    private IEnumerator RoutineSetMainWeapon(float duration)
    {
        yield return new WaitForSeconds(duration);
        EquipToMainWeapon();
    }

    public void EquipToMainWeapon()
    {
        EquipWeapon(mainWeapon);
    }

    public void EquipToSecondWeapon()
    {
        EquipWeapon(secondWeapon);
    }
    public void EquipWeapon(Weapon weapon)
    {
        if (curWeapon == weapon) return;
        if (curWeapon != null && curWeapon.curWeaponState == Weapon.WeaponStates.WeaponUse) return;
        if (mainWeapon != null)
            mainWeapon.gameObject.SetActive(false);
        if (secondWeapon != null)
            secondWeapon.gameObject.SetActive(false);
        curWeapon = weapon;
        if (owner.characterModel.aimIK != null)
        {
            if (curWeapon is ProjectileWeapon)
            {
                owner.characterModel.animator.SetInteger("GunIndex", (curWeapon as ProjectileWeapon).gunIndex);
                owner.TurnOnAim();
            }
            if (curWeapon is MeleeWeapon)
            {
                owner.TurnOffAim();
            }
        }
        curWeapon.gameObject.SetActive(true);
        curWeapon.ResetWeaponState();
    }
}
