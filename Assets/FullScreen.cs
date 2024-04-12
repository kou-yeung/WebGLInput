using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullScreen : MonoBehaviour
{
    public bool isFullScreen;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void OnEventTriggerClick()
    {
        if (!isFullScreen)
        {
            // for unity2019
            WebGLSupport.WebGLWindow.MakeFullscreen("unityContainer");

            isFullScreen = true;
        }
        else
        {
            WebGLSupport.WebGLWindow.ExitFullscreen();

            isFullScreen = false;
        }
    }
}
