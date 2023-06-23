using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pierce_PassiveSkill : PassiveSkill_Base
{
    int piercesToIncrease = 1;

    public Pierce_PassiveSkill()
    {
        skillType = SkillType.PIERCE;
        maxLevel = 3;
        name = "Pierce";
        initialDescription = "Your projectiles will pierce through your enemies!";
        improvementDescription = "Your projectiles will pierce through 1 more enemy!";
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
        PlayerAttack playerAttack = playerRef.GetComponent<PlayerAttack>();
        playerAttack.projectilePierceAmount += piercesToIncrease;
    }
}
