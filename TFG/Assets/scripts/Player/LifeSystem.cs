using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeSystem : MonoBehaviour
{
    const float BURNED_COLD_DMG_MULTIPLIER = 1.0f;
    const float BURNED_FROZEN_DMG_MULTIPLIER = 2.0f;
    const float COLD_BURNED_DMG_MULTIPLIER = 3.0f;
    const float COLD_FROZEN_DMG_MULTIPLIER = 2.0f;

    public enum HealthStates { NORMAL, DEAD, BURNED, COLD, FROZEN }
    
    [SerializeField] internal HealthStates state = HealthStates.NORMAL;
    [SerializeField] internal float maxLife = 100;
    [SerializeField] internal float currLife = 100;

    // Crec que serà millor que cada personatge controli la seva mort quan vegi que el state es HealthStates.DEAD
    //[SerializeField] internal bool managesDeath = true;
    //[SerializeField] internal float deathDelay = 3.0f;

    private void Start()
    {
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
            state = HealthStates.DEAD;
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

    public void Damage(float _dmg, HealthStates _state = HealthStates.NORMAL)
    {
        currLife -= _dmg;
        CheckPlayerLifeLimits();
        if(state != HealthStates.DEAD && _state != HealthStates.NORMAL)
        {
            if (state != HealthStates.NORMAL)
                CheckHealthStatesCompatibility(this, _state, _dmg);
            else
                ChangeHealthState(_state);
        }
    }


    /// ToDo:
    ///  - Posar això en una clase estàtica a part
    ///  - Fer que vagi amb herència en contres de amb màquina d'estats
    ///  - Usar màquina d'estats només per a diferencia si estàs afectant a un jugador, enemic o algún altre element (com l'escut)
    void CheckHealthStatesCompatibility(LifeSystem _lifeSystem, HealthStates _appliedState, float _dmg)
    {
        switch (state)
        {
            case HealthStates.BURNED:
                CheckBurnedStateCompatibility(_lifeSystem, _appliedState, _dmg);
                break;

            case HealthStates.COLD:
                CheckColdStateCompatibility(_lifeSystem, _appliedState, _dmg);
                break;

            default:
                _lifeSystem.ChangeHealthState(_appliedState);
                break;
        }
    }

    void CheckBurnedStateCompatibility(LifeSystem _lifeSystem, HealthStates _appliedState, float _dmg)
    {
        switch (_appliedState)
        {
            case HealthStates.COLD:
                CompatibilityEffect(_lifeSystem, _dmg, BURNED_COLD_DMG_MULTIPLIER, HealthStates.NORMAL);
                break;

            case HealthStates.FROZEN:
                CompatibilityEffect(_lifeSystem, _dmg, BURNED_FROZEN_DMG_MULTIPLIER, HealthStates.COLD);
                break;

            default:
                _lifeSystem.ChangeHealthState(_appliedState);
                break;
        }

    }

    void CheckColdStateCompatibility(LifeSystem _lifeSystem, HealthStates _appliedState, float _dmg)
    {
        switch (_appliedState)
        {
            case HealthStates.BURNED:
                CompatibilityEffect(_lifeSystem, _dmg, COLD_BURNED_DMG_MULTIPLIER, HealthStates.NORMAL);
                break;

            case HealthStates.FROZEN:
                CompatibilityEffect(_lifeSystem, _dmg, COLD_FROZEN_DMG_MULTIPLIER, HealthStates.FROZEN);
                break;

            default:
                _lifeSystem.ChangeHealthState(_appliedState);
                break;
        }

    }
    void CompatibilityEffect(LifeSystem _lifeSystem, float _dmg, float _dmgMultiplier, HealthStates _finalHealthState)
    {
        currLife -= _dmg * _dmgMultiplier;
        CheckPlayerLifeLimits();
        if (state != HealthStates.DEAD) _lifeSystem.ChangeHealthState(_finalHealthState);
    }


    internal void ChangeHealthState(HealthStates _state)
    {
        state = _state;
        //Set state maxTime and timer
    }


    //IEnumerator Despawn()
    //{
    //    yield return new WaitForSeconds(deathDelay);
    //    Destroy(gameObject);
    //}

}
