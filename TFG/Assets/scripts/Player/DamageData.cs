using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageData : MonoBehaviour
{
    [SerializeField] internal Transform ownerTransform;
    [SerializeField] internal float weaponDamage;
    [SerializeField] internal BaseElements attackElement;
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
        if (!IsAttacking) return;
        if ((ownerTransform == null || other.transform == ownerTransform)) return;


        if (other.CompareTag("Player"))
        {
            DamageToPlayer(other);
        }

        if (other.CompareTag("Enemy"))
        {
            if (audio != null)
                audio.PlaySound();

            LifeSystem lifeSystem = other.GetComponent<LifeSystem>();
            //Debug.Log("Damaged by: " + this.name);
            ApplyDamage(lifeSystem);
            other.GetComponent<BaseEnemyScript>().ChangeState(BaseEnemyScript.States.DAMAGE);
        }

    }

    public void DamageToPlayer(Collider other)
    {
        if (audio != null)
            audio.PlaySound();

        LifeSystem lifeSystem = other.GetComponent<LifeSystem>();
        ApplyDamage(lifeSystem);
        other.GetComponent<PlayerMovement>().DamageStartCorroutine();
    }

    void ApplyDamage(LifeSystem _lifeSystem)
    {
        _lifeSystem.Damage(weaponDamage, attackElement);

        //float tmpWeaponDmg = weaponDamage;
        //foreach(HealthState.Effect weaponEffect in weaponEffects)
        //{
        //    _lifeSystem.Damage(tmpWeaponDmg, HealthState.GetHealthStateByEffect(weaponEffect, _lifeSystem));
        //    tmpWeaponDmg = 0;
        //}
        //foreach (HealthState customHealthState in customHealthStates)
        //{
        //    _lifeSystem.Damage(0, customHealthState);
        //}

    }

}
