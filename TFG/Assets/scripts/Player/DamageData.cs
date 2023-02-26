using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageData : MonoBehaviour
{
    [SerializeField] internal Transform ownerTransform;
    [SerializeField] internal float damage = 10;
    [SerializeField] internal ElementsManager.Elements attackElement;
    [SerializeField] internal bool alwaysAttacking = false;

    [SerializeField] AudioManager audio;


    public bool IsAttacking
    {
        get
        {
            if (alwaysAttacking) return true;
            if (ownerTransform == null) return false;

            LifeSystem.EntityType entityType = ownerTransform.GetComponent<LifeSystem>().entityType;
            if (entityType == LifeSystem.EntityType.PLAYER)
            {
                return ownerTransform.GetComponent<PlayerSword>().isAttacking;
            }
            else if (entityType == LifeSystem.EntityType.ENEMY)
            {
                return ownerTransform.GetComponent<BaseEnemyScript>().isAttacking;
            }

            Debug.LogWarning("Entity not assigned");
            return false;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        ApplyDamage(other.transform);
    }

    private void OnCollisionEnter(Collision col)
    {
        ApplyDamage(col.transform);
    }



    void DamageToPlayer(Transform _player)
    {
        if (audio != null)
            audio.PlaySound();

        LifeSystem lifeSystem = _player.GetComponent<LifeSystem>();
        lifeSystem.Damage(damage, attackElement);
        _player.GetComponent<PlayerMovement>().DamageStartCorroutine();
    }

    void DamageToEnemy(Transform _enemy)
    {
        if (audio != null)
            audio.PlaySound();

        LifeSystem lifeSystem = _enemy.GetComponent<LifeSystem>();
        //Debug.Log("Damaged by: " + this.name);
        lifeSystem.Damage(damage, attackElement);
        BaseEnemyScript enemy = _enemy.GetComponent<BaseEnemyScript>();
        if (lifeSystem.isDead)
        {
            enemy.StopAllCoroutines();
            enemy.canEnterDamageState = true;
        }
        _enemy.GetComponent<BaseEnemyScript>().ChangeState(BaseEnemyScript.States.DAMAGE);
    }


    void ApplyDamage(Transform _transform)
    {
        if (!IsAttacking) return;
        if ((ownerTransform == null || _transform == ownerTransform)) return;


        if (_transform.CompareTag("Player"))
        {
            DamageToPlayer(_transform);
        }

        if (_transform.CompareTag("Enemy"))
        {
            DamageToEnemy(_transform);
        }
    }

}
