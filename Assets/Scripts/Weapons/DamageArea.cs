using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageArea : MonoBehaviour
{
    private PlayerController owner;
    public GameObject explodePrefab;
    float damage;

    private void Awake()
    {
        gameObject.SetActive(false);
    }
    public void ActiveDamageArea(float damage, float duration, PlayerController owner, System.Action callback = null)
    {
        this.owner = owner;
        this.damage = damage;
        gameObject.SetActive(true);
        StartCoroutine(RoutineDisable(duration, callback));
    }

    IEnumerator RoutineDisable(float duration, System.Action callback)
    {
        yield return new WaitForSeconds(duration);
        callback?.Invoke();
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        Health health = other.gameObject.GetComponent<Health>();
        if (health != null && health.owner.teamId != owner.teamId)
        {
          
            Vector3 hitPosition = other.gameObject.transform.position;
            hitPosition.y = owner.characterHandleWeapon.curWeapon.transform.position.y;
            if (explodePrefab != null)
            {
                Transform explode = PoolingManager.Instance.GetGameObject<Transform>(explodePrefab);
                explode.position = hitPosition;
                explode.gameObject.SetActive(true);
            }
            health.Hit(damage);
        }
    }
}
