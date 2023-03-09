using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantEnemy : BaseEnemyScript
{
    enum AnimState { IDLE, MOVING, ATTACKING, RESTING, DEAD }
    enum AttackType { NORMAL_THROW, CIRCLE_ATTACK, FOUR_PROJECTILES, THREE_PROJECTILES }


    [Header("PlantEnemy")]
    [SerializeField] float attackAnimationTime;
    [SerializeField] float attackDamage;
    [SerializeField] Animator enemyAnimator;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform shootPoint;
    [SerializeField] AttackType attackStyle;
    //[SerializeField] bool testPlant = false;

    float attackTimer;

    const int CIRCLE_ITERATIONS = 24;
    const int CIRCLE_MULTIPLIER = 50;

    internal override void Start_Call() { base.Start_Call(); }

    internal override void Update_Call() { base.Update_Call(); }

    internal override void FixedUpdate_Call() { base.FixedUpdate_Call(); }


    internal override void IdleUpdate()
    {
        enemyAnimator.SetFloat("state", (int)AnimState.IDLE);
        base.IdleUpdate();
    }
    internal override void MoveToTargetUpdate()
    {
        enemyAnimator.SetFloat("state", (int)AnimState.MOVING);
        base.MoveToTargetUpdate();
    }
    internal override void DamageUpdate()
    {
        //enemyAnimator.SetFloat("state", (int)AnimState.IDLE);
        base.DamageUpdate();
    }
    internal override void AttackUpdate()
    {
        base.AttackUpdate();

        if (!state.Equals(States.DAMAGE))
            rb.velocity = new Vector3(0, rb.velocity.y, 0);

        enemyAnimator.SetFloat("state", (int)AnimState.IDLE);
        attackTimer -= Time.fixedDeltaTime;

        if (attackTimer <= 0)
        {
            StartCoroutine(AttackCorroutine());
            attackTimer = AttackWait + attackChargingTime;
        }

    }
    internal override void DeathUpdate()
    {
        base.DeathUpdate();
    }

    IEnumerator AttackCorroutine()
    {
        yield return new WaitForSeconds(attackChargingTime);

        //place shoot animation here
        enemyAnimator.SetFloat("state", (int)AnimState.ATTACKING);
        yield return new WaitForSeconds(attackAnimationTime);
        RaycastHit hit;
        if (Physics.Raycast(shootPoint.position, (player.position - shootPoint.position).normalized, out hit))
        {
            if (!hit.transform.CompareTag("Wall"))
                AttackStateMachine(attackStyle);
        }
    }

    void AttackStateMachine(AttackType type)
    {
        ProjectileData projectile;
        switch (type)
        {
            case AttackType.FOUR_PROJECTILES:
                for (int knifeDirState = 0; knifeDirState < 4; knifeDirState++)
                {
                    projectile = Instantiate(projectilePrefab, transform).GetComponent<ProjectileData>();
                    projectile.Init(transform);
                    projectile.transform.SetParent(null);
                    switch (knifeDirState)
                    {
                        case 0:
                            projectile.moveDir = new Vector3(1, 0, 0);
                            break;
                        case 1:
                            projectile.moveDir = new Vector3(-1, 0, 0);
                            break;
                        case 2:
                            projectile.moveDir = new Vector3(0, 0, 1);
                            break;
                        case 3:
                            projectile.moveDir = new Vector3(0, 0, -1);
                            break;
                    }
                }
                break;
            case AttackType.CIRCLE_ATTACK:
                float circleY = 0.1f;
                float circleX = 0.1f;
                float auxTimer = 0;
                for (int i = 0; i < CIRCLE_ITERATIONS; i++)
                {
                    circleY = Mathf.Sin(auxTimer);
                    circleX = Mathf.Cos(auxTimer);
                    auxTimer += Time.deltaTime * CIRCLE_MULTIPLIER;
                    KnifeThrown knife_2 = Instantiate(projectilePrefab, transform).GetComponent<KnifeThrown>();
                    knife_2.knifeDir = new Vector3(circleX, 0, circleY);
                    knife_2.SetOwnerTransform(transform);
                }
                break;
            case AttackType.THREE_PROJECTILES:
                for (int direction = 0; direction < 3; direction++)
                {
                    KnifeThrown knife_3 = Instantiate(projectilePrefab, transform).GetComponent<KnifeThrown>();
                    switch (direction)
                    {
                        case 0:
                            knife_3.knifeDir = new Vector3(1, 0, 1);
                            break;
                        case 1:
                            knife_3.knifeDir = new Vector3(-1, 0, 1);
                            break;
                        case 2:
                            knife_3.knifeDir = new Vector3(0, 0, 1);
                            break;
                    }
                    knife_3.SetOwnerTransform(transform);
                    knife_3.localDir = true;
                    knife_3.entityThrowingIt = transform;
                }
                break;
            case AttackType.NORMAL_THROW:
                projectile = Instantiate(projectilePrefab, shootPoint).GetComponent<ProjectileData>();
                projectile.Init(transform);
                projectile.transform.SetParent(null);
                //projectile.moveDir = (player.position - transform.position).normalized;
                break;
        }
    }

    internal override void IdleStart() { base.IdleStart(); enemyAnimator.SetFloat("state", (int)AnimState.IDLE); }
    internal override void MoveToTargetStart() { base.MoveToTargetStart(); }
    internal override void AttackStart() { base.AttackStart(); attackTimer = 0f; canEnterDamageState = false; }


    internal override void IdleExit() { base.IdleExit(); }
    internal override void MoveToTargetExit() { base.MoveToTargetExit(); }
    internal override void AttackExit() { base.AttackExit(); canEnterDamageState = true; }


}
