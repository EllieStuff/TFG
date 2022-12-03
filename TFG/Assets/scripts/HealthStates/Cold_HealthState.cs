using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cold_HealthState : HealthState
{
    [SerializeField]
    float
        playerSpeedInc = 0.5f,
        enemySpeedInc = 0.5f;


    public Cold_HealthState()
    {
        state = HealthState.Effect.COLD;
    }
    public override void Init(LifeSystem _lifeSystem)
    {
        base.Init(_lifeSystem);

        name = "Cold State";
        state = HealthState.Effect.COLD;
        effectDuration = 10.0f;

        burnedCompatibility_DmgMultiplier = 0.0f;
        coldCompatibility_DmgMultiplier = 0.5f;
        frozenCompatibility_DmgMultiplier = 0.5f;

        burnedCompatibility_FinalEffect = new HealthState();
        coldCompatibility_FinalEffect = new Frozen_HealthState();
        wetCompatibility_FinalEffect = new Frozen_HealthState();
        //frozenCompatibility_FinalEffect = new Frozen_HealthState();

    }

    public override void StartEffect()
    {
        base.StartEffect();

        if (lifeSystem.entityType == LifeSystem.EntityType.PLAYER)
            lifeSystem.GetComponent<PlayerMovement>().speedMultiplier = playerSpeedInc;
        else if (lifeSystem.entityType == LifeSystem.EntityType.ENEMY)
            lifeSystem.GetComponent<BaseEnemyScript>().speedMultiplier = enemySpeedInc;

    }

    public override void EndEffect()
    {
        base.EndEffect();

        if (lifeSystem.entityType == LifeSystem.EntityType.PLAYER)
            lifeSystem.GetComponent<PlayerMovement>().speedMultiplier = 1.0f;
        else if (lifeSystem.entityType == LifeSystem.EntityType.ENEMY)
            lifeSystem.GetComponent<BaseEnemyScript>().speedMultiplier = 1.0f;
    }


}
