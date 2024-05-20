using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    private PlayerController owner;
    private CharacterController characterController;
    private float velocity;
    private Vector3 lastPosition;
    private float moveDelta;
    private float jumpCounter;
    private bool grounded;
    private float speedBoostPercentage;
    private float crouch;
    private void Awake()
    {
        owner = GetComponent<PlayerController>();
        characterController = GetComponent<CharacterController>();
    }

    public void ResetValue()
    {
        speedBoostPercentage = 0;
        velocity = 0;
        grounded = true;
        owner.characterModel.animator.SetFloat("AnimSpeed", 1);
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Ground"))
        {
            grounded = true;
        }

    }

    public void BoostSpeed(float duration, float speedBoostPercentage)
    {
        StartCoroutine(RoutineBoostSpeed(duration, speedBoostPercentage));
    }

    private IEnumerator RoutineBoostSpeed(float duration, float speedBoostPercentage)
    {
        owner.characterModel.animator.SetFloat("AnimSpeed", 1 + speedBoostPercentage);
        this.speedBoostPercentage = speedBoostPercentage;
        yield return new WaitForSeconds(duration);
        owner.characterModel.animator.SetFloat("AnimSpeed", 1);
        this.speedBoostPercentage = 0;
    }


    private void Update()
    {
        if (characterController.enabled)
        {
            velocity += Physics.gravity.y * 6f * Time.deltaTime;
            characterController.Move(Vector3.up * velocity * Time.deltaTime);
            moveDelta -= Time.deltaTime;
            owner.characterModel.animator.SetFloat("MoveSpeed", moveDelta);
            lastPosition = transform.position;
        }
    }

    private void LateUpdate()
    {
        if (grounded)
        {
            jumpCounter -= Time.deltaTime;
            velocity = -8;
            if (owner.curCharacterCondition == PlayerController.CharacterConditions.Dead)
            {
                characterController.enabled = false;
            }
        }
        owner.characterModel.animator.SetBool("OnGround", grounded);
    }

    public void Move(Vector3 direction)
    {
        if (owner.curCharacterCondition != PlayerController.CharacterConditions.Normal) return;
        Vector3 moveDirection = direction;
        Vector3 moveTocenter = new Vector3(transform.position.x, transform.position.y, 0) - transform.position;
        moveDirection.z = moveTocenter.z;
        float speed = (owner.characterProperties.moveSpeed) + (speedBoostPercentage * owner.characterProperties.moveSpeed);
        characterController.Move(moveDirection * (speed + crouch * 0.5f * speed) * Time.deltaTime);
        moveDelta = Mathf.Abs(direction.x);
        characterController.transform.DOKill();
        if (direction != Vector3.zero)
            characterController.transform.DORotateQuaternion(Quaternion.LookRotation(direction), 0.1f);
    }

    public void Crouch(float crouch)
    {
        if (owner.curCharacterCondition == PlayerController.CharacterConditions.Dead) return;
        this.crouch = Mathf.Clamp(crouch, -1f, 0f);
        crouch *= -1f;
        owner.characterModel.animator.SetFloat("Crouch", crouch);
    }

    public void LookDirection(Vector3 direction)
    {
        direction.y = 0;
        direction.z = 0;
        if (direction != Vector3.zero)
            characterController.transform.DORotateQuaternion(Quaternion.LookRotation(direction), 0.1f);
    }
    public void MoveToPosition(Vector3 position)
    {
        characterController.enabled = false;
        transform.position = position;
        characterController.enabled = true;
    }
    public void Jump()
    {
        if (owner.curCharacterCondition != PlayerController.CharacterConditions.Normal) return;
        if (!grounded || jumpCounter > 0) return;
        grounded = false;
        jumpCounter = 0.2f;
        velocity = owner.characterProperties.jumpForce;
        owner.characterModel.animator.SetTrigger("Jump");
    }

}
