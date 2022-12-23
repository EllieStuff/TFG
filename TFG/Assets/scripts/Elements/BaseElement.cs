using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseElements
{
    [SerializeField] internal ElementsManager.Elements attackType;
    Dictionary<ElementsManager.Elements, float> compatibilitiesData;


    public BaseElements() { }

    static BaseElements GetElementByType(ElementsManager.Elements _type)
    {
        //ToDo
        return new BaseElements();
    }

}
