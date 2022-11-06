using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_CacoRato : BaseEnemyScript
{
    //[Header("CacoRato")]
    [SerializeField] float attackDistance;
    [SerializeField] float baseAttackTimer;
    [SerializeField] float attackAnimationTime;
    [SerializeField] float attackDamage;
    [SerializeField] EnemyWeaponHand handWeapon;

    float attackTimer;

    LifeSystem playerLife;

    //PROVISIONAL
    [SerializeField] Animation swordAnim;
    //______________________________

    internal override void Start_Call() { base.Start_Call(); playerLife = player.GetComponent<LifeSystem>(); }

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

        if(Vector3.Distance(player.position, transform.position) > attackDistance)
        {
            Vector3 targetMoveDir = (player.position - transform.position).normalized;
            MoveRB(targetMoveDir, actualMoveSpeed * speedMultiplier);
        }
        else
        {
            attackTimer -= Time.deltaTime;

            if(attackTimer <= 0)
            {
                StartCoroutine(AttackCorroutine());
                attackTimer = baseAttackTimer;
            }
        }
    }

    IEnumerator AttackCorroutine()
    {
        swordAnim.Play();

        yield return new WaitForSeconds(attackAnimationTime);

        if (handWeapon.isTouchingPlayer)
            playerLife.Damage(attackDamage, playerLife.healthState);
    }


    internal override void IdleStart() { base.IdleStart(); }
    internal override void MoveToTargetStart() { base.MoveToTargetStart(); }
    internal override void AttackStart() { base.AttackStart(); }


    internal override void IdleExit() { base.IdleExit(); }
    internal override void MoveToTargetExit() { base.MoveToTargetExit(); }
    internal override void AttackExit() { base.AttackExit(); }


}
