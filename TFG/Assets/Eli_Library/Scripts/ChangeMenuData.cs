using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ChangeMenuData
{
    public float mainMenuActiveAlpha = 1, subMenuActiveAlpha = 1;
    public float mainMenuInactiveAlpha = 0, subMenuInactiveAlpha = 0;
    public bool mainMenuEndActive = true, subMenuEndActive = true;

    public ChangeMenuData(float _mainMenuActiveAlpha, float _subMenuActiveAlpha, float _mainMenuInactiveAlpha, float _subMenuInactiveAlpha, 
        bool _mainMenuEndActive, bool _subMenuEndActive)
    {
        mainMenuActiveAlpha = _mainMenuActiveAlpha;
        subMenuActiveAlpha = _subMenuActiveAlpha;
        mainMenuInactiveAlpha = _mainMenuInactiveAlpha;
        subMenuInactiveAlpha = _subMenuInactiveAlpha;
        mainMenuEndActive = _mainMenuEndActive;
        subMenuEndActive = _subMenuEndActive;
    }


}
