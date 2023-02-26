using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageData : MonoBehaviour
{
    [SerializeField] internal Transform ownerTransform;
    [SerializeField] internal float damage = 10;
    [SerializeField] internal ElementsManager.Elements attackElement;
    [SerializeField] internal bool alwaysAttacking = false;
    [SerializeField] internal float timeDisabledAfterColl = -1f;
    [SerializeField] List<string> tagsAffected;

    [SerializeField] AudioManager audio;

    internal bool disabled = false;


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
        if(tagsAffected.FindIndex(_tag => other.transform.CompareTag(_tag)) >= 0)
            ApplyDamage(other.transform);
    }

    private void OnCollisionEnter(Collision col)
    {
        if (tagsAffected.FindIndex(_tag => col.transform.CompareTag(_tag)) >= 0)
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
        if (disabled) return;
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

        if (timeDisabledAfterColl > 0f) StartCoroutine(DisableDuringTime());
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
