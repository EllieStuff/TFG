using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

public class PlayerAttack : MonoBehaviour
{
    [System.Serializable]
    public class AttackData
    {
        //public enum Type { NORMAL, FIRE, GRASS, WATER }
        //[HideInInspector] public string name;
        public ElementsManager.Elements type;
        public GameObject prefab;
        //public Attack()
        //{
        //    name = type.ToString();
        //}
    }


    [SerializeField] internal RoomEnemyManager roomEnemyManager;

    [Header("Attacks")]
    [SerializeField] internal ElementsManager.Elements currentAttackElement = ElementsManager.Elements.WATER;
    [SerializeField] internal float attackDelay = 0.8f, changeAttackDelay = 1.6f;
    [Space]
    //[SerializeField] List<Attack> attacks = new List<Attack>();
    [SerializeField] AttackData normalAttack;
    [SerializeField] AttackData fireAttack, grassAttack, waterAttack;

    Dictionary<ElementsManager.Elements, GameObject> attacksDictionary = new Dictionary<ElementsManager.Elements, GameObject>();

    internal Transform target;
    PlayerMovement playerMovement;
    LifeSystem playerLife;
    internal bool canAttack = true;
    float attackTimer;
    internal float dmgIncrease = 0f;
    internal int extraProjectiles = 0;
    internal bool damageIncreaseByAbilitySwap = false;
    internal int critSwapLevel = 0;
    internal float critChancePercentage = 0.05f;
    internal float stealLifePercentage = 0;
    internal int projectilePierceAmount = 0;
    float extraProjectilesDelay = 0.2f;
    //internal bool changingAttackType = false;

    const float RAYCAST_DISTANCE = 10;
    //const float CRIT_PERCENTAGE = 3;

    public GameObject glowBurstPS;
    [SerializeField] GameObject feedbackElementoFuerteFuego;
    [SerializeField] GameObject feedbackElementoFuerteAgua;
    [SerializeField] GameObject feedbackElementoFuerteHierba;
    

    int critSwapQuantity = 5;

    bool IsMoving { get { return playerMovement.moveDir != Vector3.zero && playerMovement.targetMousePos != Vector3.zero; } }

    //AUDIO
    private EventInstance playerAttackSound;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerLife = GetComponent<LifeSystem>();

        //target = roomEnemyManager.GetCloserEnemy(transform);

        attackTimer = attackDelay;
        attacksDictionary.Add(ElementsManager.Elements.FIRE, fireAttack.prefab);
        attacksDictionary.Add(ElementsManager.Elements.GRASS, grassAttack.prefab);
        attacksDictionary.Add(ElementsManager.Elements.WATER, waterAttack.prefab);
        attacksDictionary.Add(ElementsManager.Elements.NORMAL, normalAttack.prefab);

