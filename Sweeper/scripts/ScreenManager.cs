using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    private void Awake()
    {
        if (!PlayerPrefs.HasKey("WindowWidth"))
        {
            PlayerPrefs.SetInt("WindowHeight", Screen.height);
            PlayerPrefs.SetInt("WindowWidth", Screen.width);
        }
    }

    void Update()
    {
        //Fullscreen toggle
        if (Input.GetKeyUp(KeyCode.F11))
        {
            int windowHeight = PlayerPrefs.GetInt("WindowHeight");
            int windowWidth = PlayerPrefs.GetInt("WindowWidth");

            PlayerPrefs.SetInt("WindowHeight", Screen.height);
            PlayerPrefs.SetInt("WindowWidth", Screen.width);

            Screen.fullScreen = !Screen.fullScreen;
            Screen.SetResolution(windowWidth, windowHeight, Screen.fullScreen ? FullScreenMode.Windowed : FullScreenMode.FullScreenWindow);
        }
    }
}
