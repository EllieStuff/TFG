using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] GameObject options, credits;
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] ChangeMenuSelectionScript deactivateOptions, deactivateCredits;
    [SerializeField] Image bg;

    Vector3 initBgPos;
    float bgSpeed = 100f, sinAmplitude = 3f;


    private void Start()
    {
        initBgPos = bg.transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (credits.activeSelf) deactivateCredits.ChangeMenuSelection();
            else if (options.activeSelf)
            {
                if(!resolutionDropdown.IsExpanded)
                    deactivateOptions.ChangeMenuSelection();
            }
        }


        bg.transform.position = new Vector3(initBgPos.x, initBgPos.y + Mathf.Sin(Time.time * sinAmplitude) * bgSpeed * Time.deltaTime, initBgPos.z);
    }


    public void Play()
    {
        CustomSceneManager.Instance.ChangeScene(1);
    }

    public void Exit()
    {
        Application.Quit();
    }

}
