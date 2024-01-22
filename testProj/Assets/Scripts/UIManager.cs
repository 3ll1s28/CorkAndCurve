using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //Game state
    GameManager managerRef;
    string gameState;

    LapManager lapManagerRef;
    CarManager carManagerRef;

    //UI ELEMENTS --------------------------------
    //Start Menu and Gameplay General
    private Canvas playCanvas, startMenuCanvas;
    private Button playButton;

    public string UIState = "START_MENU";

    //Car

    public TMP_Text RPMtext, gearText;
    public Transform RPMNeedle;
    

    //Lap times
    private float lapMins, lapSecs, lapMilSecs;
    private TMP_Text lapTime;

    //--------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        lapManagerRef = GameObject.Find("Lap Line").GetComponent<LapManager>();
        carManagerRef = GameObject.Find("PlayerCar").GetComponent<CarManager>();
        managerRef = GameObject.Find("Game Manager").GetComponent<GameManager>();

        //Start menu and General play
        playCanvas = GameObject.Find("PlayCanvas").GetComponent<Canvas>();
        startMenuCanvas = GameObject.Find("StartMenuCanvas").GetComponent<Canvas>();

        playButton = GameObject.Find("PlayButton").GetComponent<Button>();

        lapTime = GameObject.Find("CurrentLapTime").GetComponent<TMP_Text>();

        StartMenu();
    }

    // Update is called once per frame
    void Update()
    {
        if(managerRef.gameState == "PLAY")
        {
            //Updating car UI
            RPMNeedle.rotation = carManagerRef.revNeedleRotation;
            gearText.text = carManagerRef.gearTextPass;
            RPMtext.text = carManagerRef.RPMtextPass;
        }

        //Updating lap UI
        TimeSpan timeSpan = TimeSpan.FromSeconds(lapManagerRef.elapsedTime);
        lapTime.text = timeSpan.ToString(@"mm\:ss\:ff");
    }

    private void StartMenu()
    {
        playCanvas.enabled = false;
        startMenuCanvas.enabled = true;
    }

    #region ClickEvents

    public void OnPlay()
    {
        playCanvas.enabled = true;
        startMenuCanvas.enabled = false;

        UIState = "PLAY";
    }

    #endregion
}
