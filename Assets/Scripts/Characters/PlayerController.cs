using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using RootMotion.FinalIK;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(CharacterMovement))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    public int teamId;
    public CharacterProperties characterProperties;
    public CharacterModel characterModel;
    public Health health { get; set; }
    public ParticleSystem deadEffect;
    public HitReaction deathReaction;
    public AudioClip[] audioClipsDead;
    private AudioSource audioSource;
    public CharacterMovement characterMovement { get; set; }
    public CharacterHandleWeapon characterHandleWeapon;
    public Transform aimPositionTransform;
    public CharacterAbility soldierSkill { get; set; }
    private GameObject soldierSkillObject;
    public CharacterConditions curCharacterCondition { get; private set; }
    private float blockCounter;
    public bool autoAim;
    public bool canLootItem = false;
    public enum CharacterConditions
    {
        Normal,
        ControlledMovement,
        Frozen,
        Paused,
        Dead
    }
    private void Awake()
    {
        characterMovement = GetComponent<CharacterMovement>();
        health = GetComponent<Health>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        ProcessAim();
    }

    private void ProcessAim()
    {
        if (!autoAim) return;
        PlayerController target = FindEnemyForwardNearby();
        if (target != null)
        {
            Vector3 center = transform.position + new Vector3(0, 1.5f, 0);
            Vector3 aimTargetPosition = target.transform.position + new Vector3(0, 1.5f, 0);
            Vector3 dir = (aimTargetPosition - center).normalized;
            aimTargetPosition = center + dir * 5f;
            aimTargetPosition.y = Mathf.Clamp(aimTargetPosition.y, center.y - 2f, center.y + 2f);
            aimPositionTransform.position = aimTargetPosition;
        }
        else
        {
            aimPositionTransform.localPosition = new Vector3(0, 1.5f, 5f);
        }
    }

    public PlayerController FindEnemyForwardNearby()
    {
        List<PlayerController> playerControllers = GameController.Instance.GetActiveEnemies(teamId);
        PlayerController target = null;
        float minEnemyDistance = Mathf.Infinity;
        for (int i = 0; i < playerControllers.Count; i++)
        {
            float dir = (transform.position + transform.forward).x - transform.position.x;
            if ((playerControllers[i].transform.position.x - transform.position.x) / dir > 0)
            {
                float distance = Vector3.Distance(transform.position, playerControllers[i].transform.position);
                if (distance > 2f && distance < minEnemyDistance)
                {
                    minEnemyDistance = distance;
                    target = playerControllers[i];
                }
            }
        }
        return target;
    }
    private void LateUpdate()
    {
        ProcessCharacterCondition();
    }

    private void ProcessCharacterCondition()
    {
        switch (curCharacterCondition)
        {
            case CharacterConditions.Normal:
                break;
            case CharacterConditions.Frozen:
                ProcessCharacterConditionFrozen();
                break;
            case CharacterConditions.Dead:
                break;
        }
    }

    private void ProcessCharacterConditionFrozen()
    {
        if (curCharacterCondition == CharacterConditions.Dead) return;
        if (blockCounter > 0)
        {
            blockCounter -= Time.deltaTime;
        }
        else
        {
            curCharacterCondition = CharacterConditions.Normal;
        }
    }

    public void InitPlayer(WeaponData curWeaponData = null, CharacterProperties characterProperties = null)
    {
        if (characterProperties != null)
            this.characterProperties = characterProperties;
        if (this.characterProperties is SoldierProperties)
        {
            if (characterModel != null)
                characterModel.gameObject.SetActive(false);
            SoldierProperties soldierProperties = this.characterProperties as SoldierProperties;
            CharacterModel model = PoolingManager.Instance.GetGameObject<CharacterModel>(soldierProperties.characterPrefab, transform);
            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
            model.aimIK.solver.target = aimPositionTransform;
            model.transform.localScale = Vector3.one;
            model.gameObject.SetActive(true);
            characterModel = model;
            characterModel.animator.SetFloat("AnimSpeed", this.characterProperties.animationSpeed);

            if (soldierSkillObject != null)
                soldierSkillObject.gameObject.SetActive(false);
            soldierSkill = PoolingManager.Instance.GetGameObject<CharacterAbility>(soldierProperties.soldierSkillPrefab, transform);
            soldierSkillObject = soldierSkill.gameObject;
            soldierSkillObject.SetActive(true);
            soldierSkill.owner = this;
        }

        curCharacterCondition = CharacterConditions.Normal;
        if (curWeaponData != null)
        {
            characterHandleWeapon.mainWeapon = null;
            if (characterHandleWeapon.curWeapon != null)
                characterHandleWeapon.curWeapon.gameObject.SetActive(false);

            characterHandleWeapon.InitWeapon(curWeaponData);
        }

        else
            characterHandleWeapon.InitWeapon();
        characterHandleWeapon.EquipToMainWeapon();
        characterMovement.ResetValue();
        health.Revive();
    }

    public void SetToStartPoint(Vector3 spawnPosition)
    {
        transform.rotation = Quaternion.LookRotation(Vector3.right);
        characterMovement.MoveToPosition(spawnPosition);
    }
    public void PlayerDead()
    {
        if (curCharacterCondition == CharacterConditions.Dead) return;
        if (characterModel.aimIK != null) characterModel.aimIK.enabled = false;
        deathReaction.Hit("", transform.position);
        curCharacterCondition = CharacterConditions.Dead;
        characterModel.animator.ResetTrigger("Hit");
        characterModel.animator.SetTrigger("Dead");
        deadEffect.Play();
        PlayAudioClip(audioClipsDead[Random.Range(0, audioClipsDead.Length)]);
        StartCoroutine(RoutineDisableObject());
    }

    public void TurnOffAim(float duration)
    {
        StartCoroutine(RoutineTurnOffAim(duration));
    }

    private IEnumerator RoutineTurnOffAim(float duration)
    {
        TurnOffAim();
        yield return new WaitForSeconds(duration);
        TurnOnAim();
    }
    public void TurnOffAim()
    {
        if (characterModel.aimIK != null)
        {
            characterModel.aimIK.enabled = false;
            characterModel.animator.SetLayerWeight(1, 0);

        }
    }

    public void TurnOnAim()
    {
        if (characterModel.aimIK != null)
        {
            characterModel.animator.SetLayerWeight(1, 1);
            characterModel.aimIK.enabled = true;
        }
    }
    public void BlockCharacter(float duration)
    {
        if (curCharacterCondition == CharacterConditions.Dead) return;
        curCharacterCondition = CharacterConditions.Frozen;
        blockCounter = duration;
    }
    public void PlayAudioClip(AudioClip audioClip)
    {
        audioSource.volume = Random.Range(0.5f, 1f);
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.PlayOneShot(audioClip);
    }

    private IEnumerator RoutineDisableObject()
    {
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }
}
