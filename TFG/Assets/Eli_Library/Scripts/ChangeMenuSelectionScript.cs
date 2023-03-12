using System;
using System.Collections;
using UnityEngine;

public class ChangeMenuSelectionScript : MonoBehaviour
{
    [Serializable]
    public class MenuSelection
    {
        public Menu_Manager[] usedMenus = new Menu_Manager[1];
        public CanvasGroup[] otherUI;
        public void SetActive(bool _active)
        {
            foreach (Menu_Manager usedMenu in usedMenus)
                usedMenu.gameObject.SetActive(_active);
            foreach (CanvasGroup other in otherUI)
                other.gameObject.SetActive(_active);
        }
    }

    public float appearSpeed = 0.25f, disappearSpeed = 0.15f;
    public bool autoSelectMenuStartOption = true;
    public bool setAutoSelectToFalseAfter1Iteration = false;
    public bool applyChangeSelectableNavigation = false;
    public MenuSelection menusToEnable;
    public MenuSelection menusToDisable;


    public void ChangeMenuSelection()
    {
        StartCoroutine(ChangeMenuSelectionCoroutine());
    }


    IEnumerator ChangeMenuSelectionCoroutine()
    {
        /// Disable Menus
        foreach (Menu_Manager usedMenu in menusToDisable.usedMenus)
            usedMenu.menuInUse = false;
        yield return LerpDisableMenuSelectionAlpha(menusToDisable, disappearSpeed);
        //


        /// Enable Menus
        menusToEnable.SetActive(true);
        yield return LerpEnableMenuSelectionAlpha(menusToEnable, appearSpeed);
        if (autoSelectMenuStartOption && menusToEnable.usedMenus.Length > 0)
        {
            menusToEnable.usedMenus[0].SetCurrentEventSystemSelection(menusToEnable.usedMenus[0].startSelectedOption.gameObject);
            menusToEnable.usedMenus[0].startSelectedOption.Select();
        }
        foreach (Menu_Manager usedMenu in menusToEnable.usedMenus)
            usedMenu.menuInUse = true;
        //

        if (applyChangeSelectableNavigation)
        {
            ChangeSelectableNavigationScript[] changeNavs = GetComponents<ChangeSelectableNavigationScript>();
            foreach (ChangeSelectableNavigationScript changeNav in changeNavs)
                changeNav.ChangeSelectableNavigation();
        }

        if (setAutoSelectToFalseAfter1Iteration)
            autoSelectMenuStartOption = false;


        /// Disable Menus
        menusToDisable.SetActive(false);
        //
    }

    IEnumerator LerpDisableMenuSelectionAlpha(MenuSelection _selection, float _lerpTime = 0.1f)
    {
        float targetAlpha = 0f;

        CanvasGroup[] usedMenusCG = new CanvasGroup[_selection.usedMenus.Length];
        float[] initUsedMenusAlpha = new float[_selection.usedMenus.Length];
        for(int i = 0; i < _selection.usedMenus.Length; i++)
        {
            if (_selection.usedMenus[i].gameObject.activeSelf)
            {
                usedMenusCG[i] = _selection.usedMenus[i].GetComponent<CanvasGroup>();
                initUsedMenusAlpha[i] = usedMenusCG[i].alpha;
            }
        }


        float[] initOtherUIAlpha = new float[_selection.otherUI.Length];
        for (int i = 0; i < _selection.otherUI.Length; i++)
        {
            if (_selection.otherUI[i].gameObject.activeSelf)
                initOtherUIAlpha[i] = _selection.otherUI[i].alpha;
        }

        float timer = 0, maxTime = _lerpTime;
        while(timer < maxTime)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            float lerpValue = timer / maxTime;

            for (int i = 0; i < _selection.usedMenus.Length; i++)
            {
                if (_selection.usedMenus[i].gameObject.activeSelf)
                    usedMenusCG[i].alpha = Mathf.Lerp(initUsedMenusAlpha[i], targetAlpha, lerpValue);
            }
            for (int i = 0; i < _selection.otherUI.Length; i++)
            {
                if (_selection.otherUI[i].gameObject.activeSelf)
                    _selection.otherUI[i].alpha = Mathf.Lerp(initOtherUIAlpha[i], targetAlpha, lerpValue);
            }
        }

        yield return new WaitForEndOfFrame();
        for (int i = 0; i < _selection.usedMenus.Length; i++)
        {
            if (_selection.usedMenus[i].gameObject.activeSelf)
                usedMenusCG[i].alpha = targetAlpha;
        }
        for (int i = 0; i < _selection.otherUI.Length; i++)
        {
            if (_selection.otherUI[i].gameObject.activeSelf)
                _selection.otherUI[i].alpha = targetAlpha;
        }

    }

    IEnumerator LerpEnableMenuSelectionAlpha(MenuSelection _selection, float _lerpTime = 0.2f)
    {
        float initAlpha = 0f, targetAlpha = 1f;


        CanvasGroup[] usedMenusCG = new CanvasGroup[_selection.usedMenus.Length];
        for (int i = 0; i < _selection.usedMenus.Length; i++)
        {
            if (_selection.usedMenus[i].gameObject.activeSelf)
            {
                usedMenusCG[i] = _selection.usedMenus[i].GetComponent<CanvasGroup>();
                usedMenusCG[i].alpha = initAlpha;
            }
        }

        //float[] targetSubMenusAlpha = new float[_selection.subMenus.Length];
        for (int i = 0; i < _selection.otherUI.Length; i++)
        {
            if (_selection.otherUI[i].gameObject.activeSelf)
            {
                //targetSubMenusAlpha[i] = _selection.subMenus[i].alpha;
                _selection.otherUI[i].alpha = initAlpha;
            }
        }

        float timer = 0, maxTime = _lerpTime;
        while (timer < maxTime)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            float lerpValue = timer / maxTime;

            for (int i = 0; i < _selection.usedMenus.Length; i++)
            {
                if (_selection.usedMenus[i].gameObject.activeSelf)
                    usedMenusCG[i].alpha = Mathf.Lerp(initAlpha, targetAlpha, lerpValue);
            }
            for (int i = 0; i < _selection.otherUI.Length; i++)
            {
                if (_selection.otherUI[i].gameObject.activeSelf)
                    _selection.otherUI[i].alpha = Mathf.Lerp(initAlpha, targetAlpha, lerpValue);
            }
        }

        yield return new WaitForEndOfFrame();
        for (int i = 0; i < _selection.usedMenus.Length; i++)
        {
            if (_selection.usedMenus[i].gameObject.activeSelf)
                usedMenusCG[i].alpha = targetAlpha;
        }
        for (int i = 0; i < _selection.otherUI.Length; i++)
        {
            if (_selection.otherUI[i].gameObject.activeSelf)
                _selection.otherUI[i].alpha = targetAlpha;
        }

    }

}
