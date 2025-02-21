using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] GameObject shop, options, credits;
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] ChangeMenuSelectionScript deactivateShop, deactivateOptions, deactivateCredits;
    [SerializeField] Image bg;

    Vector3 initBgPos, initCreditsPos, initOptionsPos;
    float bgSpeed = 100f, sinAmplitude = 3f;
    Transform creditsChild, optionsChild;

    private void Start()
    {

        initBgPos = bg.transform.position;
        creditsChild = credits.transform.GetChild(0);
        initCreditsPos = creditsChild.localPosition;
        optionsChild = options.transform.GetChild(0);
        initOptionsPos = optionsChild.localPosition;
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (credits.activeSelf) deactivateCredits.ChangeMenuSelection();
            else if (options.activeSelf)
            {
                if (!resolutionDropdown.IsExpanded)
                    deactivateOptions.ChangeMenuSelection();
            }
            else if (shop.activeSelf) deactivateShop.ChangeMenuSelection();
        }


        bg.transform.position = new Vector3(initBgPos.x, initBgPos.y + Mathf.Sin(Time.time * sinAmplitude) * bgSpeed * Time.deltaTime, initBgPos.z);
        if(credits.activeSelf) creditsChild.localPosition = new Vector3(initCreditsPos.x, initCreditsPos.y + Mathf.Sin(Time.time * sinAmplitude) * bgSpeed * Time.deltaTime, initCreditsPos.z);
        if(options.activeSelf) optionsChild.localPosition = new Vector3(initOptionsPos.x, initOptionsPos.y + Mathf.Sin(Time.time * sinAmplitude) * bgSpeed * Time.deltaTime, initOptionsPos.z);
    }


    public void Play()
    {
        if (PlayerPrefs.GetInt("TutorialHasPlayed", 0) <= 0)
            CustomSceneManager.Instance.ChangeScene("IntroVideo Scene");
        else
            CustomSceneManager.Instance.ChangeScene(2);
    }

    public void Exit()
    {
        Application.Quit();
    }

}
