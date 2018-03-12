using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NewGame : MonoBehaviour {

    private Button newGame;

    /*
     * CALLED ON UI NEW GAME BUTTON CLICK
     */
    private void Awake()
    {
        newGame = GetComponent<Button>();
        newGame.onClick.AddListener(LoadNewGame);
    }

    public void LoadNewGame()
    {
        if(GameManager.instance != null)
        {
            GameManager.instance.LoadNewGame();
        }
        else
        {
            SceneManager.LoadScene("Tavern");
        }
    }
}
