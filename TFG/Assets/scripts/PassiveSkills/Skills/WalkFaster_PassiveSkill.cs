using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkFaster_PassiveSkill : PassiveSkill_Base
{
    const float MAX_UPGRADE_NUMBER = 5;
    const float WALK_SPEED_INCREMENT = 0.05f;

    public WalkFaster_PassiveSkill()
    {
        skillType = SkillType.WALK_SPEED;
        maxLevel = -1;
        name = "Agile foot";
        description = "Increase your walk speed!";
    }

    public override void Init(Transform _playerRef)
    {
        base.Init(_playerRef);
        AddLevelEvent();
    }


    public override void UpdateCall()
    {
        base.UpdateCall();
    }


    internal override void AddLevelEvent()
    {
        base.AddLevelEvent();

        PlayerMovement movementScript = playerRef.GetComponent<PlayerMovement>();
        float maxSpeed = movementScript.baseSpeed + (WALK_SPEED_INCREMENT * MAX_UPGRADE_NUMBER);

        if (movementScript.speedMultiplier < maxSpeed)
            movementScript.speedMultiplier += WALK_SPEED_INCREMENT;
    }
}
