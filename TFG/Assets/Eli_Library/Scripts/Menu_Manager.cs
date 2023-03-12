using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class Menu_Manager : MonoBehaviour
{
    //[Serializable] public class OptionCompatibility { public string compatibilityName = "Name"; public UIFeedback[] compatibleOptions = new UIFeedback[0]; }

    [SerializeField] internal Transform optionsRef;
    [SerializeField] internal UIFeedback_Base startSelectedOption;
    [SerializeField] bool moveStartOptionWithCursor = false;
    [SerializeField] bool usesStartClickedOptions = false;
    [SerializeField] internal UIFeedback_Base[] startClickedOptions = new UIFeedback_Base[0];
    [SerializeField] internal bool menuInUse = false;
    [SerializeField] bool preserveSelectedOptions = false;
    [SerializeField] internal string acceptBttn = "Submit", cancelBttn = "Cancel";
    [Tooltip("Change menu data will deal with this menu as the 'Main' one and the ones opened with 'OpenSubMenuScript' as the 'Subs'.")]
    [SerializeField] ChangeMenuData changeMenuData = new ChangeMenuData(1, 1, 0, 0, true, true);
    //[SerializeField] OptionCompatibility[] optionsCompatibility = new OptionCompatibility[0];
    //[SerializeField] bool isSubMenu = false;

    [Tooltip("Mainer menu in Menu Hierarchy")]
    internal Menu_Manager mainMenuRef = null;
    //internal GameObject mainMenuOptionRef = null;
    internal Menu_Manager currSubMenu = null;
    //UIFeedback[] options;
    internal List<UIFeedback_Base> currOptions = new List<UIFeedback_Base>();


    private void Awake()
    {
        if(startSelectedOption != null)
            SetCurrentEventSystemSelection(startSelectedOption.gameObject);
        if (usesStartClickedOptions)
            SelectStartClickedOptions();

        //currOptions = new List<UIFeedback_Base>();

        //options = new UIFeedback[optionsRef.childCount];
        //for(int i = 0; i < options.Length; i++)
        //    options[i] = optionsRef.GetChild(i).GetComponent<UIFeedback>();

        //currOptionGO = currOption.gameObject;
        //SelectFirstBodyPart();
    }

    private void Update()
    {
        if (menuInUse && mainMenuRef != null && Input.GetButtonDown(cancelBttn))
        {
            GoBackToMainMenu();
        }
    }


    public void UnSelectOption(UIFeedback_Base _option)
    {
        _option.SetKeepSelected(false);
        _option.UnSelect();
        _option.UnClick();
        currOptions.Remove(_option);

        if (usesStartClickedOptions)
            startClickedOptions = currOptions.ToArray();
    }

    public void SelectOption(UIFeedback_Base _option)
    {
        if (startSelectedOption != null && _option != null)
        {
            for(int i = 0; i < currOptions.Count; i++)
            {
                if (!_option.IsCompatibleOption(currOptions[i]))
                {
                    UnSelectOption(currOptions[i]);
                    i--;
                }
            }
        }
        _option.SetKeepSelected(true);
        currOptions.Add(_option);

        if (usesStartClickedOptions)
            startClickedOptions = currOptions.ToArray();
        if (moveStartOptionWithCursor)
            startSelectedOption = _option;

    }

    public void SelectStartClickedOptions()
    {
        foreach(UIFeedback_Base option in startClickedOptions)
        {
            option.SetKeepSelected(true);
            currOptions.Add(option);
        }
    }

    //void SelectFirstBodyPart()
    //{
    //    if (currOption == null) return;

    //    StartCoroutine(LerpGroupAlpha(currOptionGO, 0, 1));
    //    //UIFeedback uiFeedback = selectedBodyPart.transform.GetChild(0).GetComponent<UIFeedback>();
    //    //uiFeedback.Select();
    //    //uiFeedback.Click();
    //}

    public void SelectSubMenu(Menu_Manager _subMenu, UIFeedback_Base _uiFeedbackOrigin)
    {
        currSubMenu.StopAllCoroutines();
        _subMenu.StopAllCoroutines();

        _subMenu.mainMenuRef = this;
        //_subMenu.mainMenuOptionRef = currOption.gameObject;
        //_subMenu.gameObject.SetActive(true);
        //_subMenu.GetComponent<CanvasGroup>().alpha = 0;
        StartCoroutine(SelectSubMenuCoroutine(_subMenu, _uiFeedbackOrigin));
    }

    public void GoBackToMainMenu()
    {
        if (mainMenuRef == null) return;

        this.StopAllCoroutines();
        mainMenuRef.StopAllCoroutines();
        StartCoroutine(SelectMainMenuCoroutine());
    }


    public IEnumerator SelectMainMenuCoroutine()
    {
        CanvasGroup subMenuGroup = GetComponent<CanvasGroup>();
        if(mainMenuRef.changeMenuData.subMenuEndActive)
            yield return LerpGroupAlpha(subMenuGroup, subMenuGroup.alpha, mainMenuRef.changeMenuData.subMenuInactiveAlpha);
        else
            yield return LerpGroupAlpha(subMenuGroup, subMenuGroup.alpha, 0);

        mainMenuRef.gameObject.SetActive(true);
        CanvasGroup mainMenuGroup = mainMenuRef.GetComponent<CanvasGroup>();
        yield return LerpGroupAlpha(mainMenuGroup, mainMenuGroup.alpha, mainMenuRef.changeMenuData.mainMenuActiveAlpha);

        menuInUse = false;
        mainMenuRef.menuInUse = true;
        SetCurrentEventSystemSelection(mainMenuRef.startSelectedOption.gameObject);
        if (mainMenuRef.preserveSelectedOptions)
        {
            foreach (UIFeedback_Base mainMenuSelectedOption in mainMenuRef.currOptions)
            {
                mainMenuSelectedOption.SetKeepSelected(true);
                mainMenuSelectedOption.UnClick();
                //mainMenuSelectedOption.UnClick_Visuals();
                //mainMenuSelectedOption.Select();
            }
        }
        if (!mainMenuRef.preserveSelectedOptions)
        {
            foreach (UIFeedback_Base mainMenuCurrOption in mainMenuRef.currOptions)
            {
                //mainMenuCurrOption.ResetValues();
                mainMenuCurrOption.SetKeepSelected(false);
                mainMenuCurrOption.UnClick();
                mainMenuCurrOption.UnSelect();
            }
        }
        if (!preserveSelectedOptions)
        {
            foreach (UIFeedback_Base subMenuCurrOption in currOptions)
            {
                //subMenuCurrOption.ResetValues();
                subMenuCurrOption.SetKeepSelected(false);
                subMenuCurrOption.UnClick();
                subMenuCurrOption.UnSelect();
            }
        }
        //mainMenuRef.startOption.UnClick();
        //SelectOption(startOption);
        //UnSelectOption(startOption);
        mainMenuRef.startSelectedOption.Select();

        if (!mainMenuRef.changeMenuData.subMenuEndActive)
            gameObject.SetActive(false);
    }

    public IEnumerator SelectSubMenuCoroutine(Menu_Manager _subMenu, UIFeedback_Base _uiFeedbackOrigin)
    {
        if (currSubMenu != null && currSubMenu.gameObject.activeSelf)
        {
            yield return UnselectOverlappingMenu(currSubMenu);
            currSubMenu.gameObject.SetActive(false);
        }

        //currOptionGO = _subMenu;
        //currOption = currOptionGO.GetComponent<UIFeedback>();
        currSubMenu = _subMenu;
        _subMenu.gameObject.SetActive(true);
        CanvasGroup subMenuGroup = _subMenu.GetComponent<CanvasGroup>();
        yield return LerpGroupAlpha(subMenuGroup, subMenuGroup.alpha, changeMenuData.subMenuActiveAlpha);
        //UIFeedback uiFeedback = _bodyPart.transform.GetChild(0).GetComponent<UIFeedback>();
        //uiFeedback.Select();
        //uiFeedback.Click();

        menuInUse = false;
        _subMenu.menuInUse = true;
        _subMenu.SetCurrentEventSystemSelection(_subMenu.startSelectedOption.gameObject);
        //if (preserveSelectedOptions)
        //{
        //    foreach (UIFeedback_Base mainMenuSelectedOption in currOptions)
        //    {
        //        mainMenuSelectedOption.SetKeepSelected(true);
        //    }
        //}
        //if (!_subMenu.preserveSelectedOptions)
        //{
        //    foreach (UIFeedback_Base subMenuSelectedOption in _subMenu.currOptions)
        //    {
        //        subMenuSelectedOption.SetKeepSelected(false);
        //        subMenuSelectedOption.UnClick();
        //        subMenuSelectedOption.UnSelect();
        //    }
        //}
        _subMenu.startSelectedOption.Select();

        //SelectOption(_uiFeedbackOrigin);
        CanvasGroup mainMenuGroup = GetComponent<CanvasGroup>();
        if (changeMenuData.mainMenuEndActive)
            yield return LerpGroupAlpha(mainMenuGroup, mainMenuGroup.alpha, changeMenuData.mainMenuInactiveAlpha);
        else
        {
            yield return LerpGroupAlpha(mainMenuGroup, mainMenuGroup.alpha, 0);
            gameObject.SetActive(false);
        }
    }

    public void SetCurrentEventSystemSelection(GameObject _eventSystemSelection)
    {
        if (_eventSystemSelection == null || !_eventSystemSelection.activeInHierarchy)
        {
            Debug.LogWarning("The object was null or inactive. Make sure the object is active in the hierarchy before calling this method.");
        }
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_eventSystemSelection);
    }


    IEnumerator UnselectOverlappingMenu(Menu_Manager _menu)
    {
        CanvasGroup menuGroup = _menu.GetComponent<CanvasGroup>();
        yield return LerpGroupAlpha(menuGroup, menuGroup.alpha, 0);
        _menu.gameObject.SetActive(false);

        //for (int i = 0; i < _bodyPart.transform.childCount; i++)
        //{
        //    UIFeedback uiFeedback = _bodyPart.transform.GetChild(i).GetComponent<UIFeedback>();
        //    uiFeedback.UnSelect();
        //    uiFeedback.UnClick();
        //}
    }


    IEnumerator LerpGroupAlpha(GameObject _groupGO, float _initAlpha, float _finalAlpha, float _lerpTime = 0.1f)
    {
        CanvasGroup group = _groupGO.GetComponent<CanvasGroup>();
        group.alpha = _initAlpha;

        float timer = 0, maxTime = _lerpTime;
        while (timer < maxTime)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            group.alpha = Mathf.Lerp(_initAlpha, _finalAlpha, timer / maxTime);
        }
        yield return new WaitForEndOfFrame();
        group.alpha = _finalAlpha;
    }
    IEnumerator LerpGroupAlpha(CanvasGroup _group, float _initAlpha, float _finalAlpha, float _lerpTime = 0.1f)
    {
        _group.alpha = _initAlpha;

        float timer = 0, maxTime = _lerpTime;
        while (timer < maxTime)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            _group.alpha = Mathf.Lerp(_initAlpha, _finalAlpha, timer / maxTime);
        }
        yield return new WaitForEndOfFrame();
        _group.alpha = _finalAlpha;
    }

}
