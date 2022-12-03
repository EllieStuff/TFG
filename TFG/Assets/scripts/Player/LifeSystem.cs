using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeSystem : MonoBehaviour
{
    public enum EntityType { PLAYER, ENEMY, SHIELD }

    [SerializeField] internal EntityType entityType = EntityType.PLAYER;
    //[SerializeField] internal HealthState.Effect state = HealthState.Effect.NORMAL;
    [SerializeField] internal List<HealthState> healthStates = new List<HealthState>();
    [SerializeField] internal HealthStates_FeedbackManager healthStatesFeedback;
    [SerializeField] internal float maxLife = 100;
    [SerializeField] internal float currLife = 100;
    [SerializeField] private GameObject bloodPrefab;
    [SerializeField] private GameObject deathParticlesPrefab;
    [SerializeField] private PlayerHUD playerLifeBar;

    internal float dmgInc = 1.0f;
    internal bool isDead = false;

    PlayerMovement playerMovementScript;

    // Crec que serà millor que cada personatge controli la seva mort quan vegi que el state es HealthStates.DEAD
    //[SerializeField] internal bool managesDeath = true;
    //[SerializeField] internal float deathDelay = 3.0f;

    private void Start()
    {
        if (entityType.Equals(EntityType.PLAYER))
            playerMovementScript = GetComponent<PlayerMovement>();

        CheckPlayerLifeLimits();
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

    public void Damage(float _dmg, HealthState _healthState)
    {
        if (entityType.Equals(EntityType.PLAYER) && currLife > 0)
            playerLifeBar.ShakeBar();

        if (entityType.Equals(EntityType.PLAYER) || entityType.Equals(EntityType.ENEMY))
        {
            if (currLife > 0)
                Instantiate(bloodPrefab, transform);
            else
                Instantiate(deathParticlesPrefab, transform);
        }

        currLife -= _dmg * dmgInc;
        CheckPlayerLifeLimits();

        //if (!healthState.initialized) _healthState.Init(this);
        if (entityType.Equals(EntityType.PLAYER) && !isDead) playerMovementScript.DamageStartCorroutine();

        if(!isDead && _healthState != null && _healthState.state != HealthState.Effect.NORMAL)
        {
            if (healthStates.Count > 0)
            {
                bool foundEffectWithCompatibility = false;
                foreach (HealthState healthState in healthStates)
                {
                    if (healthState.CheckEffectsCompatibility(_healthState, _dmg * dmgInc))
                        foundEffectWithCompatibility = true;
                }
                CleanRepeatedHealthEffects();
                if (!foundEffectWithCompatibility) 
                    StartHealthState(_healthState);
            }
            else
            {
                StartHealthState(_healthState);
            }
        }

    }

    public void StartHealthState(HealthState _healthState)
    {
        healthStates.Add(_healthState);
        _healthState.StartEffect();
        healthStatesFeedback.ActivateFeedback(_healthState.state, _healthState.effectDuration);
    }

    internal void ChangeHealthState(HealthState _currHealthState, HealthState _newHealthState)
    {
        if (_currHealthState != null)
            _currHealthState.EndEffect();

        if (!_newHealthState.initialized) _newHealthState.Init(this);
        _currHealthState = _newHealthState;

        _currHealthState.StartEffect();
        healthStatesFeedback.ActivateFeedback(_currHealthState.state, _currHealthState.effectDuration);
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
