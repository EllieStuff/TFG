using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstStrike_PassiveSkill : PassiveSkill_Base
{
    public FirstStrike_PassiveSkill()
    {
        skillType = SkillType.FIRST_STRIKE;
        maxLevel = 3;
        name = "First Strike";
        //initialDescription = "Increases your damage with every element switch!";
        //improvementDescription = "Increases the number of projectiles with damage increase for every element switch!";
        initialDescription = "Increases your damage by 20% with every element switch!";
        improvementDescription = "Increases the number of projectiles with a 20% damage increase for every element switch!";
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
        playerAttack.damageIncreaseByAbilitySwap = true;
        playerAttack.critSwapLevel = Level;
    }
}