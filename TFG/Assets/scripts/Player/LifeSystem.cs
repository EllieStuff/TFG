using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LifeSystem : MonoBehaviour
{
    public enum EntityType { PLAYER, ENEMY, SHIELD }

    [SerializeField] internal EntityType entityType = EntityType.PLAYER;
    [SerializeField] internal ElementsManager.Elements entityElement;
    //[SerializeField] internal HealthState.Effect state = HealthState.Effect.NORMAL;
    [SerializeField] internal List<HealthState> healthStates = new List<HealthState>();
    [SerializeField] internal HealthStates_FeedbackManager healthStatesFeedback;
    [SerializeField] internal float maxLife = 100;
    [SerializeField] internal float currLife = 100;
    [SerializeField] private GameObject bloodPrefab;
    //[SerializeField] private GameObject deathParticlesPrefab;
    [SerializeField] private PlayerLifeBar playerLifeBar;
    [SerializeField] private Slider EnemyLifeBar;
    [SerializeField] private GameObject damageTextPrefab;

    public ParticleSystem hitPS;

    internal float dmgInc = 1.0f;
    float dmgDealt;
    internal bool isDead = false;

    PlayerMovement playerMovementScript;
    Transform camera;
    Transform parentCanvas;

    // Crec que serà millor que cada personatge controli la seva mort quan vegi que el state es HealthStates.DEAD
    //[SerializeField] internal bool managesDeath = true;
    //[SerializeField] internal float deathDelay = 3.0f;

    private void Start()
    {
        if (entityType.Equals(EntityType.PLAYER))
            playerMovementScript = GetComponent<PlayerMovement>();
        if (entityType.Equals(EntityType.ENEMY))
        {
            parentCanvas = EnemyLifeBar.transform.parent;
            camera = Camera.main.transform;
        }

        if (EnemyLifeBar != null)
            EnemyLifeBar.gameObject.SetActive(true);

        CheckPlayerLifeLimits();
    }

    private void Update()
    {
        if (entityType.Equals(EntityType.ENEMY))
        {
            EnemyLifeBar.value = currLife / maxLife;
            parentCanvas.LookAt(camera);
            Vector3 canvasRot = parentCanvas.rotation.eulerAngles;
            parentCanvas.rotation = Quaternion.Euler(-canvasRot.x, 0, 0);
        }
    }

    public float GetLifePercentage(float _multiplier = 1.0f)
    {
        return currLife / maxLife * _multiplier;
    }

    internal void CheckPlayerLifeLimits()
    {
        if (currLife > maxLife)
        {
            currLife = maxLife;
        }
        else if (currLife <= 0)
        {
            currLife = 0;
            isDead = true;
            if(entityType == EntityType.ENEMY)
            {
                GetComponent<BaseEnemyScript>().ChangeState(BaseEnemyScript.States.DAMAGE);
            }
            StopAllCoroutines();
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
        CheckPlayerLifeLimits();
    }

    public void Damage(float _dmg, ElementsManager.Elements _attackElement)
    {
        float dmgMultiplier = ElementsManager.GetReceiveDamageMultiplier(entityElement, _attackElement);
        dmgDealt = _dmg * dmgInc * dmgMultiplier;
        currLife -= dmgDealt;
        CheckPlayerLifeLimits();

        if (entityType.Equals(EntityType.PLAYER) && currLife > 0)
        {
            playerLifeBar.Damage();
            //StartCoroutine(Camera.main.GetComponentInParent<CameraShake>().ShakeCamera(0.5f, 0.0002f));
        }

        if (entityType.Equals(EntityType.PLAYER) || entityType.Equals(EntityType.ENEMY))
        {
            if (currLife > 0)
                Instantiate(bloodPrefab, transform);

            Transform enemyLifeBar = EnemyLifeBar.transform;

            GameObject damageTextInstance = Instantiate(damageTextPrefab, new Vector3(enemyLifeBar.parent.gameObject.transform.position.x, enemyLifeBar.parent.transform.position.y, enemyLifeBar.parent.transform.position.z + 1f), damageTextPrefab.transform.rotation);
            string damageText = dmgDealt.ToString();

            TextMeshPro textUI = damageTextInstance.transform.GetChild(0).GetComponent<TextMeshPro>();

            if (entityType.Equals(EntityType.PLAYER))
            {
                textUI.color = Color.red;
                damageText = "-" + damageText;
            }

            textUI.SetText(damageText);

            //Le he puesto rapidamente algo provisional una particula porque no se cambiar el color del pj y es provisional hasta tener el modelo final
            hitPS.Play();

            if (entityType.Equals(EntityType.ENEMY))
            {
                if (dmgMultiplier > 1.9f)
                {
                    StartCoroutine(GetComponent<EnemyShake>().Shake(0.3f, 0.03f, 0.03f));
                    StartCoroutine(Camera.main.GetComponentInParent<CameraShake>().ShakeCamera(0.3f, 0.01f));
                    textUI.color = Color.green;
                }
                else if (dmgMultiplier > 0.7f)
                {
                    StartCoroutine(GetComponent<EnemyShake>().Shake(0.2f, 0.01f, 0.01f));
                    //StartCoroutine(Camera.main.GetComponentInParent<CameraShake>().ShakeCamera(0.2f, 0.008f));
                }
                else
                {
                    textUI.color = Color.white;
                    //StartCoroutine(GetComponent<EnemyShake>().Shake(0.15f, 0.005f, 0.005f));
                    //StartCoroutine(Camera.main.GetComponentInParent<CameraShake>().ShakeCamera(0.15f, 0.004f));
                }
            }
        }

        

        
        if (isDead)
        {
            if(entityType.Equals(EntityType.ENEMY))
            {
                parentCanvas.gameObject.SetActive(false);
                RoomEnemyManager assignedRoom = transform.GetComponentInParent<RoomEnemyManager>();
                if (assignedRoom == null) assignedRoom = transform.parent.GetComponentInParent<RoomEnemyManager>();
                if (assignedRoom == null) Debug.LogWarning("Assigned Room not found.");
                else assignedRoom.DiscardEnemy(GetComponent<BaseEnemyScript>());
            }

            //Activate death anim
            return;
        }

        //if (!healthState.initialized) _healthState.Init(this);
        if (entityType.Equals(EntityType.PLAYER)/* && !isDead*/) playerMovementScript.DamageStartCorroutine();

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
