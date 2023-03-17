using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PassiveSkill_Base
{
    public enum SkillType { IMPROVE_LIFE, IMPROVE_ATTACK_SPEED, IMPROVE_ATTACK_DAMAGE, INCREASE_PROJECTILE_AMOUNT, VAMPIRE, QUICK_CONJURING, HEAL, PIERCE, WALK_SPEED, FIRST_STRIKE, CRITICAL_CHANCE, COUNT }

    internal SkillType skillType;
    internal Transform playerRef;
    int level = 0;
    protected int maxLevel = 3;

    internal string name;
    internal string initialDescription, improvementDescription;

    internal int Level { get { return level; } }
    internal bool CanBeImproved { get { return level < maxLevel || maxLevel < 0; } }

    public virtual void Init(Transform _playerRef)
    {
        playerRef = _playerRef;
        level++;
    }


    public virtual void UpdateCall()
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

    internal virtual void AddLevelEvent()
    {

    }

}
