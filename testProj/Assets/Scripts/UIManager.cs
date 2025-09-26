using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //Game state
    GameManager managerRef;

    LapManager lapManagerRef;
    CarManager carManagerRef;

    //Physical GameObjects
    Renderer lightA, lightB, lightC;

    //UI ELEMENTS --------------------------------
    //Start Menu and Gameplay General
    private CanvasGroup playCanvas, startMenuCanvas, playIntroCanvas, qualPhase;
    private List<CanvasGroup> canvasGroups = new List<CanvasGroup>();

    public int UIState = 0;

    //Car

    public TMP_Text RPMtext, gearText, speedText; 
    GameObject engineWarning;
    public Transform RPMNeedle;
    

    //Lap times
    private TMP_Text lapTime, finishedLapTime, bestLapTime, lapCountDown, qualLapsDone;

    public Material red, yellow, green;

    //--------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        lapManagerRef = GameObject.Find("Lap Line").GetComponent<LapManager>();
        carManagerRef = GameObject.Find("PlayerCar").GetComponent<CarManager>();
        managerRef = GameObject.Find("Game Manager").GetComponent<GameManager>();

        //Start menu and General play
        playCanvas = GameObject.Find("PlayCanvas").GetComponent<CanvasGroup>();
        startMenuCanvas = GameObject.Find("StartMenuCanvas").GetComponent<CanvasGroup>();
        playIntroCanvas = GameObject.Find("PlayIntroCanvas").GetComponent<CanvasGroup>();
        qualPhase = GameObject.Find("QualPhase").GetComponent<CanvasGroup>();

        canvasGroups.Add(playCanvas); canvasGroups.Add(startMenuCanvas); canvasGroups.Add(playIntroCanvas); canvasGroups.Add(qualPhase);

        lapTime = GameObject.Find("CurrentLapTime").GetComponent<TMP_Text>();
        finishedLapTime = GameObject.Find("LastLapTime").GetComponent<TMP_Text>();
        bestLapTime = GameObject.Find("BestLapTime").GetComponent<TMP_Text>();
        lapCountDown = GameObject.Find("Countdown").GetComponent<TMP_Text>();
        qualLapsDone = GameObject.Find("QualLapsDone").GetComponent<TMP_Text>();
        engineWarning = GameObject.Find("EngineWarning");

        lightA = GameObject.Find("startlights_led_a002").GetComponent<Renderer>();
        lightB = GameObject.Find("startlights_led_b002").GetComponent<Renderer>();
        lightC = GameObject.Find("startlights_led_c002").GetComponent<Renderer>();

        lightA.material = red; lightB.material = red; lightC.material = red;

        StartMenu();
    }

    // Update is called once per frame
    void Update()
    {
        if(managerRef.gameState > 1)
        {
            //Updating car UI
            RPMNeedle.rotation = carManagerRef.revNeedleRotation;
            gearText.text = carManagerRef.gearTextPass;
            RPMtext.text = carManagerRef.RPMtextPass;
            speedText.text = Mathf.Round((carManagerRef.currentSpeed * 2.237f)).ToString(); //M/S to MPH
        }
        switch (managerRef.gameState)
        {
            case 2:
                if (!carManagerRef.engineOn)
                {
                    engineWarning.SetActive(true);
                }
                else
                {
                    engineWarning.SetActive(false);
                }

                UICountdown();

                break;
        }

        //Updating lap UI
        TimeSpan timeSpan = TimeSpan.FromSeconds(lapManagerRef.elapsedTime);
        lapTime.text = timeSpan.ToString(@"mm\:ss\:ff");

        if (lapManagerRef.lapValid)
        {
            finishedLapTime.color = Color.white;
            timeSpan = TimeSpan.FromSeconds(lapManagerRef.finishedTime);
            finishedLapTime.text = timeSpan.ToString(@"mm\:ss\:ff");
        }
        else
        {
            finishedLapTime.color = Color.red;
            finishedLapTime.text = "Invalid";
        }
        

        timeSpan = TimeSpan.FromSeconds(lapManagerRef.bestTime);
        bestLapTime.text = timeSpan.ToString(@"mm\:ss\:ff");

        //Number of qual laps done
        int qualLaps;

        if(lapManagerRef.lapTimes.Count > 3)
        {
            qualLaps = 3;
        }
        else
        {
            qualLaps = lapManagerRef.lapTimes.Count;
        }

        qualLapsDone.text = "Qualifying Laps [" + qualLaps + "/3]";
    }

    private void StartMenu()
    {
        FocusCanvas(startMenuCanvas);
    }

    #region ClickEvents

    public void OnPlay()
    {
        FocusCanvas(playIntroCanvas);
        StartCoroutine(FadeCanvas(1f, playIntroCanvas));

        UIState = 1;

    }
    public void OnQualifying()
    {
        StartCoroutine(FadeCanvas(1f, playCanvas));
        StartCoroutine(FadeCanvas(1f, qualPhase));
        FocusCanvas(playCanvas, qualPhase);

        UIState = 2;
    }

    private void UICountdown()
    {
        float interval = 2;

        if (lapManagerRef.countdownTimer <= (interval * 3) && lapManagerRef.countdownTimer > (interval * 2))
        {
            lightA.material = yellow;
        }
        else if (lapManagerRef.countdownTimer <= (interval * 2) && lapManagerRef.countdownTimer > interval)
        {
            lightB.material = yellow;
        }
        else if (lapManagerRef.countdownTimer <= interval)
        {
            lightC.material = yellow;
        }


        if (Mathf.Round(lapManagerRef.countdownTimer) <= 0)
        {
            lapCountDown.enabled = false;
            lightA.material = green;
            lightB.material = green;
            lightC.material = green;
        }
        else
        {
            TimeSpan countDownSpan = TimeSpan.FromSeconds(lapManagerRef.countdownTimer);
            lapCountDown.text = countDownSpan.ToString(@"ss\:ff");
        }
    }

    private void FocusCanvas(CanvasGroup canvas, CanvasGroup canvas2 = null)
    {
        foreach(CanvasGroup canvasGroup in canvasGroups)
        {
            if(canvasGroup != canvas && canvasGroup != canvas2)
            {
                canvasGroup.gameObject.GetComponent<Canvas>().enabled = false;
            }
        }
    }

    public IEnumerator FadeCanvas(float fadeTo, CanvasGroup canvas)
    {
        if (fadeTo == 1)
        {
            canvas.gameObject.GetComponent<Canvas>().enabled = true;
            canvas.alpha = 0;
            canvas.interactable = true;
        }
        else
        {
            canvas.interactable = false;
        }

        while(canvas.alpha != fadeTo)
        {
            canvas.alpha = Mathf.Lerp(canvas.alpha, fadeTo, 0.05f);

            yield return null;
        }
    }

    #endregion
}
