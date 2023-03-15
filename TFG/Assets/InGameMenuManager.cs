using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenuManager : MonoBehaviour
{
    [SerializeField] GameObject inGameMenu, options;
    [SerializeField] ChangeMenuSelectionScript activateMenuScript, deactivateMenuScript, deactivateOptionsScript;


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (!inGameMenu.activeSelf)
            {
                Time.timeScale = 0.0000001f;
                activateMenuScript.ChangeMenuSelection();
            }
            else
            {
                if (options.activeSelf)
                {
                    deactivateOptionsScript.ChangeMenuSelection();
                }
                else
                {
                    Time.timeScale = 1;
                    deactivateMenuScript.ChangeMenuSelection();
                }
            }
        }
    }


    public void Continue()
    {
        Time.timeScale = 1f;
        deactivateMenuScript.ChangeMenuSelection();
    }

    public void Exit()
    {
        Time.timeScale = 1f;
        CustomSceneManager.Instance.ChangeScene("MainMenu Scene");
    }


}
