using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageData : MonoBehaviour
{
    public enum DamageBehaviour { ON_ENTER, ON_STAY, ON_EXIT }

    [SerializeField] internal Transform ownerTransform;
    [SerializeField] internal DamageBehaviour dmgBehaviour = DamageBehaviour.ON_ENTER;
    [SerializeField] internal float damage = 10;
    [SerializeField] internal ElementsManager.Elements attackElement;
    [SerializeField] internal bool alwaysAttacking = false;
    [SerializeField] internal float timeDisabledAfterColl = -1f;
    [SerializeField] internal float stealLifePercentage = 0;
    [SerializeField] internal float critPercentage = 0;
    [SerializeField] List<string> tagsAffected;

    [SerializeField] AudioManager audio;

    private LifeSystem playerLifeSystem;

    internal bool disabled = false;
    float dmgVariation = 0.02f;

    const float DAMAGE_CRIT_MULTIPLIER = 0.25f;

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

    public bool CanApplyDamage(Transform _otherTransform)
    {
        return !disabled && IsAttacking && !(ownerTransform == null || _otherTransform == ownerTransform)
            && tagsAffected.FindIndex(_tag => _otherTransform.CompareTag(_tag)) >= 0;
    }


    private void OnTriggerEnter(Collider other)
    {
        if(dmgBehaviour == DamageBehaviour.ON_ENTER && CanApplyDamage(other.transform))
            ApplyDamage(other.transform);
    }
    private void OnTriggerStay(Collider other)
    {
        if (dmgBehaviour == DamageBehaviour.ON_STAY && CanApplyDamage(other.transform))
            ApplyDamage(other.transform);
    }
    private void OnTriggerExit(Collider other)
    {
        if (dmgBehaviour == DamageBehaviour.ON_EXIT && CanApplyDamage(other.transform))
            ApplyDamage(other.transform);
    }

    private void OnCollisionEnter(Collision col)
    {
        if (dmgBehaviour == DamageBehaviour.ON_ENTER && CanApplyDamage(col.transform))
            ApplyDamage(col.transform);
    }
    private void OnCollisionStay(Collision col)
    {
        if (dmgBehaviour == DamageBehaviour.ON_STAY && CanApplyDamage(col.transform))
            ApplyDamage(col.transform);
    }
    private void OnCollisionExit(Collision col)
    {
        if (dmgBehaviour == DamageBehaviour.ON_EXIT && CanApplyDamage(col.transform))
            ApplyDamage(col.transform);
    }



    void DamageToPlayer(Transform _player)
    {
        if (audio != null)
            audio.PlaySound();

        LifeSystem lifeSystem = _player.GetComponent<LifeSystem>();
        lifeSystem.Damage(damage + GetDamageVariation(), attackElement);
        _player.GetComponent<PlayerMovement>().DamageStartCorroutine();
    }

    void DamageToEnemy(Transform _enemy)
    {
        if (critPercentage > 0)
            damage = damage + GetDamageVariation() + (damage * DAMAGE_CRIT_MULTIPLIER);

        if (audio != null)
            audio.PlaySound();

        if (playerLifeSystem == null)
            playerLifeSystem = GameObject.FindGameObjectWithTag("Player").GetComponent<LifeSystem>();

        PlayerProjectileData dataProj = GetComponent<PlayerProjectileData>();
        LifeSystem lifeSystem = _enemy.GetComponent<LifeSystem>();
        //Debug.Log("Damaged by: " + this.name);

        //if (dataProj.dmgData.stealLifePercentage > 0)
        //    lifeSystem.DamageWithLifeSteal(damage + GetDamageVariation(), attackElement, dataProj, playerLifeSystem);
        //else
        //    lifeSystem.Damage(damage + GetDamageVariation(), attackElement);

        lifeSystem.Damage(damage + GetDamageVariation(), attackElement);
        if (dataProj.dmgData.stealLifePercentage > 0 && lifeSystem.isDead)
            lifeSystem.LifeSteal(attackElement, dataProj, playerLifeSystem);

        if (critPercentage > 0)
            lifeSystem.CritFeedback();

        BaseEnemyScript enemy = _enemy.GetComponent<BaseEnemyScript>();
        if (lifeSystem.isDead)
        {
            enemy.StopAllCoroutines();
            enemy.canEnterDamageState = true;
        }
        _enemy.GetComponent<BaseEnemyScript>().ActivateDamage();
    }


    void ApplyDamage(Transform _transform)
    {
        if (_transform.CompareTag("Player"))
        {
            DamageToPlayer(_transform);
        }

        if (_transform.CompareTag("Enemy"))
        {
            DamageToEnemy(_transform);
        }

        if (timeDisabledAfterColl > 0f) StartCoroutine(DisableDuringTime());
    }


    float GetDamageVariation()
    {
        return (damage * Random.Range(-dmgVariation, dmgVariation));
    }


    IEnumerator DisableDuringTime()
    {
        disabled = true;
        float disabledTimer = 0;
        while(disabledTimer < timeDisabledAfterColl)
        {
            yield return new WaitForEndOfFrame();
            disabledTimer += Time.deltaTime;
        }
        disabled = false;
    }

}
