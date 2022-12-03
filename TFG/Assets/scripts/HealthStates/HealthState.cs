using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class HealthState
{
    public enum Effect { NORMAL, DEAD, BURNED, BLEEDING, WET, COLD, FROZEN, PARALIZED, ELECTROCUTED, WIND, STRONG_BLOW, COUNT }

    [HideInInspector] public string name = "Default State";
    public Effect state = Effect.NORMAL;
    [SerializeField] internal HealthState effectWhenFinished;
    public float effectDuration = -1f;
    internal bool initialized = false;
    
    internal LifeSystem lifeSystem;
    internal float
        burnedCompatibility_DmgMultiplier = 0f,
        bleedingCompatibility_DmgMultiplier = 0f,
        wetCompatibility_DmgMultiplier = 0f,
        coldCompatibility_DmgMultiplier = 0f,
        frozenCompatibility_DmgMultiplier = 0f,
        paralizedCompatibility_DmgMultiplier = 0f,
        electrocutedCompatibility_DmgMultiplier = 0f,
        windCompatibility_DmgMultiplier = 0f,
        strongBlowCompatibility_DmgMultiplier = 0f;
    internal HealthState
        burnedCompatibility_FinalEffect = null,
        bleedingCompatibility_FinalEffect = null,
        wetCompatibility_FinalEffect = null,
        coldCompatibility_FinalEffect = null,
        frozenCompatibility_FinalEffect = null,
        paralizedCompatibility_FinalEffect = null,
        electrocutedCompatibility_FinalEffect = null,
        windCompatibility_FinalEffect = null,
        strongBlowCompatibility_FinalEffect = null;


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
                if (burnedCompatibility_FinalEffect == null) return false; 
                burnedCompatibility_FinalEffect.Init(lifeSystem);
                return ApplyCompatibilityEffect(_baseDmg, burnedCompatibility_DmgMultiplier, burnedCompatibility_FinalEffect);
                break;

            case HealthState.Effect.BLEEDING:
                if (bleedingCompatibility_FinalEffect == null) return false; 
                bleedingCompatibility_FinalEffect.Init(lifeSystem);
                return ApplyCompatibilityEffect(_baseDmg, bleedingCompatibility_DmgMultiplier, bleedingCompatibility_FinalEffect);
                break;

            case HealthState.Effect.COLD:
                if (coldCompatibility_FinalEffect == null) return false; 
                coldCompatibility_FinalEffect.Init(lifeSystem);
                return ApplyCompatibilityEffect(_baseDmg, coldCompatibility_DmgMultiplier, coldCompatibility_FinalEffect);
                break;

            case HealthState.Effect.FROZEN:
                if (frozenCompatibility_FinalEffect == null) return false; 
                frozenCompatibility_FinalEffect.Init(lifeSystem);
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
        if (effectDuration > 0 && state != Effect.NORMAL && !lifeSystem.isDead)
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
            case Effect.NORMAL:
                return new HealthState();

            case Effect.BURNED:
                return new Burned_HealthState();

            case Effect.BLEEDING:
                return new Bleeding_HealthState();

            case Effect.WET:
                return new Wind_HealthState();

            case Effect.COLD:
                return new Cold_HealthState();

            case Effect.FROZEN:
                return new Frozen_HealthState();

            case Effect.PARALIZED:
                return new Paralized_HealthState();

            case Effect.ELECTROCUTED:
                return new Electrocuted_HealthState();

            case Effect.WIND:
                return new Wind_HealthState();

            case Effect.STRONG_BLOW:
                return new StrongBlow_HealthState();


            default:
                return null;
        }
    }
    public static HealthState GetHealthStateByEffect(Effect _effect, LifeSystem _lifeSystem)
    {
        HealthState effect = GetHealthStateByEffect(_effect);
        effect.Init(_lifeSystem);
        return effect;
    }


}
