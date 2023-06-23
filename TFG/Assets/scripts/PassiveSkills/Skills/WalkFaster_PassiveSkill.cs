using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkFaster_PassiveSkill : PassiveSkill_Base
{
    const float WALK_SPEED_INCREMENT = 0.04f;

    public WalkFaster_PassiveSkill()
    {
        skillType = SkillType.WALK_SPEED;
        maxLevel = 3;
        basePrice = 3500;
        priceInc = 500;
        name = "Agile foot";
        initialDescription = "Increase your movement speed by 10%!";
        improvementDescription = initialDescription;
    }

    public override void Init(Transform _playerRef)
    {
        base.Init(_playerRef);
        AddLevelEvent();
    }


    public override void Update_Call()
    {
        base.Update_Call();
    }


    protected override void AddLevelEvent()
    {
        base.AddLevelEvent();
        PlayerMovement movementScript = playerRef.GetComponent<PlayerMovement>();
        movementScript.speedMultiplier += WALK_SPEED_INCREMENT;
    }
}
