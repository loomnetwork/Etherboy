using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{    
    /*
     * ON EXIT BUTTON CLICK
     */
    public void LoadSplashScreen()
    {
        GameManager.instance.eventsManager.events = new List<string>();
        SceneManager.LoadScene("SplashScreen");
    }
}
