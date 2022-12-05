using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paralized_HealthState : HealthState
{

    public Paralized_HealthState()
    {
        state = HealthState.Effect.PARALIZED;
    }
    public override void Init(LifeSystem _lifeSystem)
    {
        base.Init(_lifeSystem);

        name = "Paralized State";
        state = HealthState.Effect.PARALIZED;
        effectDuration = 1.0f; //Depende de la carta

        //Has no dmg multiplier compatibility

        compatibilityMap_FinalEffects.Add(Effect.PARALIZED, new Paralized_HealthState());

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
