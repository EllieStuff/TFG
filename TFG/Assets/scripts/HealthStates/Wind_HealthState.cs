using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind_HealthState : HealthState
{
    //Nota: el efecto del viento puede que fuera mejor controlarlo desde el prefab de la habilidad como tal como tal, pero ya veremos

    public Wind_HealthState()
    {
        state = HealthState.Effect.WIND;
    }
    public override void Init(LifeSystem _lifeSystem)
    {
        base.Init(_lifeSystem);

        name = "Wind State";
        state = HealthState.Effect.WIND;
        effectDuration = 10.0f; //Depende de la carta

        compatibilityMap_DmgMultipliers.Add(Effect.BURNED, 0.5f);

        compatibilityMap_FinalEffects.Add(Effect.WIND, new Wind_HealthState());

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
