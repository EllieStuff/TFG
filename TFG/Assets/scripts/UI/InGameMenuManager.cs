using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class InGameMenuManager : MonoBehaviour
{
    [SerializeField] GameObject inGameMenu, shop, options, levelCompleted;
    [SerializeField] ChangeMenuSelectionScript activateMenuScript, deactivateMenuScript,
        deactivateShopScript, deactivateOptionsScript, activateLevelCompleted;
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] BaseEnemyScript finalBossRef;

    bool levelCompletedFlag = false;

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
                if (shop.activeSelf)
                {
                    deactivateShopScript.ChangeMenuSelection();
                }
                else if (options.activeSelf)
                {
                    if(!resolutionDropdown.IsExpanded) deactivateOptionsScript.ChangeMenuSelection();
                }
                else
                {
                    Time.timeScale = 1;
                    deactivateMenuScript.ChangeMenuSelection();
                }
            }
        }

        if(!levelCompletedFlag)
        {
            if (finalBossRef != null && finalBossRef.state.Equals(BaseEnemyScript.States.DEATH))
            {
                levelCompletedFlag = true;
                StartCoroutine(ActivateLevelCompletedScreen());
            }
        }

    }


    public void Continue()
    {
        Time.timeScale = 1f;
        deactivateMenuScript.ChangeMenuSelection();
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        Destroy(GameObject.FindGameObjectWithTag("save"));
        CustomSceneManager.Instance.ChangeScene("Main Menu");
    }

    public void PlayAgain()
    {
        Time.timeScale = 1f;
        Destroy(GameObject.FindGameObjectWithTag("save"));
        CustomSceneManager.Instance.ChangeScene(SceneManager.GetActiveScene().name);
    }


    IEnumerator ActivateLevelCompletedScreen()
    {
        yield return new WaitForSeconds(2f);
        activateLevelCompleted.ChangeMenuSelection();
        Time.timeScale = 0.0000001f;
    }

}
