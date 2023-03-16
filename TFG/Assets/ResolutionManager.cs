using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResolutionManager : MonoBehaviour
{
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] Toggle fullScreenToggle;

    Resolution[] resolutions;

    // Start is called before the first frame update
    void Awake()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int resolutionIdx = -1, notFoundResolutionException = 0;
        bool resolutionInited = PlayerPrefs.GetInt("ResolutionValue", -1) >= 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string resolutionOption = resolutions[i].width + "x" + resolutions[i].height + " " + resolutions[i].refreshRate + " Hz";
            options.Add(resolutionOption);
            if (!resolutionInited && resolutions[i].width == 1920 && resolutions[i].height == 1080)
            {
                resolutionIdx = i;
            }
            //if (!resolutionInited)
            //{
            //    if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            //        resolutionIdx = i;
            //    else if (resolutions[i].width == 1920 && resolutions[i].height == 1080)
            //        resolutionIdx = i;
            //}
        }

        resolutionDropdown.AddOptions(options);
        if (resolutionInited)
        {
            fullScreenToggle.isOn = PlayerPrefs.GetString("FullScreenOn", "true") == "true";
            resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionValue");
        }
        else
        {
            if (resolutionIdx < 0) resolutionIdx = notFoundResolutionException;
            resolutionDropdown.value = resolutionIdx;
        }
        resolutionDropdown.RefreshShownValue();
    }


    public void SetResolution()
    {
        Resolution resolution = resolutions[resolutionDropdown.value];
        Screen.SetResolution(resolution.width, resolution.height, fullScreenToggle.isOn);
        PlayerPrefs.SetInt("ResolutionValue", resolutionDropdown.value);
    }

    public void SetFullScreen()
    {
        Screen.fullScreen = fullScreenToggle.isOn;
        string fullScreenOn = fullScreenToggle.isOn ? "true" : "false";
        PlayerPrefs.SetString("FullScreenOn", fullScreenOn);
    }

}
