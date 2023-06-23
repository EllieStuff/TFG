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
    [SerializeField] internal HealthState effectWhenFinished = null;
    public float effectDuration = -1f;
    internal bool initialized = false;
    
    internal LifeSystem lifeSystem;
    internal Dictionary<Effect, float> compatibilityMap_DmgMultipliers = new Dictionary<Effect, float>();
    internal Dictionary<Effect, HealthState> compatibilityMap_FinalEffects = new Dictionary<Effect, HealthState>();


    public HealthState() { }
    public HealthState(HealthState _hs)
    {
        this.state = _hs.state;
        this.effectWhenFinished = _hs.effectWhenFinished;
        this.effectDuration = _hs.effectDuration;
        this.initialized = _hs.initialized;
        this.lifeSystem = _hs.lifeSystem;
        this.compatibilityMap_DmgMultipliers = new Dictionary<Effect, float>(_hs.compatibilityMap_DmgMultipliers);
        this.compatibilityMap_FinalEffects = new Dictionary<Effect, HealthState>(_hs.compatibilityMap_FinalEffects);
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
        float appliedEffect_DmgMultiplier = compatibilityMap_DmgMultipliers.GetValueOrDefault(_appliedEffect.state, 0.0f);
        HealthState appliedEffect_FinalEffect = compatibilityMap_FinalEffects.GetValueOrDefault(_appliedEffect.state, null);

        return ApplyCompatibilityEffect(_baseDmg, appliedEffect_DmgMultiplier, appliedEffect_FinalEffect);

    }
    internal virtual bool ApplyCompatibilityEffect(float _baseDmg, float _dmgMultiplier, HealthState _finalHealthState)
    {
        //lifeSystem.currLife -= _baseDmg * _dmgMultiplier;
        //lifeSystem.CheckLifeLimits();
        //if (lifeSystem.isDead) return false;


        //if (_finalHealthState == null) //La compatibilidad no altera el efecto
        //{
        //    return false;
        //}
        //else if(_finalHealthState.state == Effect.NORMAL) //El efecto resultante es que se anulan mutuamente
        //{
        //    _finalHealthState.Init(lifeSystem);
        //    lifeSystem.healthStates.Remove(this);
        //    EndEffect();
        //}
        //else //El efecto resultante es otro, asi que se sustituye el efecto actual por el resultante
        //{
        //    _finalHealthState.Init(lifeSystem);
        //    lifeSystem.ChangeHealthState(this, _finalHealthState);
        //}

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
        //compatibilityMap_DmgMultipliers.Clear();
        //compatibilityMap_FinalEffects.Clear();
        //compatibilityMap_DmgMultipliers = new Dictionary<Effect, float>();
        //compatibilityMap_FinalEffects = new Dictionary<Effect, HealthState>();
        if (effectWhenFinished != null) effectWhenFinished.Init(lifeSystem);
        //Cambiar posibles variables
    }

    internal virtual void EndEffectByTime()
    {
        EndEffect();
        //lifeSystem.healthStates.Remove(this);
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
                return new Wet_HealthState();

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
