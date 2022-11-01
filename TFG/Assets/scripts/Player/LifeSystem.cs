using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeSystem : MonoBehaviour
{
    public enum EntityType { PLAYER, ENEMY, SHIELD }

    [SerializeField] internal EntityType entityType = EntityType.PLAYER;
    //[SerializeField] internal HealthState.Effect state = HealthState.Effect.NORMAL;
    [SerializeField] internal HealthState healthState;
    [SerializeField] internal float maxLife = 100;
    [SerializeField] internal float currLife = 100;

    internal float dmgInc = 1.0f;

    // Crec que serà millor que cada personatge controli la seva mort quan vegi que el state es HealthStates.DEAD
    //[SerializeField] internal bool managesDeath = true;
    //[SerializeField] internal float deathDelay = 3.0f;

    private void Start()
    {
        healthState = new HealthState(this);

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
        else if (currLife < 0)
        {
            currLife = 0;
            healthState.state = HealthState.Effect.DEAD;
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
        currLife -= _dmg * dmgInc;
        CheckPlayerLifeLimits();

        if (!healthState.initialized) _healthState.Init(this);
        if(healthState.state != HealthState.Effect.DEAD && _healthState.state != HealthState.Effect.NORMAL)
        {
            if (healthState.state != HealthState.Effect.NORMAL)
                healthState.CheckEffectsCompatibility(_healthState, _dmg * dmgInc);
            else
                ChangeHealthState(_healthState);
        }

    }

    internal void ChangeHealthState(HealthState _newHealthState)
    {
        healthState.EndEffect();

        if (!healthState.initialized) _newHealthState.Init(this);
        healthState = _newHealthState;

        healthState.StartEffect();

    }


    //IEnumerator Despawn()
    //{
    //    yield return new WaitForSeconds(deathDelay);
    //    Destroy(gameObject);
    //}

}
