using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HealthStates_Manager
{
    //public enum HealthState { NORMAL, DEAD, BURNED, COLD, FROZEN }
    
    //const float BURNED_COLD_DMG_MULTIPLIER = 1.0f;
    //const float BURNED_FROZEN_DMG_MULTIPLIER = 2.0f;
    //const float COLD_BURNED_DMG_MULTIPLIER = 3.0f;
    //const float COLD_FROZEN_DMG_MULTIPLIER = 2.0f;


    //static public void CheckHealthStatesCompatibility(LifeSystem _lifeSystem, HealthState.Effect _appliedState, float _dmg)
    //{
    //    switch (_lifeSystem.state)
    //    {
    //        case HealthState.Effect.BURNED:
    //            CheckBurnedStateCompatibility(_lifeSystem, _appliedState, _dmg);
    //            break;

    //        case HealthState.Effect.COLD:
    //            CheckColdStateCompatibility(_lifeSystem, _appliedState, _dmg);
    //            break;

    //        default:
    //            _lifeSystem.ChangeHealthState(_appliedState);
    //            break;
    //    }
    //}

    //static void CheckBurnedStateCompatibility(LifeSystem _lifeSystem, HealthState.Effect _appliedState, float _dmg)
    //{
    //    switch (_appliedState)
    //    {
    //        case HealthState.Effect.COLD:
    //            CompatibilityEffect(_lifeSystem, _dmg, BURNED_COLD_DMG_MULTIPLIER, HealthState.Effect.NORMAL);
    //            break;

    //        case HealthState.Effect.FROZEN:
    //            CompatibilityEffect(_lifeSystem, _dmg, BURNED_FROZEN_DMG_MULTIPLIER, HealthState.Effect.COLD);
    //            break;

    //        default:
    //            _lifeSystem.ChangeHealthState(_appliedState);
    //            break;
    //    }

    //}

    //static void CheckColdStateCompatibility(LifeSystem _lifeSystem, HealthState.Effect _appliedState, float _dmg)
    //{
    //    switch (_appliedState)
    //    {
    //        case HealthState.Effect.BURNED:
    //            CompatibilityEffect(_lifeSystem, _dmg, COLD_BURNED_DMG_MULTIPLIER, HealthState.Effect.NORMAL);
    //            break;

    //        case HealthState.Effect.FROZEN:
    //            CompatibilityEffect(_lifeSystem, _dmg, COLD_FROZEN_DMG_MULTIPLIER, HealthState.Effect.FROZEN);
    //            break;

    //        default:
    //            _lifeSystem.ChangeHealthState(_appliedState);
    //            break;
    //    }

    //}
    //static void CompatibilityEffect(LifeSystem _lifeSystem, float _dmg, float _dmgMultiplier, HealthState.Effect _finalHealthState)
    //{
    //    _lifeSystem.currLife -= _dmg * _dmgMultiplier;
    //    _lifeSystem.CheckPlayerLifeLimits();
    //    if (_lifeSystem.state != HealthState.Effect.DEAD) _lifeSystem.ChangeHealthState(_finalHealthState);
    //}
}
