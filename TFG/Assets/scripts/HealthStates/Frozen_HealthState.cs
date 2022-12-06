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

        name = "Frozen State";
        state = HealthState.Effect.FROZEN;
        effectDuration = 8.0f;

        //No tiene damage multipliers

        compatibilityMap_FinalEffects.Add(Effect.BURNED, new HealthState());

    }

    public override void StartEffect()
    {
        if(lifeSystem.entityType == LifeSystem.EntityType.ENEMY)
        {
            BaseEnemyScript.EnemyType enemyType = lifeSystem.GetComponent<BaseEnemyScript>().enemyType;
            if (enemyType == BaseEnemyScript.EnemyType.LITTLE_SNAKE) effectDuration = 15f;
            else if (enemyType == BaseEnemyScript.EnemyType.SKINY_RAT) effectDuration = 8f;
            else if (enemyType == BaseEnemyScript.EnemyType.FAT_RAT || enemyType == BaseEnemyScript.EnemyType.BIG_SNAKE) 
                effectDuration = 5f;
            else if (enemyType == BaseEnemyScript.EnemyType.CROCODILE) effectDuration = 3f;
        }
        base.StartEffect();

        // TODO:
        // - Crear el Frozen_Feedback
        // - Congelar animación
        if (lifeSystem.entityType == LifeSystem.EntityType.PLAYER)
        {
            PlayerMovement player = lifeSystem.GetComponent<PlayerMovement>();
            player.canMove = player.canRotate = false;
        }
        else
        {
            BaseEnemyScript enemy = lifeSystem.GetComponent<BaseEnemyScript>();
            enemy.canMove = enemy.canRotate = false;
        }

    }

    public override void EndEffect()
    {
        //compatibilityMap_FinalEffects.Remove(Effect.BURNED);

        // TODO:
        // - Deshacer frozen feedback? Creo que se deshacia solo pero not sure ahora
        // - Descongelar animación
        if (lifeSystem.entityType == LifeSystem.EntityType.PLAYER)
        {
            PlayerMovement player = lifeSystem.GetComponent<PlayerMovement>();
            player.canMove = player.canRotate = true;
        }
        else
        {
            BaseEnemyScript enemy = lifeSystem.GetComponent<BaseEnemyScript>();
            enemy.canMove = enemy.canRotate = true;
        }

        base.EndEffect();
    }


}
