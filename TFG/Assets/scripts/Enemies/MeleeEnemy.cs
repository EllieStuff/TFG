using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : BaseEnemyScript
{
    enum AnimState { IDLE, MOVING, ATTACKING, RESTING, DAMAGED }

    [Header("MeleeEnemy")]
    //[SerializeField] Transform shieldPivotRef; 
    //[SerializeField] Transform idleShieldPoint, attackingShieldPoint;
    //[SerializeField] internal bool hasShield = true;
    [SerializeField] float attackDmg = 15f;
    [SerializeField] float attackForce = 10.0f;
    [SerializeField] float attackDuration = 1.0f;
    [SerializeField] Vector3 atkVelocityLimit = new Vector3(20, 0, 20);
    [SerializeField] Animator enemyAnimator;
    
    Vector3 attackMoveDir = Vector3.zero;


    internal override void Start_Call()
    {
        base.Start_Call();
        endAttackFlag = false;
    }

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
            //canRotate = Vector3.Angle(transform.forward, attackMoveDir) < 1f;
            MoveRB(attackMoveDir, attackForce);
        }

        //if (isAttacking)
        //{
        //    enemyAnimator.SetFloat("state", 1);
        //    MoveRB(attackMoveDir, attackForce);
        //}
        //else
        //{
        //    enemyAnimator.SetFloat("state", 0);
        //}
    }


    internal override void IdleStart()
    {
        base.IdleStart();
        enemyAnimator.SetFloat("state", (int)AnimState.IDLE);
    }
    internal override void RandomMovementStart()
    {
        base.RandomMovementStart();
        enemyAnimator.SetFloat("state", (int)AnimState.MOVING);
    }
    internal override void MoveToTargetStart()
    {
        base.MoveToTargetStart();
        enemyAnimator.SetFloat("state", (int)AnimState.MOVING);
    }
    internal override void AttackStart()
    {
        base.AttackStart();
        SetVelocityLimit(-atkVelocityLimit, atkVelocityLimit);
        canEnterDamageState = false;
        moveDir = Vector3.zero;
        StartCoroutine(AttackCoroutine());
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
        enemyAnimator.SetFloat("state", (int)AnimState.IDLE);
        yield return new WaitForSeconds(0.5f);
        //Feedback
        yield return new WaitForSeconds(0.2f);

        // Attacks
        touchBodyDamageData.StopAllCoroutines();
        touchBodyDamageData.damage = attackDmg;
        touchBodyDamageData.disabled = false;
        canMove = isAttacking = true;
        //canRotate = false;
        attackMoveDir = (player.position - transform.position).normalized;
        //moveDir = attackMoveDir;
        enemyAnimator.SetFloat("state", (int)AnimState.ATTACKING);
        yield return new WaitForSeconds(attackDuration);

        // Ends Attack
        enemyAnimator.SetFloat("state", (int)AnimState.IDLE);
        canMove = isAttacking = false;
        StopRB(4.0f);
        yield return new WaitForSeconds(0.2f);
        touchBodyDamageData.damage = dmgOnTouch;
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
