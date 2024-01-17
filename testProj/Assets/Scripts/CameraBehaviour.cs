using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    GameObject car;
    public float followSmooth, rotationSmooth;
    Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        car = GameObject.Find("CameraTarget");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, car.transform.position, ref velocity, followSmooth);
        Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, car.transform.rotation, rotationSmooth);
    }
}
