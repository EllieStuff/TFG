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
    [SerializeField] int numOfAttacks = 1;

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

        float distToPlayer = Vector3.Distance(transform.position, player.position);
        if (!canAttack || distToPlayer > enemyStartAttackDistance)
        {
            ChangeState(States.IDLE);
            return;
        }
        else
        {
            RaycastHit hit;
            bool hitCollided = Physics.Raycast(transform.position, (player.position - transform.position).normalized, out hit, distToPlayer, layerMask);
            if (!hitCollided || !hit.transform.CompareTag("Player"))
            {
                ChangeState(States.IDLE);
                return;
            }
        }

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            StartCoroutine(AttackCorroutine());
            attackTimer = baseAttackTimer;
        }
    }
    internal override void DeathUpdate()
    {
        base.DeathUpdate();
    }

    IEnumerator AttackCorroutine()
    {
        yield return new WaitForSeconds(1f);
        //place shoot animation here
        for (int i = 0; i < numOfAttacks; i++)
        {
            yield return new WaitForSeconds(attackAnimationTime);
            ProjectileData projectile = Instantiate(projectilePrefab, shootPoint).GetComponent<ProjectileData>();
            projectile.Init(transform);
            projectile.transform.SetParent(null);
        }

    }
    protected override void EndRndMovesBehaviour()
    {
        base.EndRndMovesBehaviour();

        float distToPlayer = Vector3.Distance(transform.position, player.position);
        if (canAttack && distToPlayer <= enemyStartAttackDistance)
        {
            ChangeState(States.ATTACK);

            //RaycastHit hit;
            //bool hitCollided = Physics.Raycast(transform.position, (player.position - transform.position).normalized, out hit, distToPlayer, layerMask);
            //if (hitCollided && hit.transform.CompareTag("Player"))
            //{
            //    ChangeState(States.ATTACK);
            //}
        }
    }

    internal override void IdleStart() { base.IdleStart(); }
    internal override void MoveToTargetStart() { base.MoveToTargetStart(); }
    internal override void AttackStart()
    {
        base.AttackStart();
        enemyAnimator.SetFloat("state", 0);
        moveDir = Vector3.zero;
        StopRB(stopForce);
        StartCoroutine(AttackCorroutine());
    }


    internal override void IdleExit() { base.IdleExit(); }
    internal override void MoveToTargetExit() { base.MoveToTargetExit(); }
    internal override void AttackExit() { base.AttackExit(); }

}
