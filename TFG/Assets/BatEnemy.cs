using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatEnemy : BaseEnemyScript
{
    [Header("Bat Enemy")]
    [SerializeField] protected float attackAnimationTime;
    [SerializeField] protected float attackDamage;
    [SerializeField] Animator enemyAnimator;
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected Transform shootPoint;
    [SerializeField] protected int numOfAttacks = 1;
    [SerializeField] protected float attackSeparationTime = 0.5f;

    float attackTimer;


    internal override void Start_Call() { base.Start_Call(); }

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

        RaycastHit hit;
        bool hitCollided = Physics.Raycast(transform.position, (player.position - transform.position).normalized, out hit, Vector3.Distance(transform.position, player.position), layerMask);
        if (!hitCollided || !hit.transform.CompareTag("Player"))
        {
            ChangeState(States.IDLE);
            return;
        }

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            StartCoroutine(Attack_Cor());
            attackTimer = AttackWait + attackChargingTime;
        }

    }
    internal override void DeathUpdate()
    {
        base.DeathUpdate();
    }

    protected virtual IEnumerator Attack_Cor()
    {
        yield return new WaitForSeconds(attackChargingTime);
        //place shoot animation here
        canRotate = false;
        for (int i = 0; i < numOfAttacks; i++)
        {
            yield return new WaitForSeconds(attackAnimationTime);
            BatProjectile_Tornado projectile = Instantiate(projectilePrefab, shootPoint).GetComponent<BatProjectile_Tornado>();
            projectile.zigzagDir = i % 2 == 0 ? -1 : 1;
            //BatProjectile_Missile projectile = Instantiate(projectilePrefab, shootPoint).GetComponent<BatProjectile_Missile>();
            projectile.Init(transform);
            projectile.transform.SetParent(null);
            projectile.dmgData.damage = attackDamage;
            yield return new WaitForSeconds(attackSeparationTime);
        }
        canRotate = true;

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
        attackTimer = 0f;
        moveDir = Vector3.zero;
        StopRB(stopForce);
        canEnterDamageState = false;
        StartCoroutine(Attack_Cor());
    }


    internal override void IdleExit() { base.IdleExit(); }
    internal override void MoveToTargetExit() { base.MoveToTargetExit(); }
    internal override void AttackExit() { base.AttackExit(); canEnterDamageState = true; }

}
