using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseElements
{
    [SerializeField] internal PlayerAttack.AttackData.Type attackType;
    Dictionary<PlayerAttack.AttackData.Type, float> compatibilitiesData;


    public BaseElements() { }

    static BaseElements GetElementByType(PlayerAttack.AttackData.Type _type)
    {
        //ToDo
        return new BaseElements();
    }

}
