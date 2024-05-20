using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotController : MonoBehaviour
{
    public PlayerController owner { get; set; }
    private BotState botState;
    float maxAttackArea;
    float delayAttackCounter;
    private void Awake()
    {
        owner = GetComponent<PlayerController>();
    }

    public void Init(Vector3 spawnPosition)
    {
        owner.InitPlayer();
        owner.SetToStartPoint(spawnPosition);
        owner.gameObject.SetActive(true);
        owner.characterModel.animator.Play("Spawn");
        botState = BotState.Idle;

    }
    private void Update()
    {
        ProcessBotState();
    }

    private void ProcessBotState()
    {
        if (GameController.Instance.gameState != GameState.Gameplay) return;
        if (owner.curCharacterCondition == PlayerController.CharacterConditions.Dead) return;
        maxAttackArea = owner.characterHandleWeapon.curWeapon.shootDistance * 0.8f;

        if (owner.curCharacterCondition != PlayerController.CharacterConditions.Normal) return;

        switch (botState)
        {
            case BotState.Move:
                ProcessMove();
                break;
            case BotState.Idle:
                ProcessIdle();
                break;
            case BotState.Attack:
                ProcessAttack();
                break;
        }

    }

    private void ProcessMove()
    {
        if (owner.curCharacterCondition == PlayerController.CharacterConditions.Dead) return;
        if (GameController.Instance.mainPlayerController.curCharacterCondition == PlayerController.CharacterConditions.Dead) return;
        Vector3 target = GameController.Instance.mainPlayerController.transform.position;
        target.y = transform.position.y;
        float distance = Vector3.Distance(target, transform.position);

        if (distance > maxAttackArea && distance <= owner.characterProperties.rangeFindPlayer)
        {
            Vector3 direction = target - transform.position;
            owner.characterMovement.Move(direction.normalized);
        }
        else if (distance < owner.characterProperties.rangeRunAwayFromPlayer)
        {
            Vector3 direction = (target - transform.position) * -1;
            owner.characterMovement.Move(direction.normalized);
        }
        else
        {
            delayAttackCounter = 0.1f;
            botState = BotState.Attack;
        }
    }

    private void ProcessAttack()
    {
        if (owner.curCharacterCondition == PlayerController.CharacterConditions.Dead) return;
        if (GameController.Instance.mainPlayerController.curCharacterCondition == PlayerController.CharacterConditions.Dead) return;
        Vector3 target = GameController.Instance.mainPlayerController.transform.position;
        target.y = transform.position.y + 1;
        float distance = Vector3.Distance(target, transform.position);
        if (distance > owner.characterProperties.rangeRunAwayFromPlayer && distance <= owner.characterHandleWeapon.curWeapon.shootDistance)
        {
            Vector3 direction = target - owner.transform.position;
            owner.characterMovement.LookDirection(direction);
            if (delayAttackCounter > 0)
            {
                delayAttackCounter -= Time.deltaTime;
            }
            else
            {
                owner.characterHandleWeapon.Shoot(null);
            }
        }
        else
        {

            botState = BotState.Move;
        }
    }

    private void ProcessIdle()
    {
        if (owner.curCharacterCondition == PlayerController.CharacterConditions.Dead) return;
        botState = BotState.Move;
    }

}

[System.Serializable]
public enum BotState
{
    Move, Idle, Attack
}
