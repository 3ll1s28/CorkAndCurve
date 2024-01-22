using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public string gameState;
    public bool gameFirstStarted;
    private UIManager UIref;
    // Start is called before the first frame update
    void Start()
    {
        UIref = GameObject.Find("UI Layer").GetComponent<UIManager>();

        gameState = "START_MENU";
    }

    // Update is called once per frame
    void Update()
    {
        GamestateListener();

        if(gameState == "PLAY")
        {
            gameFirstStarted = true;
        }
    }

    private void GamestateListener()
    {
        if(UIref.UIState == "PLAY")
        {
            gameState = UIref.UIState;
        }
    }
}
