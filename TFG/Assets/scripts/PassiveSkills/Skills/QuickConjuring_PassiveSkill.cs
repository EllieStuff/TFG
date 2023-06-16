using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickConjuring_PassiveSkill : PassiveSkill_Base
{
    const float SPEED_INCREASE = 0.35f;

    public QuickConjuring_PassiveSkill()
    {
        skillType = SkillType.QUICK_CONJURING;
        maxLevel = 3;
        basePrice = 24000;
        priceInc = 10500;
        name = "Agile Conjuration";
        //initialDescription = "Speed up your element casting time!";
        initialDescription = "Speed up your element casting time by 20%!";
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
        PlayerAttack playerAttack = playerRef.GetComponent<PlayerAttack>();
        playerAttack.changeAttackDelay -= SPEED_INCREASE;
    }

}
