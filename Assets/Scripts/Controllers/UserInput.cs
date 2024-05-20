using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInput : MonoBehaviour
{
    private PlayerController playerController;
    private BotState botState = BotState.Idle;
    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    public void Init()
    {
        InputManager.Instance.ShowUI();
        InputManager.Instance.shootingInput.ResetCooldown();
        InputManager.Instance.shootingInput.onHold = () =>
        {
            playerController.characterHandleWeapon.Shoot(
                (duration) =>
                {
                    UIManager.Instance.uIGameplayMenu.StartReload(duration);
                }
                );

        };
        InputManager.Instance.buttonInputUISkill0.onClick = () => { playerController.characterMovement.Jump(); };
        InputManager.Instance.buttonInputUISkill0.ResetCooldown();
        InputManager.Instance.buttonInputUISkill1.buttonImage.sprite = playerController.soldierSkill.skillUIImage;
        InputManager.Instance.buttonInputUISkill1.ResetCooldown();
        InputManager.Instance.buttonInputUISkill1.onClick = () =>
        {
            playerController.soldierSkill.Shoot(
                (duration) =>
                {
                    InputManager.Instance.buttonInputUISkill1.StartCooldown(duration);
                });
        };
        InputManager.Instance.buttonInputUISkill2.ResetCooldown();
        InputManager.Instance.buttonInputUISkill2.onClick = () =>
        {
            playerController.GetComponent<SkillGrenade>().Shoot(
                (duration) =>
                {
                    InputManager.Instance.buttonInputUISkill2.StartCooldown(duration);
                });
        };
        botState = BotState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.Instance.gameState != GameState.Gameplay) return;
        if (playerController.curCharacterCondition == PlayerController.CharacterConditions.Dead) return;
        playerController.characterMovement.Move(new Vector3(InputManager.Instance.movementJoystick.Direction.x + Input.GetAxis("Horizontal"), 0, 0).normalized);
        playerController.characterMovement.Crouch(InputManager.Instance.movementJoystick.Direction.y + Input.GetAxis("Vertical"));
        if (Input.GetKeyDown(KeyCode.Space))
            InputManager.Instance.buttonInputUISkill0.onClick?.Invoke();
        if (Input.GetKey(KeyCode.J))
        {
            InputManager.Instance.shootingInput.onHold.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            InputManager.Instance.buttonInputUISkill2.onClick?.Invoke();
        }

        if (GameManager.autoplay)
        {
            ProcessBotState();
        }
    }

    private void ProcessBotState()
    {
        if (playerController.curCharacterCondition == PlayerController.CharacterConditions.Dead) return;
        if (playerController.curCharacterCondition != PlayerController.CharacterConditions.Normal) return;

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
        if (playerController.curCharacterCondition == PlayerController.CharacterConditions.Dead) return;
        PlayerController target = playerController.FindEnemyForwardNearby();
        if (target != null)
        {
            Vector3 targetPosition = target.transform.position;
            targetPosition.y = transform.position.y;
            float distance = Vector3.Distance(targetPosition, transform.position);
            float maxAttackArea = playerController.characterHandleWeapon.curWeapon.shootDistance * 0.8f;
            if (distance > maxAttackArea)
            {
                Vector3 direction = targetPosition - transform.position;
                playerController.characterMovement.Move(direction.normalized);
            }
            else
            {
                botState = BotState.Attack;
            }
        }
        else
        {

            Vector3 targetPosition = GameController.Instance.endPointPosition;
            Vector3 direction = targetPosition - transform.position;
            playerController.characterMovement.Move(direction.normalized);
        }
    }

    private void ProcessAttack()
    {
        if (playerController.curCharacterCondition == PlayerController.CharacterConditions.Dead) return;
        PlayerController target = playerController.FindEnemyForwardNearby();
        if (target != null)
        {
            Vector3 targetPosition = target.transform.position;
            targetPosition.y = transform.position.y + 1;
            float distance = Vector3.Distance(targetPosition, transform.position);
            if (distance <= playerController.characterHandleWeapon.curWeapon.shootDistance)
            {
                Vector3 direction = targetPosition - playerController.transform.position;
                playerController.characterMovement.LookDirection(direction);
                InputManager.Instance.shootingInput.onHold?.Invoke();
            }
            else
            {
                botState = BotState.Move;
            }
        }
        else
        {
            botState = BotState.Move;
        }
    }

    private void ProcessIdle()
    {
        if (playerController.curCharacterCondition == PlayerController.CharacterConditions.Dead) return;
        botState = BotState.Move;
    }

}
