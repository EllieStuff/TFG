using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageInGameMenu : MonoBehaviour
{
    [SerializeField] GameObject inGameMenu;
    [SerializeField] ChangeMenuSelectionScript activateMenuScript, deactivateMenuScript;


    private void Start()
    {
        //inGameMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (!inGameMenu.activeSelf) activateMenuScript.ChangeMenuSelection();
            else deactivateMenuScript.ChangeMenuSelection();
        }
    }


}
