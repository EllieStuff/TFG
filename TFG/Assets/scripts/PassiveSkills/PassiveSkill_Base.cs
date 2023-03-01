using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PassiveSkill_Base
{
    public enum SkillType { IMPROVE_LIFE, HEAL, IMPROVE_ATTACK_SPEED, IMPROVE_ATTACK_DAMAGE, QUICK_CONJURING, VAMPIRE, COUNT }

    internal SkillType skillType;
    internal Transform playerRef;
    [SerializeField] int level = 1;
    internal int maxLevel = 3;

    internal string name;
    internal string description;

    internal int Level { get { return level; } }

    public virtual void Init(Transform _playerRef)
    {
        playerRef = _playerRef;
    }


    public virtual void UpdateCall()
    {

    }


    public virtual void AddLevel(int _lvlsToAdd)
    {
        if (level < maxLevel || maxLevel < 0)
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
