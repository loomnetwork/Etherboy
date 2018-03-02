using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{

    public Button exit;

    private void Awake()
    {
        exit = GetComponent<Button>();

        exit.onClick.AddListener(LoadSplashScreen);
    }
    
    private void LoadSplashScreen()
    {
        SceneManager.LoadScene("SplashScreen");
    }
}
