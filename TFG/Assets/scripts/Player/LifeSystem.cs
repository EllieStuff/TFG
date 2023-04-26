using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LifeSystem : MonoBehaviour
{
    public enum EntityType { PLAYER, ENEMY, BOSS, OTHER }

    [SerializeField] internal EntityType entityType = EntityType.PLAYER;
    [SerializeField] internal ElementsManager.Elements entityElement;
    //[SerializeField] internal HealthState.Effect state = HealthState.Effect.NORMAL;
    [SerializeField] internal List<HealthState> healthStates = new List<HealthState>();
    [SerializeField] internal HealthStates_FeedbackManager healthStatesFeedback;
    [SerializeField] internal float maxLife = 100;
    [SerializeField] internal float currLife = 100;
    [SerializeField] bool healWithSameElement = false;
    [SerializeField] internal float healPercentage = 0.05f;
    [SerializeField] private GameObject bloodPrefab;
    //[SerializeField] private GameObject deathParticlesPrefab;
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

    // Crec que ser� millor que cada personatge controli la seva mort quan vegi que el state es HealthStates.DEAD
    //[SerializeField] internal bool managesDeath = true;
    //[SerializeField] internal float deathDelay = 3.0f;

    private void Start()
    {
        if (entityType.Equals(EntityType.PLAYER))
            playerMovementScript = GetComponent<PlayerMovement>();
        if (entityType.Equals(EntityType.ENEMY) || entityType.Equals(EntityType.BOSS))
        {
            parentCanvas = EnemyLifeBar.transform.parent;
            cameraRef = Camera.main.transform.GetComponent<CameraShake>();
            enemyShake = GetComponent<EnemyShake>();
        }

        if (EnemyLifeBar != null)
            EnemyLifeBar.gameObject.SetActive(true);

        CheckLifeLimits();
    }

    private void Update()
    {
        if (entityType.Equals(EntityType.ENEMY) || entityType.Equals(EntityType.BOSS))
        {
            EnemyLifeBar.value = currLife / maxLife;
            parentCanvas.LookAt(cameraRef.transform);
            Vector3 canvasRot = parentCanvas.rotation.eulerAngles;
            parentCanvas.rotation = Quaternion.Euler(-canvasRot.x, 0, 0);
        }
    }

    public float GetLifePercentage(float _multiplier = 1.0f)
    {
        return currLife / maxLife * _multiplier;
    }

    internal void CheckLifeLimits()
    {
        if (currLife > maxLife)
        {
            currLife = maxLife;
        }
        else if (currLife <= 0)
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

        if (entityType.Equals(EntityType.PLAYER) && currLife > 0)
        {
            playerLifeBar.Damage();
            //StartCoroutine(Camera.main.GetComponentInParent<CameraShake>().ShakeCamera(0.5f, 0.0002f));
        }

        if (entityType.Equals(EntityType.PLAYER) || entityType.Equals(EntityType.ENEMY) || entityType.Equals(EntityType.BOSS))
        {
            if (currLife > 0)
                Instantiate(bloodPrefab, transform);

            GameObject damageTextInstance;
            if (entityType.Equals(EntityType.BOSS))
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

            if (entityType.Equals(EntityType.PLAYER))
            {
                textUI.color = Color.red;
                damageText = "-" + damageText;
            }

            textUI.SetText(damageText);

           

            if (entityType.Equals(EntityType.ENEMY) || entityType.Equals(EntityType.BOSS))
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
            if(entityType.Equals(EntityType.ENEMY) || entityType.Equals(EntityType.BOSS))
            {
                parentCanvas.gameObject.SetActive(false);
                RoomEnemyManager assignedRoom = transform.GetComponentInParent<RoomEnemyManager>();
                if (assignedRoom == null) assignedRoom = transform.parent.GetComponentInParent<RoomEnemyManager>();
                if (assignedRoom == null) Debug.LogWarning("Assigned Room not found.");
                else assignedRoom.DiscardEnemy(GetComponent<BaseEnemyScript>());
            }
            else if (entityType.Equals(EntityType.PLAYER))
            {
                FindObjectOfType<DeathScreenManager>().DeathScreenAppear();
            }

            //Activate death anim
            return;
        }

        //if (!healthState.initialized) _healthState.Init(this);
        if (entityType.Equals(EntityType.PLAYER)/* && !isDead*/) playerMovementScript.DamageStartCorroutine();

    }

    void HealEntity()
    {
        int lifeHealed = Mathf.CeilToInt(maxLife * healPercentage);
        currLife += lifeHealed;
        CheckLifeLimits();

        GameObject addLifeTextInstance; // = Instantiate(damageTextPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z + 1f), damageTextPrefab.transform.rotation);
        if (entityType.Equals(EntityType.BOSS))
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

    public void StartHealthState(HealthState _healthState)
    {
        healthStates.Add(_healthState);
        _healthState.StartEffect();
        if (healthStatesFeedback != null) healthStatesFeedback.ActivateFeedback(_healthState.state, _healthState.effectDuration);
        else Debug.LogWarning("HealthStatesFeedback for " + healthStates[healthStates.Count - 1].state.ToString() + " was not assigned");
    }

    internal void ChangeHealthState(HealthState _currHealthState, HealthState _newHealthState)
    {
        if (_currHealthState != null)
        {
            _currHealthState.EndEffect();
            //healthStates.Remove(_currHealthState);
        }

        if (!_newHealthState.initialized) _newHealthState.Init(this);
        healthStates[healthStates.IndexOf(_currHealthState)] = _newHealthState;
        //healthStates.Add(_newHealthState);
        //_currHealthState = _newHealthState;

        _newHealthState.StartEffect();
        if (healthStatesFeedback != null) healthStatesFeedback.ActivateFeedback(_newHealthState.state, _newHealthState.effectDuration);
        else Debug.LogWarning("HealthStatesFeedback for " + healthStates[healthStates.Count - 1].state.ToString() + " was not assigned");
    }

    void CleanRepeatedHealthEffects()
    {
        for(int i = 0; i < healthStates.Count; i++)
        {
            HealthState.Effect searchedEffect = healthStates[i].state;
            if (searchedEffect == HealthState.Effect.BLEEDING) continue;

            for (int j = 0; j < healthStates.Count; j++)
            {
                if (i == j) continue;
                if(searchedEffect == healthStates[j].state)
                {
                    healthStates.RemoveAt(j);
                    j--;
                }
            }
        }
    }


    //IEnumerator Despawn()
    //{
    //    yield return new WaitForSeconds(deathDelay);
    //    Destroy(gameObject);
    //}

    /*void OnMouseOver()
    {
        if(entityType.Equals(EntityType.ENEMY))
            EnemyLifeBar.gameObject.SetActive(true);
    }

    private void OnMouseExit()
    {
        if (entityType.Equals(EntityType.ENEMY))
            EnemyLifeBar.gameObject.SetActive(false);
    }*/

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SwordRegion") || other.CompareTag("Weapon"))
        {
            //DamageData dmgData = col.GetComponent<DamageData>();
            //Damage(dmgData.damage, dmgData.effect);
            //Damage(10, new HealthState());
        }
    }


}
