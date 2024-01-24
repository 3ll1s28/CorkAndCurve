using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapManager : MonoBehaviour
{
    //Game state
    GameManager managerRef;
    int gameState;

    //Lap Time
    bool lapStarted = false, lastLap = false;
    public bool hasCounted = false, lapValid;
    float startTime, countdownToStart;
    public float elapsedTime, countdownTimer = 10, finishedTime, bestTime;
    public List<float> lapTimes = new List<float>(); //Stores all RELEVANT lap times

    // Start is called before the first frame update
    void Start()
    {
        managerRef = GameObject.Find("Game Manager").GetComponent<GameManager>();
        lapValid = true;
        gameState = managerRef.gameState;
    }

    // Update is called once per frame
    void Update()
    {
        if(managerRef.gameState == 2)
        {
            if (!hasCounted)
            {
                CountDown(countdownTimer);
            }
        }

        if (lapStarted)
        {
            elapsedTime = Time.time - startTime;
        }
    }

    private void OnTriggerEnter(Collider col) //LineCrossed
    {
        if (elapsedTime > 0)
        {
            if (lapValid)
            {
                finishedTime = Time.time - startTime;
                lapTimes.Add(finishedTime);

                if (bestTime == 0)
                {
                    bestTime = finishedTime;
                }

                if (lapTimes.Count > 1 && finishedTime < bestTime) //If more than one
                {
                    bestTime = finishedTime;
                }
            }
            lapValid = true;
        }
        else
        {
            lapValid = true;
        }

        lapStarted = true;
        startTime = Time.time;
    }

    private void CountDown(float seconds)
    {
        if(seconds > 0)
        {
            seconds -= Time.deltaTime;
            countdownTimer = seconds;
        }
        else
        {
            hasCounted = true;
        }
    }
}
