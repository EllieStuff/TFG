using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burned_HealthState : HealthState
{
    [SerializeField] float
        defaultDmgInc = 1.5f,
        shieldDmgInc = 4.0f;


    public Burned_HealthState()
    {
        state = HealthState.Effect.BURNED;
    }
    public override void Init(LifeSystem _lifeSystem)
    {
        base.Init(_lifeSystem);

        state = HealthState.Effect.BURNED;
        effectDuration = 5.0f;

        burnedCompatibility_DmgMultiplier = 0.0f;
        coldCompatibility_DmgMultiplier = 1.0f;
        frozenCompatibility_DmgMultiplier = 2.0f;

        burnedCompatibility_FinalEffect = new Burned_HealthState();
        coldCompatibility_FinalEffect = new HealthState();
        frozenCompatibility_FinalEffect = new Cold_HealthState();

    }

    public override void StartEffect()
    {
        base.StartEffect();

        if (lifeSystem.entityType == LifeSystem.EntityType.SHIELD)
            lifeSystem.dmgInc = shieldDmgInc;
        else
            lifeSystem.dmgInc = defaultDmgInc;

    }

    public override void EndEffect()
    {
        base.EndEffect();

        lifeSystem.dmgInc = 1.0f;
    }


}
