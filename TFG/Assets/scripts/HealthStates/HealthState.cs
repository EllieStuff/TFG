using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class HealthState
{
    public enum Effect { NORMAL, DEAD, BURNED, COLD, FROZEN }

    public Effect state = Effect.NORMAL;
    [SerializeField] internal HealthState effectWhenFinished;
    public float effectDuration = -1f;
    internal bool initialized = false;
    
    internal LifeSystem lifeSystem;
    internal float
        burnedCompatibility_DmgMultiplier = 1f,
        coldCompatibility_DmgMultiplier = 1f,
        frozenCompatibility_DmgMultiplier = 1f;
    internal HealthState
        burnedCompatibility_FinalEffect,
        coldCompatibility_FinalEffect,
        frozenCompatibility_FinalEffect;


    public HealthState() { }
    public HealthState(LifeSystem _lifeSystem)
    {
        Init(_lifeSystem);
    }
    public virtual void Init(LifeSystem _lifeSystem)
    {
        initialized = true;
        lifeSystem = _lifeSystem;
    }


    public virtual void Update() { }
    

    public virtual void CheckEffectsCompatibility(HealthState _appliedEffect, float _baseDmg)
    {
        switch (_appliedEffect.state)
        {
            case HealthState.Effect.BURNED:
                burnedCompatibility_FinalEffect.Init(lifeSystem);
                ApplyCompatibilityEffect(_baseDmg, burnedCompatibility_DmgMultiplier, burnedCompatibility_FinalEffect);
                break;

            case HealthState.Effect.COLD:
                coldCompatibility_FinalEffect.Init(lifeSystem);
                ApplyCompatibilityEffect(_baseDmg, coldCompatibility_DmgMultiplier, coldCompatibility_FinalEffect);
                break;

            case HealthState.Effect.FROZEN:
                frozenCompatibility_FinalEffect.Init(lifeSystem);
                ApplyCompatibilityEffect(_baseDmg, frozenCompatibility_DmgMultiplier, frozenCompatibility_FinalEffect);
                break;


            default:
                if (!_appliedEffect.initialized) _appliedEffect.Init(lifeSystem);
                lifeSystem.ChangeHealthState(_appliedEffect);
                break;
        }
    }
    internal virtual void ApplyCompatibilityEffect(float _baseDmg, float _dmgMultiplier, HealthState _finalHealthState)
    {
        lifeSystem.currLife -= _baseDmg * _dmgMultiplier;
        lifeSystem.CheckPlayerLifeLimits();
        if (state != HealthState.Effect.DEAD) lifeSystem.ChangeHealthState(_finalHealthState);
    }


    public virtual void StartEffect()
    {
        if (effectDuration > 0 && state != Effect.NORMAL && state != Effect.DEAD)
            lifeSystem.StartCoroutine(EndEffectByTimeCoroutine());
        //Cambiar posibles variables
    }

    public virtual void EndEffect()
    {
        lifeSystem.StopCoroutine(EndEffectByTimeCoroutine());
        //Cambiar posibles variables
    }

    internal virtual void EndEffectByTime()
    {
        EndEffect();
        effectWhenFinished.Init(lifeSystem);
        lifeSystem.ChangeHealthState(effectWhenFinished);
    }

    internal IEnumerator EndEffectByTimeCoroutine()
    {
        yield return new WaitForSeconds(effectDuration);
        EndEffectByTime();
    }



    public static HealthState GetHealthStateByEffect(Effect _effect)
    {
        switch (_effect)
        {
            case Effect.NORMAL: case Effect.DEAD:
                return new HealthState();

            case Effect.BURNED:
                return new Burned_HealthState();

            case Effect.COLD:
                return new Cold_HealthState();

            case Effect.FROZEN:
                return new Frozen_HealthState();


            default:
                return null;
        }
    }

}
