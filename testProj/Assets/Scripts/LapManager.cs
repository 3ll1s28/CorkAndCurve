using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapManager : MonoBehaviour
{
    //Game state
    GameManager managerRef;
    string gameState;

    //Lap Time
    bool lapStarted = false, lastLap = false;
    float startTime, finishedTime, countdownToStart;
    public float elapsedTime;
    public float[] lapTimes; //Stores all RELEVANT lap times

    // Start is called before the first frame update
    void Start()
    {
        managerRef = GameObject.Find("Game Manager").GetComponent<GameManager>();

        gameState = managerRef.gameState;
    }

    // Update is called once per frame
    void Update()
    {
        if(managerRef.gameState == "PLAY" && managerRef.gameFirstStarted)
        {
            managerRef.gameFirstStarted = false; //Ensures single execution


        }

        if (lapStarted)
        {
            elapsedTime = Time.time - startTime;
        }
    }

    private void OnTriggerEnter(Collider col) //LineCrossed
    {
        finishedTime = Time.time - startTime;

        lapStarted = true;
        startTime = Time.time;
    }

    private IEnumerator CountDown(int seconds)
    {
        for(int i=0; i<seconds; i++)
        {
            yield return new WaitForSeconds(1);
        }        
    }
}
