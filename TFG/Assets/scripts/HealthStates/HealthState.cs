using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class HealthState
{
    public enum Effect { NORMAL, DEAD, BURNED, BLEEDING, WET, COLD, FROZEN, PARALIZED, ELECTROCUTED, WIND, STRONG_BLOW, COUNT }

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
    public HealthState(HealthState _hs)
    {
        this.state = _hs.state;
        this.effectWhenFinished = _hs.effectWhenFinished;
        this.effectDuration = _hs.effectDuration;
        this.initialized = _hs.initialized;
        this.lifeSystem = _hs.lifeSystem;
        this.burnedCompatibility_DmgMultiplier = _hs.burnedCompatibility_DmgMultiplier;
        this.coldCompatibility_DmgMultiplier = _hs.coldCompatibility_DmgMultiplier;
        this.frozenCompatibility_DmgMultiplier = _hs.frozenCompatibility_DmgMultiplier;
        this.burnedCompatibility_FinalEffect = _hs.burnedCompatibility_FinalEffect;
        this.coldCompatibility_FinalEffect = _hs.coldCompatibility_FinalEffect;
        this.frozenCompatibility_FinalEffect = _hs.frozenCompatibility_FinalEffect;
    }

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
    

    public virtual bool CheckEffectsCompatibility(HealthState _appliedEffect, float _baseDmg)
    {
        switch (_appliedEffect.state)
        {
            case HealthState.Effect.BURNED:
                if(burnedCompatibility_FinalEffect != null) burnedCompatibility_FinalEffect.Init(lifeSystem);
                return ApplyCompatibilityEffect(_baseDmg, burnedCompatibility_DmgMultiplier, burnedCompatibility_FinalEffect);
                break;

            case HealthState.Effect.COLD:
                if (coldCompatibility_FinalEffect != null) coldCompatibility_FinalEffect.Init(lifeSystem);
                return ApplyCompatibilityEffect(_baseDmg, coldCompatibility_DmgMultiplier, coldCompatibility_FinalEffect);
                break;

            case HealthState.Effect.FROZEN:
                if (frozenCompatibility_FinalEffect != null) frozenCompatibility_FinalEffect.Init(lifeSystem);
                return ApplyCompatibilityEffect(_baseDmg, frozenCompatibility_DmgMultiplier, frozenCompatibility_FinalEffect);
                break;


            default:
                //if (!_appliedEffect.initialized) _appliedEffect.Init(lifeSystem);
                //lifeSystem.StartHealthState(_appliedEffect);
                break;
        }

        return false;
    }
    internal virtual bool ApplyCompatibilityEffect(float _baseDmg, float _dmgMultiplier, HealthState _finalHealthState)
    {
        lifeSystem.currLife -= _baseDmg * _dmgMultiplier;
        lifeSystem.CheckPlayerLifeLimits();
        if (lifeSystem.isDead) return false;

        if (_finalHealthState == null) //La compatibilidad no altera el efecto
        {
            return false;
        }
        else if(_finalHealthState.state == Effect.NORMAL) //El efecto resultante es que se anulan mutuamente
        {
            EndEffect();
            lifeSystem.healthStates.Remove(this);
        }
        else //El efecto resultante es otro, asi que se sustituye el efecto actual por el resultante
        {
            lifeSystem.ChangeHealthState(this, _finalHealthState);
        }

        return true;
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
        effectWhenFinished.Init(lifeSystem);
        //Cambiar posibles variables
    }

    internal virtual void EndEffectByTime()
    {
        EndEffect();
        lifeSystem.healthStates.Remove(this);
        //lifeSystem.ChangeHealthState(effectWhenFinished);
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
