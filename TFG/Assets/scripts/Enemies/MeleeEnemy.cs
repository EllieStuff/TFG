using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : BaseEnemyScript
{
    enum AnimState { IDLE, MOVING, ATTACKING, RESTING, DEAD }

    [Header("MeleeEnemy")]
    [SerializeField] float attackDmg = 15f;
    [SerializeField] float attackForce = 10.0f;
    [SerializeField] float attackDuration = 1.0f;
    [SerializeField] Vector3 atkVelocityLimit = new Vector3(20, 0, 20);
    [SerializeField] Animator enemyAnimator;
    
    Vector3 attackMoveDir = Vector3.zero;


    internal override void Start_Call() { base.Start_Call(); }

    internal override void Update_Call() { base.Update_Call(); }

    internal override void FixedUpdate_Call() { base.FixedUpdate_Call(); }


    internal override void IdleUpdate() 
    {
        base.IdleUpdate(); 
    }
    internal override void MoveToTargetUpdate() 
    {
        base.MoveToTargetUpdate(); 
    }
    internal override void AttackUpdate()
    {
        base.AttackUpdate();

        if (canRotate && isAttacking && Vector3.Angle(transform.forward, attackMoveDir) < 1f) canRotate = false;
        if (isAttacking)
        {
            attackMoveDir = new Vector3(attackMoveDir.x, 0, attackMoveDir.z);
            MoveRB(attackMoveDir, attackForce);
        }

    }


    internal override void IdleStart()
    {
        base.IdleStart();
        enemyAnimator.SetInteger("state", (int)AnimState.IDLE);
    }
    internal override void RandomMovementStart()
    {
        base.RandomMovementStart();
        enemyAnimator.SetInteger("state", (int)AnimState.MOVING);
    }
    internal override void MoveToTargetStart()
    {
        base.MoveToTargetStart();
        enemyAnimator.SetInteger("state", (int)AnimState.MOVING);
    }
    internal override void AttackStart()
    {
        base.AttackStart();
        SetVelocityLimit(-atkVelocityLimit, atkVelocityLimit);
        canEnterDamageState = false;
        moveDir = Vector3.zero;
        StartCoroutine(AttackCoroutine());
    }
    internal override void DeathStart()
    {
        enemyAnimator.SetInteger("state", (int)AnimState.DEAD);
        base.DeathStart();
    }


    internal override void IdleExit() { base.IdleExit(); }
    internal override void MoveToTargetExit() { base.MoveToTargetExit(); }
    internal override void AttackExit()
    { 
        base.AttackExit();
        SetVelocityLimit(baseMinVelocity, baseMaxVelocity);
        isAttacking = false;
    }

    IEnumerator AttackCoroutine()
    {
        // Prepares For Attack
        canMove = false;
        StopRB(2.0f);
        yield return new WaitForSeconds(0.2f);
        //Feedback
        enemyAnimator.SetInteger("state", (int)AnimState.IDLE);
        yield return new WaitForSeconds(attackChargingTime);
        //Feedback
        yield return new WaitForSeconds(0.2f);

        // Attacks
        touchBodyDamageData.StopAllCoroutines();
        touchBodyDamageData.damage = attackDmg;
        touchBodyDamageData.disabled = false;
        canMove = isAttacking = true;
        yield return null;
        touchBodyDamageData.dmgBehaviour = DamageData.DamageBehaviour.ON_ENTER;
        //canRotate = false;
        //attackMoveDir = (player.position - transform.position).normalized;
        attackMoveDir = transform.forward;
        moveDir = attackMoveDir;
        enemyAnimator.SetInteger("state", (int)AnimState.ATTACKING);
        yield return new WaitForSeconds(attackDuration);

        // Ends Attack
        enemyAnimator.SetInteger("state", (int)AnimState.RESTING);
        canMove = isAttacking = false;
        StopRB(stopForce);
        yield return new WaitForSeconds(0.2f);
        touchBodyDamageData.damage = dmgOnTouch;
        touchBodyDamageData.dmgBehaviour = DamageData.DamageBehaviour.ON_STAY;
        canEnterDamageState = true;
        ChangeState(States.REST);
        
        //canMove = canRotate = true;

        //if(state.Equals(States.DAMAGE))
        //{
        //    base.damageTimer = baseDamageTimer;
        //}

    }


    //IEnumerator LerpTransformsPosition(Transform _affectedTrans, Transform _initPointTrans, Transform _targetPointTrans, float _lerpDuration = 0.5f)
    //{
    //    float timer = 0, maxTime = _lerpDuration;
    //    while (timer < maxTime)
    //    {
    //        yield return new WaitForEndOfFrame();
    //        timer += Time.deltaTime;
    //        _affectedTrans.position = Vector3.Lerp(_initPointTrans.position, _targetPointTrans.position, timer / maxTime);
    //    }
    //    yield return new WaitForEndOfFrame();
    //    _affectedTrans.position = _targetPointTrans.position;
    //}

}
