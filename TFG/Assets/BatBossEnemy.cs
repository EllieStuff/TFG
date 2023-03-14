using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatBossEnemy : BatEnemy
{
    [Header("BatBoss Enemy")]
    [SerializeField] float changeElementDelay = 1.5f;
    [SerializeField] ParticleSystem changeToFireParticles, changeToGrassParticles, changeToWaterParticles;
    [SerializeField] Material fireMat, fireTransparentMat, grassMat, grassTransparentMat, waterMat, waterTransparentMat;

    protected override IEnumerator Attack_Cor()
    {
        yield return ChangeElement_Cor();
        yield return new WaitForSeconds(attackChargingTime);
        //place shoot animation here
        canRotate = false;
        for (int i = 0; i < numOfAttacks; i++)
        {
            yield return new WaitForSeconds(attackAnimationTime);
            BatProjectile_Missile projectile = Instantiate(projectilePrefab, shootPoint).GetComponent<BatProjectile_Missile>();
            projectile.Init(transform);
            projectile.transform.SetParent(null);
            projectile.dmgData.damage = attackDamage;
            yield return new WaitForSeconds(attackSeparationTime);
        }
        canRotate = true;
    }


    IEnumerator ChangeElement_Cor()
    {
        ElementsManager.Elements newElement = (ElementsManager.Elements)Random.Range(0, (int)ElementsManager.Elements.COUNT - 1);
        while(newElement == enemyLife.entityElement) 
            newElement = (ElementsManager.Elements)Random.Range(0, (int)ElementsManager.Elements.COUNT - 1);

        PlayChangeElementParticles(newElement);
        yield return new WaitForSeconds(changeElementDelay);
        ChangeElementMaterials(newElement);
        enemyLife.entityElement = newElement;
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
