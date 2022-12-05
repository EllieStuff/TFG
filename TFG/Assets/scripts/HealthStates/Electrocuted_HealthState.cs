using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Electrocuted_HealthState : HealthState
{
    [SerializeField] internal float paralisisDuration;

    public Electrocuted_HealthState()
    {
        state = HealthState.Effect.ELECTROCUTED;
    }
    public override void Init(LifeSystem _lifeSystem)
    {
        base.Init(_lifeSystem);

        name = "Electrocuted State";
        state = HealthState.Effect.ELECTROCUTED;
        effectDuration = 6.0f;
        paralisisDuration = 1.0f; //Depende de la carta (0.5s a 6s)

        compatibilityMap_DmgMultipliers.Add(Effect.BURNED, 0.5f);
        compatibilityMap_DmgMultipliers.Add(Effect.WET, 0.5f);

        compatibilityMap_FinalEffects.Add(Effect.ELECTROCUTED, new Electrocuted_HealthState());

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
