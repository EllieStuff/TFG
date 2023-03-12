using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class NavigationGridChild : MonoBehaviour
{
    [SerializeField] Vector2Int gridId = -Vector2Int.one;

    //[SerializeField] Selectable rightSelectable;
    //[SerializeField] Selectable leftSelectable, upSelectable, downSelectable;
    [Serializable]
    struct NavigationAccessSelectables
    {
        public Selectable rightSelectable, leftSelectable, upSelectable, downSelectable;
    }
    [Space]
    [Header("OverWrite Navigation accesses")]
    [SerializeField] NavigationAccessSelectables navAccesses;


    internal void Initialize(NavigationGridLayout _navGrid, Vector2Int _gridId)
    {
        gridId = _gridId;

        if(_gridId.x < 0 || _gridId.y < 0 || _navGrid == null)
        {
            Debug.LogWarning("Grid Id was not assigned properly. Check that you have a NavigationGridLayout component in the parent.");
        }
        else
        {
            if (_navGrid == null)
                Debug.Log(1);
            if (_navGrid.GetNavigationChildById(gridId) == null)
                Debug.Log(2);
            Selectable thisSelectable = _navGrid.GetNavigationChildById(gridId).GetComponent<Selectable>();
            if (thisSelectable == null)
                Debug.Log(3);

            if (_navGrid.navigationMode == NavigationGridLayout.NavigationMode.BY_NAVIGATION_GRID)
            {
                Vector2Int
                    rightId = new Vector2Int(gridId.x + 1, gridId.y),
                    leftId = new Vector2Int(gridId.x - 1, gridId.y),
                    upId = new Vector2Int(gridId.x, gridId.y - 1),
                    downId = new Vector2Int(gridId.x, gridId.y + 1);
                if (leftId.x < 0) leftId.x = _navGrid.columns - 1;
                if (rightId.x >= _navGrid.columns) rightId.x = 0;
                if (upId.y < 0) upId.y = _navGrid.rows - 1;
                if (downId.y >= _navGrid.rows) downId.y = 0;

                if (_navGrid.LastRowNumOfGridElementsDiff > 0)
                {
                    //if (_navGrid.GetNavigationChildById(leftId) == null) leftId = gridId;
                    //if (_navGrid.GetNavigationChildById(rightId) == null) rightId = gridId;
                    //if (_navGrid.GetNavigationChildById(upId) == null) upId = gridId;
                    //if (_navGrid.GetNavigationChildById(downId) == null) downId = gridId;

                    int fixedXId = _navGrid.columns - _navGrid.LastRowNumOfGridElementsDiff - 1;
                    if (_navGrid.GetNavigationChildById(leftId) == null)
                        leftId.x = fixedXId;
                    if (_navGrid.GetNavigationChildById(rightId) == null)
                        rightId.x = fixedXId;
                    if (_navGrid.GetNavigationChildById(upId) == null)
                        upId.x = fixedXId;
                    if (_navGrid.GetNavigationChildById(downId) == null)
                        downId.x = fixedXId;
                }


                Navigation nav = thisSelectable.navigation;
                nav.mode = Navigation.Mode.Explicit;
                nav.selectOnRight = GetNavigationSelection(_navGrid, navAccesses.rightSelectable, rightId);
                nav.selectOnLeft = GetNavigationSelection(_navGrid, navAccesses.leftSelectable, leftId);
                nav.selectOnUp = GetNavigationSelection(_navGrid, navAccesses.upSelectable, upId);
                nav.selectOnDown = GetNavigationSelection(_navGrid, navAccesses.downSelectable, downId);

                //if(_navGrid.navConnections.rightNavigationGrid != null && gridId.x == 0 && navAccesses.rightSelectable == null) { }
                if (ValidNavigationGridConnection(_navGrid.navConnections.rightNavigationGrid, navAccesses.rightSelectable, gridId.x, _navGrid.columns - 1))
                    nav.selectOnRight = GetNavigationGridConnection(_navGrid.navConnections.rightNavigationGrid, false, 0);
                if (ValidNavigationGridConnection(_navGrid.navConnections.leftNavigationGrid, navAccesses.leftSelectable, gridId.x, 0))
                    nav.selectOnLeft = GetNavigationGridConnection(_navGrid.navConnections.leftNavigationGrid, false, _navGrid.navConnections.leftNavigationGrid.columns - 1);
                if (ValidNavigationGridConnection(_navGrid.navConnections.upNavigationGrid, navAccesses.upSelectable, gridId.y, 0))
                    nav.selectOnUp = GetNavigationGridConnection(_navGrid.navConnections.upNavigationGrid, true, _navGrid.navConnections.upNavigationGrid.rows - 1);
                if (ValidNavigationGridConnection(_navGrid.navConnections.downNavigationGrid, navAccesses.downSelectable, gridId.y, _navGrid.rows - 1))
                    nav.selectOnDown = GetNavigationGridConnection(_navGrid.navConnections.downNavigationGrid, true, 0);

                thisSelectable.navigation = nav;

            }
            else if(_navGrid.navigationMode == NavigationGridLayout.NavigationMode.IGNORE)
            {
                //Do nothing
            }
            else
            {
                Navigation nav = thisSelectable.navigation;
                nav.mode = (Navigation.Mode)_navGrid.navigationMode;
                thisSelectable.navigation = nav;
            }

        }

    }


    Selectable GetNavigationSelection(NavigationGridLayout _navGrid, Selectable _overWriteSelectable, Vector2Int _id)
    {
        if (_overWriteSelectable != null) return _overWriteSelectable;
        else if (_id != gridId) return _navGrid.GetNavigationChildById(_id).GetComponent<Selectable>();
        else return null;
    }

    bool ValidNavigationGridConnection(NavigationGridLayout _navGridToLink, Selectable _overWriteSelectable, int _id, int _validId)
    {
        return _navGridToLink != null && _id == _validId && _overWriteSelectable == null;
    }
    Selectable GetNavigationGridConnection(NavigationGridLayout _navGridToLink, bool _iteratesColumns, int _nonIteratedLine)
    {
        Transform currGridChild = null;
        float bestDistance = 999999;

        int countLimit;
        if (_iteratesColumns) countLimit = _navGridToLink.columns;
        else countLimit = _navGridToLink.rows;
        for(int i = 0; i < countLimit; i++)
        {
            Transform newChild = null;
            if (_iteratesColumns) newChild = _navGridToLink.GetNavigationChildById(new Vector2Int(i, _nonIteratedLine));
            else newChild = _navGridToLink.GetNavigationChildById(new Vector2Int(_nonIteratedLine, i));
            if (newChild == null) continue;

            float newDist = Vector3.Distance(transform.position, newChild.position);
            if (newDist < bestDistance)
            {
                bestDistance = newDist;
                currGridChild = newChild;
            }
        }

        if (currGridChild == null)
            return null;
        else
            return currGridChild.GetComponent<Selectable>();

    }

}
