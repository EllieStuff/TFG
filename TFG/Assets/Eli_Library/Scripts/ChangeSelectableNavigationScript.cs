using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeSelectableNavigationScript : MonoBehaviour
{
    [System.Serializable]
    struct NavigationAccessSelectables
    {
        public Selectable rightSelectable, leftSelectable, upSelectable, downSelectable;
    }

    [SerializeField] Selectable target;
    [SerializeField] bool overWriteOldData = true;
    [SerializeField] bool overWriteWhenNull = false;
    [SerializeField] NavigationAccessSelectables navAccesses;


    public void ChangeSelectableNavigation()
    {
        Navigation nav = target.navigation;
        if (nav.mode != Navigation.Mode.Explicit)
            nav.mode = Navigation.Mode.Explicit;

        if (ValidSelectableChange(nav.selectOnRight, navAccesses.rightSelectable))
            nav.selectOnRight = navAccesses.rightSelectable;
        if (ValidSelectableChange(nav.selectOnLeft, navAccesses.leftSelectable))
            nav.selectOnLeft = navAccesses.leftSelectable;
        if (ValidSelectableChange(nav.selectOnUp, navAccesses.upSelectable))
            nav.selectOnUp = navAccesses.upSelectable;
        if (ValidSelectableChange(nav.selectOnDown, navAccesses.downSelectable))
            nav.selectOnDown = navAccesses.downSelectable;

        target.navigation = nav;
    }


    bool ValidSelectableChange(Selectable _oldSelectable, Selectable _newSelectable)
    {
        return (_oldSelectable == null || overWriteOldData) && (_newSelectable != null || overWriteWhenNull);
    }

}
