using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImproveLife_PassiveSkill : PassiveSkill_Base
{
    const float LIFE_TO_ADD = 300f;

    public ImproveLife_PassiveSkill()
    {
        skillType = SkillType.IMPROVE_LIFE;
        maxLevel = -1;
        name = "Life Improvement";
        initialDescription = "Your maximum life gets improved!";
        improvementDescription = initialDescription;
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
        LifeSystem playerLife = playerRef.GetComponent<LifeSystem>();
        playerLife.maxLife += LIFE_TO_ADD;
        playerLife.AddLife(LIFE_TO_ADD);
    }

}
