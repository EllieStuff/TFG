using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PassiveSkill_Base
{
    [SerializeField] int level = 1;
    internal int maxLevel = 3;


    public virtual void Init(Transform _playerRef)
    {

    }


    public virtual void UpdateCall()
    {

    }


    public virtual void AddLevel(int _lvlsToAdd)
    {
        if (level < maxLevel)
        {
            level += _lvlsToAdd;
        }
    }

}
