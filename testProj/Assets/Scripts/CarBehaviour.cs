using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CarBehaviour : MonoBehaviour
{
    //Vars

    //Car Components
    Rigidbody carRb;
    Transform frontLeftWheel, frontRightWheel, rearLeftWheel, rearRightWheel;

    WheelCollider frontLeftCol, frontRightCol, rearLeftCol, rearRightCol;

    Vector3 colliderPosition;
    Quaternion colliderRotation;

    //Data clusters
    Transform[] wheels;
    WheelCollider[] wheelCols;

    void Start()
    {
        //Initialisation

        //Car
        carRb = this.GetComponent<Rigidbody>();

        //Wheels
        frontLeftWheel = GameObject.Find("FrontLeftWheelMesh").transform;
        frontRightWheel = GameObject.Find("FrontRightWheelMesh").transform;
        rearLeftWheel = GameObject.Find("RearLeftWheelMesh").transform;
        rearRightWheel = GameObject.Find("RearRightWheelMesh").transform;

        frontLeftCol = GameObject.Find("FrontLeftWheel").GetComponent<WheelCollider>();
        frontRightCol = GameObject.Find("FrontRightWheel").GetComponent<WheelCollider>();
        rearLeftCol = GameObject.Find("RearLeftWheel").GetComponent<WheelCollider>();
        rearRightCol = GameObject.Find("RearRightWheel").GetComponent<WheelCollider>();

        wheels = new Transform[] { frontLeftWheel, frontRightWheel, rearLeftWheel, rearRightWheel };
        wheelCols = new WheelCollider[] { frontLeftCol, frontRightCol, rearLeftCol, rearRightCol };
    }

    void FixedUpdate()
    {
        for (int i = 0; i < wheels.Length; i++)
        {
            wheelCols[i].GetWorldPose(out colliderPosition, out colliderRotation);

            wheels[i].position = colliderPosition;
            wheels[i].rotation = colliderRotation;
        }
    }
}