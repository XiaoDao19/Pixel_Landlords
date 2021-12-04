using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class testScreen : MonoBehaviour
{
    public Dropdown dropdown;
    public Button rightButton;

    Resolution[] resolutions;
    void Start()
    {
        rightButton.onClick.AddListener(() => { SetScrren(); });

        resolutions = Screen.resolutions;

        dropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;

            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        dropdown.AddOptions(options);
        dropdown.value = currentResolutionIndex;
        dropdown.RefreshShownValue();

        //dropdown.onValueChanged.AddListener

        Screen.SetResolution(1920, 1080, Screen.fullScreen);
    }

    void Update()
    {
        
    }

    private void SetScrren() 
    {
        dropdown.value = 0;
    }

    public void SetResolution(int resolutionIndex) 
    {
        Resolution resolution = resolutions[resolutionIndex];

        if (resolution.width == 1920 && resolution.height == 1080)
        {
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

            return;
        }

        Screen.SetResolution(resolution.width, resolution.height, FullScreenMode.Windowed);
    }

    public void SetQuality(int qualityIndex) 
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
}
