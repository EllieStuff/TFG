using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatBossEnemy : BatEnemy
{
    const float CHANGE_PHASE_BASE_DELAY = 0.5f;

    [Header("BatBoss Enemy")]
    [SerializeField] float changeElementDelay = 1.5f;
    [SerializeField] float flashDuration = 0.2f, changePhaseDelay = 3f;
    [SerializeField] int secondPhaseStartLife = 3000;
    [SerializeField] float secondPhaseAttackMultiplier = 1.3f, secondPhaseWaitMultiplier = 0.7f, secondPhaseSpeedMultiplier = 1.5f, repeatAttackProbability = 3f, secondPhaseProjSizeMultiplier = 1.3f, secondPhaseProjSpeedMultiplier = 1.2f;
    [SerializeField] ParticleSystem changeToFireParticles, changeToGrassParticles, changeToWaterParticles;
    [SerializeField] Material fireMat, fireTransparentMat, grassMat, grassTransparentMat, waterMat, waterTransparentMat;
    [SerializeField] Image flashImage;
    [SerializeField] Color fireFlashColor = Color.red, grassFlashColor = Color.green, waterFlashColor = Color.cyan;
    [SerializeField] float uiAppearSpeed = 1f;
    [SerializeField] CanvasGroup nameTextRef, lifeBarRef;


    [SerializeField] ParticleSystem enrageVFX;
    [SerializeField] ParticleSystem enrageBGVFX;

    CameraShake camShake;
    List<BatProjectile_Missile> projectiles = new List<BatProjectile_Missile>();
    bool changingElement = false;
    internal bool secondPhaseEntered = false;
    bool changingPhase = false;
    float projectileSizeMultiplier = 1f, projectileSpeedMultiplier = 1f;

    internal override void Start_Call()
    {
        base.Start_Call();
        camShake = Camera.main.GetComponent<CameraShake>();
    }

    private void OnEnable()
    {
        StartCoroutine(EnableUI(true));
    }
    private void OnDisable()
    {
        StopAllCoroutines();
        if (nameTextRef != null) nameTextRef.gameObject.SetActive(false);
        if (lifeBarRef != null) lifeBarRef.gameObject.SetActive(false);
    }

    internal override void Update_Call()
    {
        //if(enemyLife.currLife <= secondPhaseStartLife && !secondPhaseEntered && !isAttacking && !changingElement)
        //{
        //    secondPhaseEntered = true;
        //    //canAttack = false;
        //    attackTimer = 9999f;
        //    StopRB(stopForce);
        //    StopCoroutine(Attack_Cor());
        //    StartCoroutine(ChangePhase_Cor());
        //}
        if (changingPhase) return;

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            StopRB(stopForce);
            StartCoroutine(Attack_Cor());
            attackTimer = AttackWait + attackChargingTime;
        }
        else if(!isAttacking)
        {
            Vector3 targetMoveDir = (player.position - transform.position).normalized;
            MoveRB(targetMoveDir, actualMoveSpeed * speedMultiplier);
        }
    }

    internal override void AttackStart()
    {
        attackTimer = AttackWait + attackChargingTime; ;
    }

    internal override void DeathStart()
    {
        StopAllCoroutines();
        base.DeathStart();
        for (int i = 0; i < projectiles.Count; i++) projectiles[i].DestroyObject();
        projectiles.Clear();
        enrageBGVFX.Stop();
        enrageBGVFX.Clear();
        StartCoroutine(EnableUI(false));

        //AUDIO
        AudioManager.instance.PlayOneShot(FMODEvents.instance.bossDeathSound, this.transform.position);
    }


    IEnumerator EnableUI(bool _enable)
    {
        if (_enable)
        {
            nameTextRef.gameObject.SetActive(true);
            lifeBarRef.gameObject.SetActive(true);
            yield return null;
            lifeBarRef.alpha = nameTextRef.alpha = 0f;
            float timer = 0f;
            while (timer < uiAppearSpeed)
            {
                yield return new WaitForEndOfFrame();
                timer += Time.deltaTime;
                lifeBarRef.alpha = Mathf.Lerp(0f, 1f, timer / uiAppearSpeed);
                nameTextRef.alpha = Mathf.Lerp(0f, 1f, timer / uiAppearSpeed);
            }
        }
        else
        {
            float initAlphaLife = lifeBarRef.alpha, initAlphaName = nameTextRef.alpha;
            float timer = 0f;
            while (timer < uiAppearSpeed)
            {
                yield return new WaitForEndOfFrame();
                timer += Time.deltaTime;
                lifeBarRef.alpha = Mathf.Lerp(initAlphaLife, 0f, timer / uiAppearSpeed);
                nameTextRef.alpha = Mathf.Lerp(initAlphaName, 0f, timer / uiAppearSpeed);
            }
            yield return null;
            nameTextRef.gameObject.SetActive(false);
            lifeBarRef.gameObject.SetActive(false);
        }
    }

    void ChangePhase(float _initDelay)
    {
        secondPhaseEntered = true;
        //canAttack = false;
        attackTimer = 9999f;
        StopRB(stopForce);
        StartCoroutine(ChangePhase_Cor(_initDelay));
        StopCoroutine(Attack_Cor());
    }
    IEnumerator ChangePhase_Cor(float _initDelay)
    {
        yield return new WaitForSeconds(_initDelay + CHANGE_PHASE_BASE_DELAY);

        changingPhase = true;
        canRotate = false;
        attackDamage *= secondPhaseAttackMultiplier;
        attackWait *= secondPhaseWaitMultiplier;
        attackChargingTime *= secondPhaseWaitMultiplier;
        projectileSizeMultiplier *= secondPhaseProjSizeMultiplier;
        projectileSpeedMultiplier *= secondPhaseProjSpeedMultiplier;
        speedMultiplier = secondPhaseSpeedMultiplier;
        ///ToDo: Posar particules enfadament
        enrageVFX.Play();
        enrageBGVFX.Play();

        StartCoroutine(GetComponent<EnemyShake>().Shake(changePhaseDelay, 0.03f, 0.03f));
        yield return camShake.ShakeCamera(changePhaseDelay, 0.3f);
        //canAttack = true;
        canRotate = true;
        changingPhase = false;
        attackTimer = 0.1f;
    }

    protected override IEnumerator Attack_Cor()
    {
        if (enemyLife.currLife <= secondPhaseStartLife && !secondPhaseEntered)
            ChangePhase(changeElementDelay + flashDuration * 2f);

        float rnd = Random.Range(0f, repeatAttackProbability);
        if (!secondPhaseEntered || rnd < 1f)
        {
            changingElement = true;
            yield return ChangeElement_Cor();
            changingElement = false;
        }

        if (enemyLife.currLife <= secondPhaseStartLife && !secondPhaseEntered)
            ChangePhase(0f);

        //place shoot animation here
        isAttacking = true;
        yield return new WaitForSeconds(attackChargingTime);


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

            //AUDIO
            AudioManager.instance.PlayOneShot(FMODEvents.instance.bossAttackSound, this.transform.position);

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
        //StartCoroutine(camShake.ShakeCamera(changeElementDelay, 0.02f));
        yield return new WaitForSeconds(changeElementDelay);

        flashImage.gameObject.SetActive(true);

        StartCoroutine(camShake.ShakeCamera(flashDuration * 2f, 0.1f));
        EliTween.ChangeColor(flashImage, GetFlashColor(newElement), flashDuration);
        yield return new WaitForSeconds(flashDuration);

        //AUDIO
        AudioManager.instance.PlayOneShot(FMODEvents.instance.bossChangeElementSound, this.transform.position);

        ChangeElementMaterialsAndLights(newElement);
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

    void ChangeElementMaterialsAndLights(ElementsManager.Elements _element)
    {
        switch (_element)
        {
            case ElementsManager.Elements.FIRE:
                if (enemyMesh != null) enemyMesh.material = fireMat;
                else enemyMeshTmp.material = fireMat;
                transparentMat = fireTransparentMat;
                for (int i = 0; i < enemyLights.Count; i++) enemyLights[i].color = fireFlashColor;
                break;

            case ElementsManager.Elements.GRASS:
                if (enemyMesh != null) enemyMesh.material = grassMat;
                else enemyMeshTmp.material = grassMat;
                transparentMat = grassTransparentMat;
                for (int i = 0; i < enemyLights.Count; i++) enemyLights[i].color = grassFlashColor;
                break;

            case ElementsManager.Elements.WATER:
                if (enemyMesh != null) enemyMesh.material = waterMat;
                else enemyMeshTmp.material = waterMat;
                transparentMat = waterTransparentMat;
                for (int i = 0; i < enemyLights.Count; i++) enemyLights[i].color = waterFlashColor;
                break;

            default:
                break;
        }
    }

}
