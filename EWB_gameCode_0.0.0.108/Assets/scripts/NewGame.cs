using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NewGame : MonoBehaviour {

    private Button newGame;

    private void Awake()
    {
        newGame = GetComponent<Button>();

        newGame.onClick.AddListener(LoadNewGame);
    }

    private void LoadNewGame()
    {
        SceneManager.LoadScene("Tavern");

        if(GameManager.instance != null)
        {
            GameManager.instance.eventsManager.events = new List<string>();
            GameManager.instance.health = 100;
            GameManager.instance.coins = 0;
        }
    }
}
