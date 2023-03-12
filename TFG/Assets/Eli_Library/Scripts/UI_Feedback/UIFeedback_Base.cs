using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIFeedback_Base : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IPointerClickHandler, IDeselectHandler, IPointerDownHandler
{
    [Header("Base Feedback")]
    [Space(2)]
    [SerializeField] internal Image feedbackImage;
    [SerializeField] internal bool startClicked = false, ignoreInputs = false, keepSelected = false, unclickByDoubleClick = false;
    [SerializeField] internal UIFeedback_Base[] compatibleOptions = new UIFeedback_Base[0];

    [SerializeField] internal bool selected = false, clicked = false;
    internal Menu_Manager menuManager;

    string acceptButton = "Submit";

    bool MenuInUse { get { return menuManager != null && menuManager.menuInUse; } }


    void Start()
    {
        Start_Call();
    }
    internal virtual void Start_Call()
    {
        Init();

        if (startClicked) //Fer un clickedOnEnable??
        {
            StartClicked_Call();
        }

    }
    internal virtual void Init()
    {
        menuManager = transform.parent.parent.GetComponent<Menu_Manager>();
        if (menuManager != null)
            acceptButton = menuManager.acceptBttn;
    }
    internal virtual void StartClicked_Call()
    {
        selected = clicked = true;
    }

    private void OnEnable() { }
    internal virtual void OnEnable_Call() { }


    private void Update()
    {
        Update_Call();
    }
    internal virtual void Update_Call()
    {
        if (!MenuInUse && keepSelected) KeepSelected_Call();
        if (ignoreInputs || !MenuInUse) return;

        if (Input.GetButtonDown("Submit") && selected && !keepSelected)
        {
            Click();
        }
        else if (Input.GetButtonDown("Submit") && selected && keepSelected && unclickByDoubleClick)
        {
            UnClickByDoubleClick();
        }

        if (keepSelected)
        {
            KeepSelected_Call();
            return;
        }

        if (clicked && !selected && !keepSelected && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Mouse0)))
        {
            UnClick();
        }
    }
    internal virtual void KeepSelected_Call() { }


    public virtual void Select()
    {
        selected = true;
        if (gameObject.activeInHierarchy)
            Select_Visuals();
    }
    internal virtual void Select_Visuals() { }

    public virtual void UnSelect()
    {
        //if (keepSelected) return;

        selected = false;
        if (!clicked && !keepSelected)
        {
            if (gameObject.activeInHierarchy)
                UnSelect_Visuals();
        }
    }
    internal virtual void UnSelect_Visuals() { }

    public virtual void Click()
    {
        if (selected && keepSelected && clicked && unclickByDoubleClick)
        {
            UnClickByDoubleClick();
            return;
        }

        clicked = true;
        if(menuManager != null) menuManager.SelectOption(this);
        if (gameObject.activeInHierarchy)
            Click_Visuals();

    }
    internal virtual void Click_Visuals() { }

    public virtual void UnClick()
    {
        clicked = false;
        if (gameObject.activeInHierarchy)
            UnClick_Visuals();
    }
    internal virtual void UnClick_Visuals() { }
    internal virtual void UnClickByDoubleClick()
    {
        keepSelected = false;
        menuManager.UnSelectOption(this);
        UnClick();
        Select();
    }

    internal virtual void ResetValues()
    {
        startClicked = ignoreInputs = keepSelected = false;
        //selected = clicked = false;
    }


    public void SetIgnoreInputs(bool _ignoreInputs)
    {
        ignoreInputs = _ignoreInputs;
    }
    public void SetKeepSelected(bool _keepSelected)
    {
        keepSelected = _keepSelected;
    }
    public void SetCurrentEventSystemSelection(GameObject _eventSystemSelection)
    {
        if (_eventSystemSelection == null || !_eventSystemSelection.activeInHierarchy)
        {
            Debug.LogWarning("The object was null or inactive. Make sure the object is active in the hierarchy before calling this method.");
        }
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_eventSystemSelection);

        //UIFeedback selection = _eventSystemSelection.GetComponent<UIFeedback>();
        //selection.Select();
        //selection.Click();
    }
    public bool IsCompatibleOption(UIFeedback_Base _uiFeedback)
    {
        for(int i = 0; i < compatibleOptions.Length; i++)
        {
            if (compatibleOptions[i] == _uiFeedback)
                return true;
        }
        return false;
    }
    public UIFeedback_Base[] GetCompatibleOptions()
    {
        return compatibleOptions;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        OnPointerEnter_Call(eventData);
    }
    internal virtual void OnPointerEnter_Call(PointerEventData _eventData)
    {
        if (ignoreInputs) return;

        Select();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnPointerExit_Call(eventData);
    }
    internal virtual void OnPointerExit_Call(PointerEventData _eventData)
    {
        if (ignoreInputs) return;

        UnSelect();
    }

    public void OnSelect(BaseEventData eventData)
    {
        OnSelect_Call(eventData);
    }
    internal virtual void OnSelect_Call(BaseEventData _eventData)
    {
        if (ignoreInputs) return;

        if (!selected)
            Select();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        OnDeselect_Call(eventData);
    }
    internal virtual void OnDeselect_Call(BaseEventData _eventData)
    {
        if (ignoreInputs) return;

        UnSelect();
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        OnPointerClick_Call(eventData);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        //OnPointerClick_Call(eventData);
    }
    internal virtual void OnPointerClick_Call(PointerEventData _eventData)
    {
        if (ignoreInputs) return;

        Click();
    }

}
