using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class CarBehaviour : MonoBehaviour
{
    //Vars

    //Car Components
    Rigidbody carRb;
    Transform frontLeftWheel, frontRightWheel, rearLeftWheel, rearRightWheel;

    WheelCollider frontLeftCol, frontRightCol, rearLeftCol, rearRightCol;

    bool rearWheelFocused;

    Vector3 colliderPosition;
    Quaternion colliderRotation;

    public float tyreGrip = 10, tyreMass = 20;
    public float maxSteerAngle = 30;
    public float horsepower, brakingPower;

    //Axis
    float hAxis, vAxis;

    //Gears
    int currentGear;

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

        //Configuring the car
        currentGear = -1;
        horsepower *= 10;
        brakingPower = -brakingPower;
    }

    void FixedUpdate()
    {
        //Match wheels up to suspension forces for aesthetics
        for (int i = 0; i < wheels.Length; i++)
        {
            wheelCols[i].GetWorldPose(out colliderPosition, out colliderRotation);

            wheels[i].position = colliderPosition;
            wheels[i].rotation = colliderRotation;
        }

        //Input Management
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            for(int i=0; i<2; i++)
            {
                wheelCols[i].steerAngle = Mathf.Lerp(wheelCols[i].steerAngle, -maxSteerAngle, 0.2f);
            }
        }
        else if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            for (int i = 0; i < 2; i++)
            {
                wheelCols[i].steerAngle = wheelCols[i].steerAngle = Mathf.Lerp(wheelCols[i].steerAngle, maxSteerAngle, 0.2f);
            }
        }
        else
        {
            for (int i = 0; i < 2; i++)
            {
                wheelCols[i].steerAngle = wheelCols[i].steerAngle = Mathf.Lerp(wheelCols[i].steerAngle, 0, 0.2f);
            }
        }

        //Apply torque to rear wheels
        for(int i=0; i<4; i++)
        {
            if(i >= 2)
            {
                rearWheelFocused = true;
            }
            else
            {
                rearWheelFocused = false;
            }

            if(vAxis < 0) //Brake on all wheels
            {
                wheelCols[i].brakeTorque = vAxis * brakingPower;

                if (rearWheelFocused && carRb.velocity.magnitude > 10) //10 to not stall
                {
                    wheelCols[i].motorTorque = -(horsepower / 2); //REPLACE HP WITH ENGINE BRAKING NUMBER CALCULATED BY REVS
                }
            }
            else if(rearWheelFocused && vAxis > 0) //Rear wheel drive
            {
                wheelCols[i].motorTorque = vAxis * horsepower;
                wheelCols[i].brakeTorque = 0;
            }
            else if(rearWheelFocused && vAxis == 0 && carRb.velocity.magnitude > 10) //If car still moving with no vertical input, engine brake on rear wheels
            {
                wheelCols[i].motorTorque = -(horsepower / 2); //REPLACE HP WITH ENGINE BRAKING NUMBER CALCULATED BY REVS
                wheelCols[i].brakeTorque = 0;
            }
            else
            {
                wheelCols[i].motorTorque = 0;
                wheelCols[i].brakeTorque = 0;
            }
        }
        
        //Applying brake to front wheels

    }
}