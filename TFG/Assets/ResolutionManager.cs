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
    void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int resolutionIdx = 0;
        for(int i = 0; i < resolutions.Length; i++)
        {
            string resolutionOption = resolutions[i].width + "x" + resolutions[i].height + " " + resolutions[i].refreshRate + " Hz";
            options.Add(resolutionOption);
            if(resolutions[i].width == Screen.width && resolutions[i].height == Screen.height 
                && resolutions[i].refreshRate == Screen.currentResolution.refreshRate)
            {
               resolutionIdx = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = resolutionIdx;
        resolutionDropdown.RefreshShownValue();
    }


    public void SetResolution(int _resolutionIdx)
    {
        Resolution resolution = resolutions[_resolutionIdx];
        Screen.SetResolution(resolution.width, resolution.height, fullScreenToggle.isOn);
    }

}
