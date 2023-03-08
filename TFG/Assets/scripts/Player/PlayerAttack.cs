using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] internal float attackDelay = 1.5f, changeAttackDelay = 2f;
    [Space]
    //[SerializeField] List<Attack> attacks = new List<Attack>();
    [SerializeField] AttackData normalAttack;
    [SerializeField] AttackData fireAttack, grassAttack, waterAttack;

    Dictionary<ElementsManager.Elements, GameObject> attacksDictionary = new Dictionary<ElementsManager.Elements, GameObject>();

    internal Transform target;
    PlayerMovement playerMovement;
    internal bool canAttack = true;
    float attackTimer, changeAttackTimer;
    internal float dmgIncrease = 0f;
    internal int extraProjectiles = 0;
    internal bool stealLifeEnabled = false;
    internal bool damageIncreaseByAbilitySwap = false;
    internal int critSwapLevel = 0;
    internal float stealLifePercentage = 0;
    internal int projectilePierceAmount = 0;
    float extraProjectilesDelay = 0.2f;
    //internal bool changingAttackType = false;

    const float RAYCAST_DISTANCE = 10;
    const float CRIT_PERCENTAGE = 3;

    public GameObject glowBurstPS;
    

    int critSwapQuantity = 5;

    bool IsMoving { get { return playerMovement.moveDir != Vector3.zero && playerMovement.targetMousePos != Vector3.zero; } }


    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();

        //target = roomEnemyManager.GetCloserEnemy(transform);

        attackTimer = attackDelay;
        changeAttackTimer = changeAttackDelay;
        attacksDictionary.Add(ElementsManager.Elements.FIRE, fireAttack.prefab);
        attacksDictionary.Add(ElementsManager.Elements.GRASS, grassAttack.prefab);
        attacksDictionary.Add(ElementsManager.Elements.WATER, waterAttack.prefab);
        attacksDictionary.Add(ElementsManager.Elements.NORMAL, normalAttack.prefab);
    }

    // Update is called once per frame
    void Update()
    {
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

    void SetCritAttack(PlayerProjectileData _projectile)
    {
        if(damageIncreaseByAbilitySwap && critSwapQuantity < critSwapLevel)
        {
            _projectile.dmgData.critPercentage = CRIT_PERCENTAGE;
            critSwapQuantity++;
        }

    }

    void Attack()
    {
        //Spawn Particle glow
        ParticleSystem referenceGlowBurst = Instantiate(glowBurstPS,transform).GetComponent<ParticleSystem>();
        referenceGlowBurst.Play();
        referenceGlowBurst.transform.SetParent(null);

        PlayerProjectileData attack = Instantiate(attacksDictionary[currentAttackElement], transform).GetComponent<PlayerProjectileData>();
        attack.transform.SetParent(null);
        attack.Init(transform);

        if(damageIncreaseByAbilitySwap)
            SetCritAttack(attack);

        attack.dmgData.stealLife = stealLifeEnabled;
        attack.dmgData.stealLifePercentage = stealLifePercentage;
        attack.dmgData.damage += dmgIncrease;
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

}
