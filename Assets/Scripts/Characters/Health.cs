using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public PlayerController owner { get; private set; }
    public HealthBar healthBar;
    public float blockOnHitDuration = 0.1f;
    public HitReaction hitReaction;
    public HitReaction healReaction;
    public float currentHealth { get; private set; }
    private bool isInvincible = false;
    private void Awake()
    {
        owner = GetComponent<PlayerController>();
    }
    public void Revive()
    {
        isInvincible = false;
        currentHealth = owner.characterProperties.maxHealth;
        if (healthBar != null)
        {
            healthBar.SetHealthPercentage(1f, currentHealth, owner.characterProperties.maxHealth, true);
            healthBar.gameObject.SetActive(true);
        }
    }

    public void Hit(float damage)
    {
        if (GameController.Instance.gameState != GameState.Gameplay) return;
        if (owner.curCharacterCondition == PlayerController.CharacterConditions.Dead) return;
        if (GameManager.godmod && owner.teamId == 0) return;
        if (isInvincible) return;
        currentHealth -= damage;
        hitReaction.Hit(damage.ToString(), transform.position + Vector3.up);
        currentHealth = Mathf.Max(0, currentHealth);
        healthBar.SetHealthPercentage(currentHealth / owner.characterProperties.maxHealth, currentHealth, owner.characterProperties.maxHealth);
        if (currentHealth <= 0)
        {
            healthBar.gameObject.SetActive(false);
            owner.PlayerDead();
        }
        else
        {
            owner.characterModel.animator.SetTrigger("Hit");
            owner.BlockCharacter(blockOnHitDuration);
        }
    }
    public void Healing(float heal)
    {
        currentHealth += heal;
        currentHealth = Mathf.Min(currentHealth, owner.characterProperties.maxHealth);
        healthBar.SetHealthPercentage(currentHealth / owner.characterProperties.maxHealth, currentHealth, owner.characterProperties.maxHealth);
        healReaction.Hit(heal.ToString(), transform.position + Vector3.up);
    }

    public void SetInvincible(float duration)
    {
        StartCoroutine(RoutineSetInvincible(duration));
    }
    private IEnumerator RoutineSetInvincible(float duration)
    {
        isInvincible = true;
        yield return new WaitForSeconds(duration);
        isInvincible = false;
    }

}
