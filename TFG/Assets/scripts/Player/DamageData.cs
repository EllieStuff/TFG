using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageData : MonoBehaviour
{
    [SerializeField] internal Transform ownerTransform;
    [SerializeField] internal float weaponDamage;
    //[SerializeField] internal HealthState.Effect weaponEffect = HealthState.Effect.NORMAL;
    [SerializeField] internal HealthState.Effect[] weaponEffects = new HealthState.Effect[1] { HealthState.Effect.NORMAL };
    [SerializeField] internal bool alwaysAttacking = false;
    [SerializeField] bool isACardEffect = false;

    internal HealthState[] customHealthStates = new HealthState[0];


    public bool IsAttacking
    {
        get
        {
            if (isACardEffect || alwaysAttacking) return true;
            if (ownerTransform == null) return false;

            if(ownerTransform.GetComponent<LifeSystem>().entityType == LifeSystem.EntityType.PLAYER)
            {
                return ownerTransform.GetComponent<PlayerSword>().isAttacking;
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
        if (!isACardEffect && (ownerTransform == null || other.transform == ownerTransform)) return;


        if (!isACardEffect && other.CompareTag("Player"))
        {
            LifeSystem lifeSystem = other.GetComponent<LifeSystem>();
            ApplyDamage(lifeSystem);
            other.GetComponent<PlayerMovement>().DamageStartCorroutine();
        }

        if (other.CompareTag("Enemy"))
        {
            LifeSystem lifeSystem = other.GetComponent<LifeSystem>();
            ApplyDamage(lifeSystem);
            other.GetComponent<BaseEnemyScript>().ChangeState(BaseEnemyScript.States.DAMAGE);
        }

    }


    void ApplyDamage(LifeSystem _lifeSystem)
    {
        float tmpWeaponDmg = weaponDamage;
        foreach(HealthState.Effect weaponEffect in weaponEffects)
        {
            _lifeSystem.Damage(tmpWeaponDmg, HealthState.GetHealthStateByEffect(weaponEffect, _lifeSystem));
            tmpWeaponDmg = 0;
        }
        foreach (HealthState customHealthState in customHealthStates)
        {
            _lifeSystem.Damage(0, customHealthState);
        }

    }

}
