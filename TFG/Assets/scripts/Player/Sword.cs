using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    Rigidbody playerRB;
    PlayerLifeSystem playerStatus;
    const float attackBaseCooldown = 0.5f;
    const float attackBoolTimeEnabled = 0.25f;
    [SerializeField] internal float attackDistance = 2;

    internal bool isAttacking;

    bool isHoldingTheKey = false;

    [SerializeField] float attackCooldown;

    void Start()
    {
        playerRB = GetComponent<Rigidbody>();
        playerStatus = GetComponent<PlayerLifeSystem>();
    }

    void FixedUpdate()
    {
        if (CanAttackWithSword())
            AttackWithSword();
        else if (attackCooldown > 0)
            attackCooldown -= Time.deltaTime;
        else if (playerStatus.playerState == PlayerLifeSystem.playerStates.ATTACKING)
            playerStatus.playerState = PlayerLifeSystem.playerStates.NORMAL;
    }

    void AttackWithSword()
    {
        attackCooldown = attackBaseCooldown;
        isAttacking = true;
        StartCoroutine(AttackCorroutine());
    }

    bool CanAttackWithSword()
    {
        if (!isHoldingTheKey && Input.GetKey(KeyCode.Mouse0) && attackCooldown <= 0 && playerStatus.playerState == PlayerLifeSystem.playerStates.NORMAL)
        {
            isHoldingTheKey = true;
            return true;
        }
        else if(isHoldingTheKey && !Input.GetKey(KeyCode.Mouse0))
            isHoldingTheKey = false;

        return false;
    }

    IEnumerator AttackCorroutine()
    {
        yield return new WaitForSeconds(attackBoolTimeEnabled);

        isAttacking = false;

        yield return 0;
    }
}
