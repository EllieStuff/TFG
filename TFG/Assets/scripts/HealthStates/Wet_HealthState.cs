using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wet_HealthState : HealthState
{
    public Wet_HealthState()
    {
        state = HealthState.Effect.WET;
    }
    public override void Init(LifeSystem _lifeSystem)
    {
        base.Init(_lifeSystem);

        name = "Wet State";
        state = HealthState.Effect.WET;
        effectDuration = 10.0f;

        compatibilityMap_DmgMultipliers.Add(Effect.ELECTROCUTED, 0.5f);

        compatibilityMap_FinalEffects.Add(Effect.WET, new Wet_HealthState());
        compatibilityMap_FinalEffects.Add(Effect.BURNED, new HealthState());
        compatibilityMap_FinalEffects.Add(Effect.COLD, new Frozen_HealthState());

    }

    public override void StartEffect()
    {
        base.StartEffect();

    }

    public override void EndEffect()
    {
        base.EndEffect();
    }


}
