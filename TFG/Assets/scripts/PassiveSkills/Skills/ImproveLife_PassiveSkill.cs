using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImproveLife_PassiveSkill : PassiveSkill_Base
{
    float lifeToAdd = 100f;

    public ImproveLife_PassiveSkill()
    {
        skillType = SkillType.IMPROVE_LIFE;
        maxLevel = -1;
        name = "Life Improve";
        description = "Your maximum life gets improved!";
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
        playerLife.maxLife += lifeToAdd;
        playerLife.AddLife(lifeToAdd);
    }

}
