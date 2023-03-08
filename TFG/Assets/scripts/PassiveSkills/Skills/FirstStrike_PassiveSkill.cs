using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstStrike_PassiveSkill : PassiveSkill_Base
{
    const int MAX_UPGRADE_NUMBER = 5;

    public FirstStrike_PassiveSkill()
    {
        skillType = SkillType.FIRST_STRIKE;
        maxLevel = -1;
        name = "First Strike";
        description = "Increase your damage with every element switch!";
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

        if(playerAttack.critSwapLevel < MAX_UPGRADE_NUMBER)
        {
            playerAttack.damageIncreaseByAbilitySwap = true;
            playerAttack.critSwapLevel += 1;
        }
    }
}