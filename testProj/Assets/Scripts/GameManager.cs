using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int gameState;
    private UIManager UIref;
    private CarManager carRef;
    private LapManager lapRef;

    private GameObject car;

    public Transform carStartPos, carMenuPos;

    bool carMoved;

    // Start is called before the first frame update
    void Start()
    { 

        UIref = GameObject.Find("UI Layer").GetComponent<UIManager>();
        car = GameObject.Find("PlayerCar");
        carRef = car.GetComponent<CarManager>();
        lapRef = GameObject.Find("Lap Line").GetComponent<LapManager>();

        gameState = 0;

        car.transform.position = carMenuPos.position;
        car.transform.rotation = carMenuPos.rotation;

        carMoved = false;
    }

    // Update is called once per frame
    void Update()
    {
        GamestateListener();

        switch (gameState)
        {
            case 0:
                carRef.canDrive = false;
                break;

            case 1: //Tutorial and stuff
                

                break;

            case 2: //Qualifying Phase
                if (!carMoved) //Do once
                {
                    car.transform.position = carStartPos.position;
                    car.transform.rotation = carStartPos.rotation;

                    carMoved = true;
                }

                if (lapRef.hasCounted)
                {
                    carRef.canDrive = true;
                }
                break;

            case 3:
                break;

            case 4:
                break;

            case 5:
                break;
            default:
                carRef.canDrive = false;
                break;
        }
    }

    private void GamestateListener()
    {
        gameState = UIref.UIState;
    }
}
