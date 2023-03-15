using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatBossEnemy : BatEnemy
{
    [Header("BatBoss Enemy")]
    [SerializeField] float changeElementDelay = 1.5f;
    [SerializeField] float flashDuration = 0.2f, changePhaseDelay = 3f;
    [SerializeField] int secondPhaseStartLife = 3000;
    [SerializeField] float secondPhaseAttackMultiplier = 1.3f, secondPhaseWaitMultiplier = 0.7f, secondPhaseProjSizeMultiplier = 1.3f, secondPhaseProjSpeedMultiplier = 1.2f;
    [SerializeField] ParticleSystem changeToFireParticles, changeToGrassParticles, changeToWaterParticles;
    [SerializeField] Material fireMat, fireTransparentMat, grassMat, grassTransparentMat, waterMat, waterTransparentMat;
    [SerializeField] Image flashImage;
    [SerializeField] Color fireFlashColor = Color.red, grassFlashColor = Color.green, waterFlashColor = Color.cyan;

    CameraShake camShake;
    List<BatProjectile_Missile> projectiles = new List<BatProjectile_Missile>();
    bool secondPhaseEntered = false;
    float projectileSizeMultiplier = 1f, projectileSpeedMultiplier = 1f;

    internal override void Start_Call()
    {
        base.Start_Call();
        camShake = Camera.main.GetComponent<CameraShake>();
    }

    internal override void Update_Call()
    {
        base.Update_Call();
        if(enemyLife.currLife <= secondPhaseStartLife && !secondPhaseEntered && !isAttacking)
        {
            secondPhaseEntered = true;
            //canAttack = false;
            attackTimer = 99999;
            StartCoroutine(ChangePhase_Cor());
        }
    }

    internal override void DeathStart()
    {
        base.DeathStart();
        for (int i = 0; i < projectiles.Count; i++) projectiles[i].DestroyObject();
        projectiles.Clear();
    }


    IEnumerator ChangePhase_Cor()
    {
        canRotate = false;
        attackDamage *= secondPhaseAttackMultiplier;
        attackWait *= secondPhaseWaitMultiplier;
        attackChargingTime *= secondPhaseWaitMultiplier;
        projectileSizeMultiplier *= secondPhaseProjSizeMultiplier;
        projectileSpeedMultiplier *= secondPhaseProjSpeedMultiplier;
        //ToDo: Posar particules enfadament
        StartCoroutine(GetComponent<EnemyShake>().Shake(changePhaseDelay, 0.03f, 0.03f));
        yield return camShake.ShakeCamera(changePhaseDelay, 0.3f);
        canAttack = canRotate = true;
        attackTimer = 0.1f;
    }

    protected override IEnumerator Attack_Cor()
    {
        isAttacking = true;
        yield return ChangeElement_Cor();
        yield return new WaitForSeconds(attackChargingTime);
        //place shoot animation here
        for (int i = 0; i < numOfAttacks; i++)
        {
            yield return new WaitForSeconds(attackAnimationTime);
            BatProjectile_Missile projectile = Instantiate(projectilePrefab, shootPoint).GetComponent<BatProjectile_Missile>();
            projectile.Init(transform);
            projectile.transform.SetParent(null);
            projectile.dmgData.damage = attackDamage;
            projectile.transform.localScale *= projectileSizeMultiplier;
            projectile.moveSpeed *= projectileSpeedMultiplier;
            projectiles.Add(projectile);
            yield return new WaitForSeconds(attackSeparationTime);
        }
        isAttacking = false;
    }


    IEnumerator ChangeElement_Cor()
    {
        ElementsManager.Elements newElement = (ElementsManager.Elements)Random.Range(0, (int)ElementsManager.Elements.COUNT - 1);
        while(newElement == enemyLife.entityElement) 
            newElement = (ElementsManager.Elements)Random.Range(0, (int)ElementsManager.Elements.COUNT - 1);

        PlayChangeElementParticles(newElement);
        StartCoroutine(camShake.ShakeCamera(changeElementDelay, 0.02f));
        yield return new WaitForSeconds(changeElementDelay);
        flashImage.gameObject.SetActive(true);
        StartCoroutine(camShake.ShakeCamera(flashDuration * 2f, 0.1f));
        EliTween.ChangeColor(flashImage, GetFlashColor(newElement), flashDuration);
        yield return new WaitForSeconds(flashDuration);
        ChangeElementMaterials(newElement);
        enemyLife.entityElement = newElement;
        for (int i = 0; i < projectiles.Count; i++) projectiles[i].DestroyObject();
        projectiles.Clear();
        projectiles = new List<BatProjectile_Missile>();
        EliTween.ChangeColor(flashImage, Color.clear, flashDuration);
        yield return new WaitForSeconds(flashDuration);
        flashImage.gameObject.SetActive(false);
    }


    Color GetFlashColor(ElementsManager.Elements _element)
    {
        switch (_element)
        {
            case ElementsManager.Elements.FIRE:
                return fireFlashColor;

            case ElementsManager.Elements.GRASS:
                return grassFlashColor;

            case ElementsManager.Elements.WATER:
                return waterFlashColor;

            default:
                Debug.LogWarning("Flash color not found");
                return Color.black;
        }
    }

    void PlayChangeElementParticles(ElementsManager.Elements _element)
    {
        switch (_element)
        {
            case ElementsManager.Elements.FIRE:
                if (changeToFireParticles != null) changeToFireParticles.Play();
                else Debug.LogWarning("ChangeToFireParticles was not assigned");
                break;

            case ElementsManager.Elements.GRASS:
                if (changeToGrassParticles != null) changeToGrassParticles.Play();
                else Debug.LogWarning("ChangeToGrassParticles was not assigned");
                break;

            case ElementsManager.Elements.WATER:
                if (changeToWaterParticles != null) changeToWaterParticles.Play();
                else Debug.LogWarning("ChangeToWaterParticles was not assigned");
                break;

            default:
                break;
        }
    }

    void ChangeElementMaterials(ElementsManager.Elements _element)
    {
        switch (_element)
        {
            case ElementsManager.Elements.FIRE:
                if (enemyMesh != null) enemyMesh.material = fireMat;
                else enemyMeshTmp.material = fireMat;
                transparentMat = fireTransparentMat;
                break;

            case ElementsManager.Elements.GRASS:
                if (enemyMesh != null) enemyMesh.material = grassMat;
                else enemyMeshTmp.material = grassMat;
                transparentMat = grassTransparentMat;
                break;

            case ElementsManager.Elements.WATER:
                if (enemyMesh != null) enemyMesh.material = waterMat;
                else enemyMeshTmp.material = waterMat;
                transparentMat = waterTransparentMat;
                break;

            default:
                break;
        }
    }

}