        //AUDIO
        playerAttackSound = AudioManager.instance.CreateInstance(FMODEvents.instance.playerAttackSound);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerLife.isDead) return;

        if (CanAttack() && roomEnemyManager.HasEnemiesRemainging())
        {
            Attack();
            if(extraProjectiles > 0) StartCoroutine(ExtraAttacksCoroutine());
            attackTimer = attackDelay;
        }

        if (attackTimer > 0 /*&& !changingAttackType*/) attackTimer -= Time.deltaTime;
        //else if (changeAttackTimer > 0 && changingAttackType) attackTimer -= Time.deltaTime;
        //else if(changeAttackTimer <= 0 && changingAttackType)
        //{
        //    changingAttackType = false;
        //    changeAttackTimer = changeAttackDelay;
        //    attackTimer = attackTimer / 2f;
        //}
        
    }

    public bool ShouldPlayAttackAnim()
    {
        return canAttack && target != null;
    }

    public bool CanAttack()
    {
        return !IsShootingWalls() && canAttack && !IsMoving && attackTimer <= 0 && target != null;
    }

    private bool IsShootingWalls()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.TransformDirection(Vector3.forward * RAYCAST_DISTANCE));

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag.Equals("Wall") || hit.collider.tag.Equals("Obstacle"))
                return true;
        }

        return false;
    }

    public void ResetCritQuantity()
    {
        if(damageIncreaseByAbilitySwap)
            critSwapQuantity = 0;
    }

    void SetCritAttack(PlayerProjectileData _projectile, bool _randomCrit)
    {
        if(damageIncreaseByAbilitySwap && critSwapQuantity < critSwapLevel)
        {
            _projectile.dmgData.critPercentage = critChancePercentage;
            critSwapQuantity++;
        }
        if(_randomCrit)
            _projectile.dmgData.critPercentage = critChancePercentage;
    }

    bool checkRandomCrit()
    {
        if(critChancePercentage > 0)
        {
            float randomChance = Random.Range(0f, 1f);

            if (randomChance <= critChancePercentage)
                return true;
        }

        return false;
    }


    void Attack()
    {
        //Spawn Particle glow
        ParticleSystem referenceGlowBurst = Instantiate(glowBurstPS,transform).GetComponent<ParticleSystem>();
        referenceGlowBurst.Play();
        referenceGlowBurst.transform.SetParent(null);

        Destroy(referenceGlowBurst.gameObject, 3f);

        if (target.gameObject.GetComponent<LifeSystem>().entityElement == ElementsManager.Elements.FIRE)
        {
            feedbackElementoFuerteAgua.SetActive(true);
            feedbackElementoFuerteFuego.SetActive(false);
            feedbackElementoFuerteHierba.SetActive(false);
        }
        else if (target.gameObject.GetComponent<LifeSystem>().entityElement == ElementsManager.Elements.WATER)
        {
            feedbackElementoFuerteAgua.SetActive(false);
            feedbackElementoFuerteFuego.SetActive(false);
            feedbackElementoFuerteHierba.SetActive(true);
        }
        else if (target.gameObject.GetComponent<LifeSystem>().entityElement == ElementsManager.Elements.GRASS)
        {
            feedbackElementoFuerteAgua.SetActive(false);
            feedbackElementoFuerteFuego.SetActive(true);
            feedbackElementoFuerteHierba.SetActive(false);
        }

        PlayerProjectileData attack = Instantiate(attacksDictionary[currentAttackElement], transform).GetComponent<PlayerProjectileData>();
        attack.transform.SetParent(null);
        attack.Init(transform);

        bool randomCrit = checkRandomCrit();

        if (damageIncreaseByAbilitySwap || randomCrit)
        {
            SetCritAttack(attack, randomCrit);
        }

        attack.dmgData.stealLifePercentage = stealLifePercentage;
        attack.dmgData.damage += dmgIncrease;

        //AUDIO
        PlayerAttackSound();
    }


    public void SetAttackTimer(float _timer)
    {
        attackTimer = _timer;
    }


    IEnumerator ExtraAttacksCoroutine()
    {
        for(int i = 0; i < extraProjectiles; i++)
        {
            yield return new WaitForSeconds(extraProjectilesDelay);
            Attack();
        }
        yield return new WaitForEndOfFrame();
    }


    //AUDIO
    //changes the element and plays attack sound 
    public void PlayerAttackSound()
    {
        if (currentAttackElement == ElementsManager.Elements.FIRE)
        {
            AudioManager.instance.SetFMODLabeledParameter("element", "fire", playerAttackSound);
        }
        else if (currentAttackElement == ElementsManager.Elements.GRASS)
        {
            AudioManager.instance.SetFMODLabeledParameter("element", "plant", playerAttackSound);
        }
        else if (currentAttackElement == ElementsManager.Elements.WATER)
        {
            AudioManager.instance.SetFMODLabeledParameter("element", "water", playerAttackSound);
        }

        playerAttackSound.start();
    }
}
