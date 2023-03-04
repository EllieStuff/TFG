using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatEnemy : BaseEnemyScript
{
    [Header("DistanceEnemy")]
    [SerializeField] float attackDistance;
    [SerializeField] float baseAttackTimer;
    [SerializeField] float attackAnimationTime;
    [SerializeField] float attackDamage;
    [SerializeField] Animator enemyAnimator;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform shootPoint;

    float attackTimer;


    internal override void Start_Call()
    {
        base.Start_Call(); 
        attackTimer = baseAttackTimer;
        endAttackFlag = false;
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
    internal override void DamageUpdate()
    {
        enemyAnimator.SetFloat("state", 2);
        base.DamageUpdate();
    }
    internal override void AttackUpdate()
    {
        base.AttackUpdate();

        //if (!state.Equals(States.DAMAGE))
        //    rb.velocity = new Vector3(0, rb.velocity.y, 0);

        //enemyAnimator.SetFloat("state", 0);
        //attackTimer -= Time.deltaTime;

        //if (attackTimer <= 0)
        //{
        //    StartCoroutine(AttackCorroutine());
        //    attackTimer = baseAttackTimer;
        //}
    }
    internal override void DeathUpdate()
    {
        base.DeathUpdate();
    }

    IEnumerator AttackCorroutine()
    {
        //place shoot animation here

        yield return new WaitForSeconds(attackAnimationTime);
        ProjectileData projectile = Instantiate(projectilePrefab, shootPoint).GetComponent<ProjectileData>();
        projectile.Init(transform);
        projectile.transform.SetParent(null);

        yield return new WaitForSeconds(0.5f);
        ChangeState(States.IDLE);

    }

    internal override void IdleStart() { base.IdleStart(); }
    internal override void MoveToTargetStart() { base.MoveToTargetStart(); }
    internal override void AttackStart()
    {
        base.AttackStart();
        enemyAnimator.SetFloat("state", 0);
        StartCoroutine(AttackCorroutine());
    }


    internal override void IdleExit() { base.IdleExit(); }
    internal override void MoveToTargetExit() { base.MoveToTargetExit(); }
    internal override void AttackExit() { base.AttackExit(); }

}
