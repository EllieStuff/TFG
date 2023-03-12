using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class SetDescriptionScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] SetDescriptionScript_Manager manager;
    [SerializeField] string description = "";
    [SerializeField] bool activateOnHover = true;

    UIFeedback_Base uiFeedback;
    bool wasClicked = false;


    private void Start()
    {
        uiFeedback = GetComponent<UIFeedback_Base>();
    }

    private void Update()
    {
        if(uiFeedback.clicked && !wasClicked)
        {
            SetDescription();
            wasClicked = true;
        }
        else if(!uiFeedback.clicked && wasClicked)
        {
            wasClicked = false;
        }
    }


    public void SetDescription()
    {
        manager.SetDescription(description);
    }

    public bool DescriptionDisplayed()
    {
        return manager.targetTMPro.text == description;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (activateOnHover)
            manager.targetTMPro.text = description;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (activateOnHover)
            manager.ResetDescription();
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (activateOnHover)
            manager.targetTMPro.text = description;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (activateOnHover)
            manager.ResetDescription();
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        SetDescription();
    }


}
