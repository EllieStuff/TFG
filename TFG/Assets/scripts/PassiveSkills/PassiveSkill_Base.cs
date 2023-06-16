using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PassiveSkill_Base
{
    public enum SkillType { IMPROVE_LIFE, IMPROVE_ATTACK_SPEED, IMPROVE_ATTACK_DAMAGE, /*INCREASE_PROJECTILE_AMOUNT,*/ VAMPIRE, QUICK_CONJURING, HEAL, PIERCE, WALK_SPEED, FIRST_STRIKE, CRITICAL_CHANCE, COUNT, NONE }

    [HideInInspector] public SkillType skillType;
    protected Transform playerRef;
    int level = 0;
    protected int maxLevel = 3;

    protected string name;
    protected string initialDescription, improvementDescription;
    protected int basePrice = 0, priceInc = 0;
    protected float appearRatio = 1f;

    public string Name { get { return name; } }
    public string Description { 
        get {
            if (level == 0) return initialDescription;
            else return improvementDescription;
        } 
    }
    public int Level { get { return level; } }
    public int MaxLevel { get { return maxLevel; } }
    public float AppearRatio { get { return appearRatio; } }
    public int Price { get { return basePrice + priceInc * level; } }
    public bool CanBeImproved { get { return level < maxLevel || maxLevel < 0; } }

    public virtual void Init(Transform _playerRef)
    {
        playerRef = _playerRef;
        level++;
    }


    public virtual void Update_Call()
    {

    }


    public virtual void AddLevel(int _lvlsToAdd)
    {
        if (CanBeImproved)
        {
            for (int i = 0; i < _lvlsToAdd; i++)
            {
                level++;
                AddLevelEvent();
            }
        }
    }

    public void SetShopLevel(int _level)
    {
        level = _level;
    }

    protected virtual void AddLevelEvent()
    {

    }

}
