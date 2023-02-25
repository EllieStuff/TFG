using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Ragloton : BaseEnemyScript
{
    [Header("Ragloton")]
    //[SerializeField] Transform shieldPivotRef; 
    //[SerializeField] Transform idleShieldPoint, attackingShieldPoint;
    //[SerializeField] internal bool hasShield = true;
    [SerializeField] float attackForce = 10.0f, attackDuration = 1.0f;
    [SerializeField] Vector3 atkVelocityLimit = new Vector3(20, 0, 20);
    [SerializeField] Animator enemyAnimator;
    
    Vector3 attackMoveDir = Vector3.zero;


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
    internal override void AttackUpdate()
    {
        enemyAnimator.SetFloat("state", 0);
        base.AttackUpdate();

        if (isAttacking)
        {
            enemyAnimator.SetFloat("state", 1);
            MoveRB(attackMoveDir, attackForce);
        }
        else
        {
            enemyAnimator.SetFloat("state", 0);
        }
    }


    internal override void IdleStart() { base.IdleStart(); }
    internal override void MoveToTargetStart() { base.MoveToTargetStart(); }
    internal override void AttackStart()
    { 
        base.AttackStart();
        SetVelocityLimit(-atkVelocityLimit, atkVelocityLimit);
        StartCoroutine(AttackCoroutine());
        //ToDo:
        // - Potser fer que l'escut tingui un tag default i quan acabi canviar-lo a EnemyWeapon?
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
        yield return new WaitForSeconds(0.5f);
        //Feedback
        yield return new WaitForSeconds(0.2f);

        // Attacks
        canMove = isAttacking = true;
        canRotate = false;
        attackMoveDir = (player.position - transform.position).normalized;
        enemyAnimator.SetFloat("state", 1);
        yield return new WaitForSeconds(attackDuration);

        // Ends Attack
        enemyAnimator.SetFloat("state", 0);
        canMove = isAttacking = false;
        StopRB(4.0f);
        yield return new WaitForSeconds(1.0f);
        //Feedback
        yield return new WaitForSeconds(0.5f);
        canMove = canRotate = true;

        if(state.Equals(States.DAMAGE))
        {
            base.damageTimer = baseDamageTimer;
        }

        ChangeState(States.IDLE);
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
