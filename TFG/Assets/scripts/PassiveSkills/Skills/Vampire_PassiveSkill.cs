using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vampire_PassiveSkill : PassiveSkill_Base
{
    const float STEAL_LIFE_PERCENTAGE_TO_ADD = 0.05f;

    public Vampire_PassiveSkill()
    {
        skillType = SkillType.VAMPIRE;
        maxLevel = 5;
        appearRatio = 0.6f;
        basePrice = 30000;
        priceInc = 14000;
        name = "Vampire";
        //initialDescription = "Steals your enemies life by damaging them with effective elements!";
        //improvementDescription = "Increases the life stolen from your enemies by damaging them with effective elements!";
        initialDescription = "Steals 5% of your enemies life by hitting them with effective elements!";
        improvementDescription = "Increases by 5% the life stolen from your enemies when hitting them with effective elements!";
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
        playerAttack.stealLifePercentage += STEAL_LIFE_PERCENTAGE_TO_ADD;
    }

}
