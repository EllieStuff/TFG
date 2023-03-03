using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pierce_PassiveSkill : PassiveSkill_Base
{
    int piercesToIncrease = 1;

    public Pierce_PassiveSkill()
    {
        skillType = SkillType.PIERCE;
        maxLevel = -1;
        name = "Pierce";
        description = "Your projectiles will pierce throught your enemies!";
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
        PlayerAttack playerAttack = playerRef.GetComponent<PlayerAttack>();
        playerAttack.projectilePierceAmount += piercesToIncrease;
    }
}
