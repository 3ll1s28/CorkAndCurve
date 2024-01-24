using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class CarManager : MonoBehaviour
{
    //Script references
    LapManager lapRef;

    //Car Components
    Rigidbody carRb;

    Transform frontLeftWheel, frontRightWheel, rearLeftWheel, rearRightWheel;
    WheelCollider frontLeftCol, frontRightCol, rearLeftCol, rearRightCol;

    Light leftBrake, rightBrake;

    public bool engineOn;
    public bool canDrive;

    Vector3 colliderPosition;
    Quaternion colliderRotation;

    public float maxSteerAngle = 30, steerDampening;
    public AnimationCurve steeringCurve;

    [NonSerialized]
    public float brake, currentSpeed;

    public float brakingPower, horsepower, horsepowerPerm;

    //Input axis
    float hAxis, vAxis;

    //Gears and speed
    int currentGear = 0;
    bool isReverse = false;

    float[] gearRatios;
    float diffRatio = 4, currentTorque, clutch = 1;

    public int topSpeed = 160;
    private float revs, redline = 8000, idle = 1000, wheelRPM;

    public Quaternion revNeedleRotation;

    private float minRotation = 133.89f, maxRotation = -103.17f;
    public AnimationCurve HPtoRPMcurve;

    public string gearTextPass, RPMtextPass;

    //Data clusters
    Transform[] wheels;
    WheelCollider[] wheelCols;



    void Start()//Initialisation
    {
        lapRef = GameObject.Find("Lap Line").GetComponent<LapManager>();

        //Car
        carRb = this.GetComponent<Rigidbody>();

        #region Define Wheels and Wheel Arrays
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

        #endregion

        leftBrake = GameObject.Find("BrakeLightLeft").GetComponent<Light>();
        rightBrake = GameObject.Find("BrakeLightRight").GetComponent<Light>();

        //Configuring the car
        currentGear = 1; //0: Reverse     1: Neutral      2: 1st etc
        engineOn = false;

        gearRatios = new float[] { 3f, 0f, 3f, 2.5f, 2f, 1.5f, 1f, 0.8f };

        horsepowerPerm = horsepower;
    }

    void Update()
    {
        currentSpeed = carRb.velocity.magnitude;

        //Update the RPM UI and implement reverse gear
        RPMtextPass = "RPM:" + (int)revs;

        if(currentGear > 1)
        {
            isReverse = false;

            horsepower = horsepowerPerm;

            gearTextPass = (currentGear - 1).ToString();
        }
        else if(currentGear == 0)
        {
            gearTextPass = "R";

            if (!isReverse)
            {
                horsepower = -horsepower;
                isReverse = true;
            }           
        }
        else
        {
            gearTextPass = "N";
        }

        revNeedleRotation = Quaternion.Euler(0, 0, Mathf.Lerp(minRotation, maxRotation, revs / (redline * 1.1f)));

        leftBrake.enabled = false;
        rightBrake.enabled = false;


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

        if (Input.GetKeyDown(KeyCode.Alpha1)) //Engine on/ off
        {
            engineOn = !engineOn;
        }

        clutch = Input.GetKey(KeyCode.LeftShift) ? 0 : Mathf.Lerp(clutch, 1, Time.deltaTime); //Clutch implementation

        VerticalInputAndGearing();
        HorizontalInput();
        ApplyPower();
        
    }
    private float CalculateTorque()
    {
        float torque = 0;

        if (engineOn)
        { 
            if(clutch < 0.1f || currentGear == 1)
            {
                revs = Mathf.Lerp(revs, Mathf.Max(idle, redline * vAxis) + UnityEngine.Random.Range(-100, 100), Time.deltaTime);
            }
            else
            {
                wheelRPM = Mathf.Abs((wheelCols[2].rpm + wheelCols[3].rpm) / 2) * gearRatios[currentGear] * diffRatio;
                revs = Mathf.Lerp(revs, Mathf.Max(idle - 100, wheelRPM), Time.deltaTime * 3f);
                torque = (HPtoRPMcurve.Evaluate(revs / redline) * horsepower / revs) * gearRatios[currentGear] * diffRatio * 5252f * clutch;
            }
        }
        return torque;
    }

    private void ApplyPower()
    {
        currentTorque = CalculateTorque();
        if (canDrive)
        {
            for (int i = 2; i < 4; i++)
            {
                wheelCols[i].motorTorque = currentTorque * vAxis;
            }
        }
    }

    private void HorizontalInput()
    {
        //Steering rotation relative to speed
        float steeringAngle = hAxis * steeringCurve.Evaluate(currentSpeed);

        for(int i=0; i<2; i++)
        {
            wheelCols[i].steerAngle = steeringAngle;
        }
    }

    private void VerticalInputAndGearing()
    {
        //Apply torque and brake to rear wheels
       if (vAxis < 0)
        {
            if (engineOn)
            {
                leftBrake.enabled = true;
                rightBrake.enabled = true;
            }
            
            brake = brakingPower;
        }
        else
        {
            brake = 0;
        }

        for (int i = 0; i < wheelCols.Length; i++)
        {
            wheelCols[i].brakeTorque = brake;
        }

        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            if(currentGear < 7)
            {
                currentGear += 1;
            }
        }
        else if (Input.GetKeyDown(KeyCode.PageDown))
        {
            if(currentGear > 0)
            {
                currentGear -= 1;
            }
        }
    }
}