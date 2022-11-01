using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frozen_HealthState : HealthState
{
    // TODO: StartEffect() i EndEffect()

    public Frozen_HealthState()
    {
        state = HealthState.Effect.FROZEN;
    }
    public override void Init(LifeSystem _lifeSystem)
    {
        base.Init(_lifeSystem);

        state = HealthState.Effect.FROZEN;
        effectDuration = 5.0f;

        burnedCompatibility_DmgMultiplier = 1.5f;
        coldCompatibility_DmgMultiplier = 1.5f;
        frozenCompatibility_DmgMultiplier = 2.0f;

        burnedCompatibility_FinalEffect = new Cold_HealthState();
        coldCompatibility_FinalEffect = new Frozen_HealthState();
        frozenCompatibility_FinalEffect = new Frozen_HealthState();

    }

    public override void StartEffect()
    {
        base.StartEffect();

        // TODO

    }

    public override void EndEffect()
    {
        base.EndEffect();

        // TODO

    }


}
