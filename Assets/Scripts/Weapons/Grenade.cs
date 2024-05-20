using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Grenade : MonoBehaviour
{
   public  GameObject prefabExplodeEffect;
    public DamageArea damageArea;
    public float speed;
    public float damage;
    private PlayerController owner;
    public void MoveToTarget(PlayerController owner,Vector3 spawnPosition, Vector3 target)
    {
        this.owner = owner;
        float distance = Vector3.Distance(spawnPosition, target);
        float duration = distance / speed;
        transform.position = spawnPosition;
        gameObject.SetActive(true);
        transform.DOMoveX(target.x, duration).SetEase(Ease.Linear);
        transform.DOMoveY(target.y + distance * 0.3f, duration / 2f).OnComplete(() => {
            transform.DOMoveY(target.y , duration / 2f).OnComplete(() => {
                Explode();
            }).SetEase(Ease.InQuad); 
        }).SetEase(Ease.OutQuad);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ground")
        {
            transform.DOKill();
            Explode();
        }else if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerController player= other.GetComponent<PlayerController>();
            if(player.teamId!= owner.teamId)
            {
                transform.DOKill();
                Explode();
            }
        }
    }
    private void Explode()
    {
        ParticleSystem explodeEffect = PoolingManager.Instance.GetGameObject<ParticleSystem>(prefabExplodeEffect);
        explodeEffect.transform.position = transform.position;
        explodeEffect.gameObject.SetActive(true);
        damageArea.ActiveDamageArea(damage, 0.1f, owner, ()=> {
            gameObject.SetActive(false);
        });
       
      
    }

    
}
