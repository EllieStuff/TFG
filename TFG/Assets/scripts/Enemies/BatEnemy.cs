using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatEnemy : BaseEnemyScript
{
    enum AnimState { IDLE, ATTACKING, DEAD }

    [Header("Bat Enemy")]
    [SerializeField] protected float attackAnimationTime;
    [SerializeField] protected float attackDamage;
    [SerializeField] Animator enemyAnimator;
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected Transform shootPoint;
    [SerializeField] protected int numOfAttacks = 1;
    [SerializeField] protected float attackSeparationTime = 0.5f;

    protected float attackTimer;

    private bool blockAnim = false;

    AnimState currentAnim = AnimState.IDLE;

    const float RESET_ANIM_TIMER = 3;

    internal override void Start_Call() { base.Start_Call(); }

    internal override void Update_Call() { base.Update_Call(); }

    internal override void FixedUpdate_Call() 
    { 
        base.FixedUpdate_Call();
    }

    private void ChangeAnim(AnimState _state)
    {
        if (!blockAnim || _state.Equals(AnimState.DEAD))
        {
            enemyAnimator.SetInteger("state", (int) _state);
            currentAnim = _state;
        }
    }

    internal override void IdleUpdate()
    {
        ChangeAnim(AnimState.IDLE);

        base.IdleUpdate();
    }
    internal override void MoveToTargetUpdate()
    {
        ChangeAnim(AnimState.IDLE);

        base.MoveToTargetUpdate();
    }
    //internal override void DamageUpdate()
    //{
    //    enemyAnimator.SetFloat("state", 2);
    //    base.DamageUpdate();
    //}
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
            StartCoroutine(ResetAnimCor());
            StartCoroutine(Attack_Cor());
            attackTimer = AttackWait + attackChargingTime;
        }

    }

    internal override void DeathStart()
    {
        base.DeathStart();
        ChangeAnim(AnimState.DEAD);
    }

    internal override void DeathUpdate()
    {
        ChangeAnim(AnimState.DEAD);
        base.DeathUpdate();
    }

    private IEnumerator ResetAnimCor()
    {
        yield return new WaitForSeconds(RESET_ANIM_TIMER);

        if (!currentAnim.Equals(AnimState.DEAD))
        {
            blockAnim = false;
            ChangeAnim(AnimState.IDLE);
        }
    }

    protected virtual IEnumerator Attack_Cor()
    {
        //place shoot animation here
        ChangeAnim(AnimState.ATTACKING);
        blockAnim = true;
        yield return new WaitForSeconds(attackChargingTime);

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

        blockAnim = false;
        ChangeAnim(AnimState.IDLE);

        canRotate = true;
    }
    protected override void EndRndMovesBehaviour()
    {
        base.EndRndMovesBehaviour();

        //float distToPlayer = Vector3.Distance(transform.position, player.position);
        if (canAttack && InAttackRange())
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
        attackTimer = 0f;
        moveDir = Vector3.zero;
        StopRB(stopForce);
        //StartCoroutine(Attack_Cor());
    }


    internal override void IdleExit() { base.IdleExit(); }
    internal override void MoveToTargetExit() { base.MoveToTargetExit(); }
    internal override void AttackExit() { base.AttackExit(); }

}
