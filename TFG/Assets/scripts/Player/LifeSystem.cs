using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LifeSystem : MonoBehaviour
{
    public enum EntityTypes { PLAYER, ENEMY, BOSS, OTHER }

    [SerializeField] EntityTypes entityType = EntityTypes.PLAYER;
    [SerializeField] internal ElementsManager.Elements entityElement;
    [SerializeField] float maxLife = 100;
    [SerializeField] float currLife = 100;
    [SerializeField] bool healWithSameElement = false;
    [SerializeField] float healPercentage = 0.05f;
    [SerializeField] private GameObject bloodPrefab;
    [SerializeField] private PlayerLifeBar playerLifeBar;
    [SerializeField] private Slider EnemyLifeBar;
    [SerializeField] private GameObject damageTextPrefab;

    //public ParticleSystem hitPS;

    internal float dmgInc = 1.0f;
    internal bool isDead = false;

    PlayerMovement playerMovementScript;
    CameraShake cameraRef;
    EnemyShake enemyShake;
    Transform parentCanvas;

    public EntityTypes EntityType { get { return entityType; } }
    public float MaxLife { get { return maxLife; } set { maxLife = value; } }
    public float CurrLife { get { return currLife; } set { currLife = value; } }

    float InitialLife
    {
        get
        {
            if (EntityType == EntityTypes.PLAYER) return MaxLife;
            switch (DifficultyManager.Difficulty)
            {
                case DifficultyMode.EASY: return MaxLife * DifficultyManager.Enemies_LifeMultiplier_EasyMode;
                case DifficultyMode.NORMAL: return MaxLife * DifficultyManager.Enemies_LifeMultiplier_NormalMode;
                case DifficultyMode.HARD: return MaxLife * DifficultyManager.Enemies_LifeMultiplier_HardMode;
                default: return MaxLife;
            }
        }
    }

    // Crec que ser� millor que cada personatge controli la seva mort quan vegi que el state es HealthStates.DEAD
    //[SerializeField] internal bool managesDeath = true;
    //[SerializeField] internal float deathDelay = 3.0f;

    private void Start()
    {
        if (EntityType.Equals(EntityTypes.PLAYER))
            playerMovementScript = GetComponent<PlayerMovement>();
        if (EntityType.Equals(EntityTypes.ENEMY) || EntityType.Equals(EntityTypes.BOSS))
        {
            parentCanvas = EnemyLifeBar.transform.parent;
            cameraRef = Camera.main.transform.GetComponent<CameraShake>();
            enemyShake = GetComponent<EnemyShake>();
        }

        if (EnemyLifeBar != null)
            EnemyLifeBar.gameObject.SetActive(true);

    }

    private void OnEnable()
    {
        if (CurrLife == MaxLife) currLife = InitialLife;
        CheckLifeLimits();
    }

    private void Update()
    {
        if (EntityType.Equals(EntityTypes.ENEMY) || EntityType.Equals(EntityTypes.BOSS))
        {
            EnemyLifeBar.value = CurrLife / InitialLife;
            parentCanvas.LookAt(cameraRef.transform);
            Vector3 canvasRot = parentCanvas.rotation.eulerAngles;
            parentCanvas.rotation = Quaternion.Euler(-canvasRot.x, 0, 0);
        }
    }

    public float GetLifePercentage(float _multiplier = 1.0f)
    {
        return CurrLife / InitialLife * _multiplier;
    }

    private void CheckLifeLimits()
    {
        if (CurrLife > InitialLife)
        {
            currLife = InitialLife;
        }
        else if (CurrLife <= 0)
        {
            currLife = 0;
            isDead = true;
            StopAllCoroutines();
            //if(entityType == EntityType.ENEMY)
            //{
            //    GetComponent<BaseEnemyScript>().ChangeState(BaseEnemyScript.States.DEATH);
            //}
            //healthStates.Clear();
            //if (managesDeath)
            //{
            //    //Trigger animation

            //}
        }
    }

    public void AddLife(float _lifeToAdd)
    {
        currLife += _lifeToAdd;
        CheckLifeLimits();
    }

    public void DamageWithLifeSteal(float _dmg, ElementsManager.Elements _attackElement, PlayerProjectileData projectileData, LifeSystem playerLifeSystem)
    {
        float dmgMultiplier = ElementsManager.GetReceiveDamageMultiplier(entityElement, _attackElement);

        if (dmgMultiplier > 1.9f)
        {
            int damageDealt = Mathf.CeilToInt(_dmg * dmgInc * dmgMultiplier);
            int lifeStolen = Mathf.CeilToInt(damageDealt * projectileData.dmgData.stealLifePercentage);
            playerLifeSystem.AddLife(lifeStolen);

            Transform playerLifeBar = playerLifeSystem.EnemyLifeBar.transform;

            GameObject addLifeTextInstance = Instantiate(damageTextPrefab, new Vector3(playerLifeBar.parent.position.x, playerLifeBar.parent.position.y, playerLifeBar.parent.position.z + 1f), damageTextPrefab.transform.rotation);
            string lifeStolenText = "+ " + lifeStolen.ToString();
            TextMeshProUGUI textUI = addLifeTextInstance.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
            textUI.color = Color.green;
            textUI.text = lifeStolenText;
        }


        Damage(_dmg, _attackElement);
    }
    //public void LifeSteal(ElementsManager.Elements _attackElement, PlayerProjectileData projectileData, LifeSystem playerLifeSystem)
    //{
    //    float dmgMultiplier = ElementsManager.GetReceiveDamageMultiplier(entityElement, _attackElement);

    //    if (dmgMultiplier > 1.9f)
    //    {
    //        int lifeStolen = Mathf.CeilToInt(maxLife * projectileData.dmgData.stealLifePercentage);
    //        playerLifeSystem.AddLife(lifeStolen);

    //        Transform playerLifeBar = playerLifeSystem.EnemyLifeBar.transform;

    //        GameObject addLifeTextInstance = Instantiate(damageTextPrefab, new Vector3(playerLifeBar.parent.position.x, playerLifeBar.parent.position.y, playerLifeBar.parent.position.z + 1f), damageTextPrefab.transform.rotation);
    //        string lifeStolenText = "+ " + lifeStolen.ToString();
    //        TextMeshProUGUI textUI = addLifeTextInstance.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
    //        textUI.color = Color.green;
    //        textUI.text = lifeStolenText;
    //    }
    //}

    public void CritFeedback()
    {
        Transform playerLifeBar = EnemyLifeBar.transform;
        GameObject critTextInstance = Instantiate(damageTextPrefab, new Vector3(playerLifeBar.parent.position.x, playerLifeBar.parent.position.y + 1, playerLifeBar.parent.position.z + 1f), damageTextPrefab.transform.rotation);
        TextMeshProUGUI textUI = critTextInstance.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
        textUI.color = Color.yellow;
        textUI.text = "CRITICAL!";
    }

    public void Damage(float _dmg, ElementsManager.Elements _attackElement)
    {
        if(healWithSameElement && _attackElement == entityElement)
        {
            HealEntity();
            StartCoroutine(enemyShake.Shake(0.3f, 0.03f, 0.03f));
            return;
        }

        float dmgMultiplier = ElementsManager.GetReceiveDamageMultiplier(entityElement, _attackElement);
        int dmgDealt = Mathf.CeilToInt(_dmg * dmgInc * dmgMultiplier);
        currLife -= dmgDealt;
        CheckLifeLimits();

        if (EntityType.Equals(EntityTypes.PLAYER) && CurrLife > 0)
        {
            playerLifeBar.Damage();
            //StartCoroutine(Camera.main.GetComponentInParent<CameraShake>().ShakeCamera(0.5f, 0.0002f));

            //AUDIO
            AudioManager.instance.PlayOneShot(FMODEvents.instance.playerDamageSound, this.transform.position);
        }

        if (EntityType.Equals(EntityTypes.PLAYER) || EntityType.Equals(EntityTypes.ENEMY) || EntityType.Equals(EntityTypes.BOSS))
        {
            if (CurrLife > 0)
                Instantiate(bloodPrefab, transform);

            GameObject damageTextInstance;
            if (EntityType.Equals(EntityTypes.BOSS))
            {
                Transform appearTransform = transform.Find("EnemyCanvas");
                damageTextInstance = Instantiate(damageTextPrefab, new Vector3(appearTransform.position.x, appearTransform.position.y, appearTransform.position.z + 1f), damageTextPrefab.transform.rotation);
            }
            else
            {
                Transform appearTransform = EnemyLifeBar.transform.parent;
                damageTextInstance = Instantiate(damageTextPrefab, new Vector3(appearTransform.position.x, appearTransform.position.y, appearTransform.position.z + 1f), damageTextPrefab.transform.rotation);
            }
            string damageText = dmgDealt.ToString();

            TextMeshProUGUI textUI = damageTextInstance.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();

            if (EntityType.Equals(EntityTypes.PLAYER))
            {
                textUI.color = Color.red;
                damageText = "-" + damageText;
            }

            textUI.SetText(damageText);

           

            if (EntityType.Equals(EntityTypes.ENEMY) || EntityType.Equals(EntityTypes.BOSS))
            {
                if (dmgMultiplier > 1.9f)
                {
                    if(GetComponent<BaseEnemyScript>().canEnterDamageState)
                        StartCoroutine(enemyShake.Shake(0.3f, 0.03f, 0.03f));
                    StartCoroutine(cameraRef.ShakeCamera(0.3f, 0.07f));
                    textUI.color = Color.yellow;
                }
                else if (dmgMultiplier > 0.6f)
                {
                    if (GetComponent<BaseEnemyScript>().canEnterDamageState)
                        StartCoroutine(enemyShake.Shake(0.2f, 0.01f, 0.01f));
                }
                else
                {
                    textUI.color = Color.white;
                }
                GameObject go = Instantiate(bloodPrefab, transform);
                go.transform.SetParent(null);
            }
        }

        
        if (isDead)
        {
            if(EntityType.Equals(EntityTypes.ENEMY) || EntityType.Equals(EntityTypes.BOSS))
            {
                parentCanvas.gameObject.SetActive(false);
                RoomEnemyManager assignedRoom = transform.GetComponentInParent<RoomEnemyManager>();
                if (assignedRoom == null) assignedRoom = transform.parent.GetComponentInParent<RoomEnemyManager>();
                if (assignedRoom == null) Debug.LogWarning("Assigned Room not found.");
                else assignedRoom.DiscardEnemy(GetComponent<BaseEnemyScript>());
            }
            else if (EntityType.Equals(EntityTypes.PLAYER))
            {
                FindObjectOfType<DeathScreenManager>().DeathScreenAppear();
            }

            //Activate death anim
            return;
        }

        //if (!healthState.initialized) _healthState.Init(this);
        if (EntityType.Equals(EntityTypes.PLAYER)/* && !isDead*/) playerMovementScript.DamageStartCorroutine();

    }

    void HealEntity()
    {
        int lifeHealed = Mathf.CeilToInt(InitialLife * healPercentage);
        currLife += lifeHealed;
        CheckLifeLimits();

        GameObject addLifeTextInstance; // = Instantiate(damageTextPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z + 1f), damageTextPrefab.transform.rotation);
        if (EntityType.Equals(EntityTypes.BOSS))
        {
            Transform appearTransform = transform.Find("EnemyCanvas");
            addLifeTextInstance = Instantiate(damageTextPrefab, new Vector3(appearTransform.position.x, appearTransform.position.y, appearTransform.position.z + 1f), damageTextPrefab.transform.rotation);
        }
        else
        {
            Transform appearTransform = EnemyLifeBar.transform.parent;
            addLifeTextInstance = Instantiate(damageTextPrefab, new Vector3(appearTransform.position.x, appearTransform.position.y, appearTransform.position.z + 1f), damageTextPrefab.transform.rotation);
        }
        string lifeHealedText = "+ " + lifeHealed.ToString();
        TextMeshProUGUI textUI = addLifeTextInstance.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
        textUI.color = Color.green;
        textUI.text = lifeHealedText;
    }


}
