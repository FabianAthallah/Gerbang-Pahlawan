using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullScript : MonoBehaviour
{
    // Start is called before the first frame update


    public void fullscreenOnOffoff()
    {
        if(Screen.fullScreen == true)
        {
            Screen.fullScreen = false;
        }
        else
        {
            Screen.fullScreen = true;
        }
    }
}
