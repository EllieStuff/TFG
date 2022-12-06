using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_CacoRato_Melee : BaseEnemyScript
{
    //[Header("CacoRato")]
    [SerializeField] float attackDistance;
    [SerializeField] float baseAttackTimer;
    [SerializeField] float attackAnimationTime;
    [SerializeField] float attackDamage;
    [SerializeField] EnemyWeaponHand handWeapon;
    [SerializeField] Animator enemyAnimator;
    [SerializeField] TrailRenderer trailsEffect;

    float attackTimer;

    LifeSystem playerLife;

    [SerializeField] Animation swordAnim;

    internal override void Start_Call()
    {
        base.Start_Call();
        playerLife = player.GetComponent<LifeSystem>();
        attackTimer = baseAttackTimer / 2f;
    }

    internal override void Update_Call() { base.Update_Call(); }

    internal override void FixedUpdate_Call() { base.FixedUpdate_Call(); }


    internal override void IdleUpdate()
    {
        enemyAnimator.SetFloat("state", 0);
        base.IdleUpdate();
    }
    internal override void MoveToTargetUpdate()
    {
        enemyAnimator.SetFloat("state", 1);
        base.MoveToTargetUpdate();

    }
    internal override void AttackUpdate()
    {
        base.AttackUpdate();

        if (Vector3.Distance(player.position, transform.position) > attackDistance)
        {
            Vector3 targetMoveDir = (player.position - transform.position).normalized;
            MoveRB(targetMoveDir, actualMoveSpeed * speedMultiplier);
        }
        else
        {
            enemyAnimator.SetFloat("state", 0);

            rb.velocity = Vector3.zero;

            attackTimer -= Time.fixedDeltaTime;

            if(attackTimer <= 0)
            {
                StartCoroutine(AttackCorroutine());
                attackTimer = baseAttackTimer;
            }
        }
    }

    internal override void DamageUpdate()
    {
        enemyAnimator.SetFloat("state", 2);
        base.DamageUpdate();
    }

    IEnumerator AttackCorroutine()
    {
        swordAnim.Play();

        trailsEffect.enabled = true;
        isAttacking = true;

        yield return new WaitForSeconds(attackAnimationTime);

        isAttacking = false;
        trailsEffect.enabled = false;
    }


    internal override void IdleStart() { base.IdleStart(); }
    internal override void MoveToTargetStart() { base.MoveToTargetStart(); }
    internal override void AttackStart() { base.AttackStart(); }


    internal override void IdleExit() { base.IdleExit(); }
    internal override void MoveToTargetExit() { base.MoveToTargetExit(); }
    internal override void AttackExit() { base.AttackExit(); }


}
