using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageData : MonoBehaviour
{
    [SerializeField] internal Transform ownerTransform;
    [SerializeField] internal float weaponDamage;
    [SerializeField] internal HealthState.Effect weaponEffect = HealthState.Effect.NORMAL;
    [SerializeField] internal bool alwaysAttacking = false;

    internal HealthState customHealthState = null;


    public bool IsAttacking
    {
        get
        {
            if (alwaysAttacking) return true;
            if (ownerTransform == null) return false;

            if(ownerTransform.GetComponent<LifeSystem>().entityType == LifeSystem.EntityType.PLAYER)
            {
                return ownerTransform.GetComponent<PlayerController>().state == PlayerController.PlayerState.ATTACKING;
            }
            else
            {
                return ownerTransform.GetComponent<BaseEnemyScript>().isAttacking;
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!IsAttacking) return;
        if (ownerTransform == null || other.transform == ownerTransform) return;


        if (other.CompareTag("Player"))
        {
            LifeSystem lifeSystem = other.GetComponent<LifeSystem>();
            ApplyDamage(lifeSystem);
            other.GetComponent<PlayerMovement>().DamageStartCorroutine();
        }

        if (other.CompareTag("Enemy"))
        {
            LifeSystem lifeSystem = other.GetComponent<LifeSystem>();
            Debug.Log("Enemy Attacks");
            ApplyDamage(lifeSystem);
            other.GetComponent<BaseEnemyScript>().ChangeState(BaseEnemyScript.States.DAMAGE);
        }

    }


    void ApplyDamage(LifeSystem _lifeSystem)
    {
        if (customHealthState == null)
        {
            _lifeSystem.Damage(weaponDamage, HealthState.GetHealthStateByEffect(weaponEffect, _lifeSystem));
        }
        else
        {
            customHealthState.Init(_lifeSystem);
            _lifeSystem.Damage(weaponDamage, customHealthState);
        }
    }

}
