using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class OpenSubMenuScript : MonoBehaviour
{
    [SerializeField] Menu_Manager menuManager;
    [SerializeField] Menu_Manager subMenu;

    UIFeedback_Base uiFeedbackOrigin;


    // Start is called before the first frame update
    void Start()
    {
        uiFeedbackOrigin = GetComponent<UIFeedback_Base>();
    }


    public void OpenSubMenu()
    {
        //menuManager.StopAllCoroutines();
        //subMenu.StopAllCoroutines();

        //menuManager.SelectOption(uiFeedback);
        //subMenu.startOption.Select();
        subMenu.mainMenuRef = menuManager;

        StartCoroutine(menuManager.SelectSubMenuCoroutine(subMenu, uiFeedbackOrigin));
    }


}
