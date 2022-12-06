using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrongBlow_HealthState : HealthState
{
    public StrongBlow_HealthState()
    {
        state = HealthState.Effect.STRONG_BLOW;
    }
    public override void Init(LifeSystem _lifeSystem)
    {
        base.Init(_lifeSystem);

        name = "StrongBlow State";
        state = HealthState.Effect.STRONG_BLOW;
        effectDuration = 1.0f;

        //Has no dmg multiplier compatibility

        //Has no compatibilities with this or other effects

    }

    public override void StartEffect()
    {
        base.StartEffect();
        if (lifeSystem.entityType == LifeSystem.EntityType.PLAYER) 
        {
            //Stun
        }
        if(lifeSystem.entityType == LifeSystem.EntityType.ENEMY)
        {
            //Stun
        }

        //Se tiene que desactivar en cuanto se active
        EndEffect();
    }

    public override void EndEffect()
    {
        base.EndEffect();
    }

}
