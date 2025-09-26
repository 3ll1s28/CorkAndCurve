using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    private GameManager managerRef;

    GameObject car;
    public float followSmooth, rotationSmooth, transitionDamping;
    Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        managerRef = GameObject.Find("Game Manager").GetComponent<GameManager>();
        car = GameObject.Find("CameraTarget");
    }

    // Update is called once per frame
    void Update()
    {
        switch (managerRef.gameState)
        {
            case 0:
                Camera.main.transform.position = new Vector3(77.53999f, 6.694772f, 50.71f);
                Camera.main.transform.rotation = Quaternion.Euler(4.713f, 260.301f, 0.452f);
                Camera.main.fieldOfView = 20;
                break;
            case 1: //Tutorial sorta thing
                Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(24.52f, 0.723f, 44.619f), transitionDamping);
                Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, Quaternion.Euler(-4.476f, 107.398f, 0.051f), transitionDamping);
                Camera.main.fieldOfView = 34;
                break;
            case 2:
                Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, car.transform.position, ref velocity, followSmooth);
                Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, car.transform.rotation, rotationSmooth);
                Camera.main.fieldOfView = 60;
                break;
            default:
                break;
        }
    }
}
