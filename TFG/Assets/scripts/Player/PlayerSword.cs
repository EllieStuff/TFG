using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSword : MonoBehaviour
{
    Rigidbody playerRB;
    //LifeSystem lifeSystem;
    PlayerController playerController;
    const float attackBaseCooldown = 0.5f;
    const float attackBoolTimeEnabled = 0.25f;
    const float disableParticlesTime = 0.5f;
    const float minAttackMovespeed = 1;
    [SerializeField] Animation swordAnim;
    [SerializeField] Collider swordCollider;
    [SerializeField] TrailRenderer swordTrails;

    //
    AudioSource audioSource;
    public AudioClip[] clips;
    //

    internal bool isAttacking;

    internal bool mustAttack;

    [SerializeField] float attackCooldown;

    void Start()
    {
        swordTrails.enabled = false;
        playerRB = GetComponent<Rigidbody>();
        playerController = GetComponent<PlayerController>();
        audioSource = GetComponent<AudioSource>();
        //lifeSystem = GetComponent<LifeSystem>();

        swordCollider.enabled = false;

    }

    void FixedUpdate()
    {
        if (CanAttackWithSword())
            AttackWithSword();
        else if (attackCooldown > 0)
            attackCooldown -= Time.deltaTime;
        else if (playerController.StateEquals(PlayerController.PlayerState.ATTACKING))
            playerController.ChangeState(PlayerController.PlayerState.NORMAL);
    }

    void AttackWithSword()
    {
        swordAnim.Play();

        //
        AudioClip chosenClip = AudioManager.ChoseClip(clips);
        audioSource.PlayOneShot(chosenClip);
        //

        attackCooldown = attackBaseCooldown;
        isAttacking = true;
        swordCollider.enabled = true;
        StartCoroutine(AttackCorroutine());
    }

    bool CanAttackWithSword()
    {
        //modificar perquè sigui automàtic

        if (mustAttack && playerRB.velocity.magnitude <= minAttackMovespeed && attackCooldown <= 0 && playerController.StateEquals(PlayerController.PlayerState.NORMAL))
        {
            mustAttack = false;
            return true;
        }

        return false;
    }

    IEnumerator AttackCorroutine()
    {
        swordTrails.enabled = true;
        yield return new WaitForSeconds(attackBoolTimeEnabled);

        isAttacking = false;
        swordCollider.enabled = false;

        yield return new WaitForSeconds(disableParticlesTime);

        swordTrails.enabled = false;

        yield return 0;
    }


}
