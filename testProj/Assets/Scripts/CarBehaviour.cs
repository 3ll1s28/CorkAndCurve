using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class CarBehaviour : MonoBehaviour
{
    //Car Components
    Rigidbody carRb;

    Transform frontLeftWheel, frontRightWheel, rearLeftWheel, rearRightWheel;
    WheelCollider frontLeftCol, frontRightCol, rearLeftCol, rearRightCol;

    Light leftBrake, rightBrake;

    bool engineOn;

    Vector3 colliderPosition;
    Quaternion colliderRotation;

    public float tyreGrip = 10, tyreMass = 20;
    public float maxSteerAngle = 30, steerDampening;

    private float activeSteerDampening, drive, brake;

    public float brakingPower, horsepower;

    //Input axis
    float hAxis, vAxis;

    //Gears and speed
    int currentGear = 0;

    float[] gearRatios;
    float diffRatio = 4, currentTorque, clutch;

    public int topSpeed = 160;
    private float revs, redline = 8000, idle = 1000, wheelRPM;

    private Transform RPMNeedle;
    private float minRotation = 133.89f, maxRotation = -103.17f;
    public AnimationCurve HPtoRPMcurve;

    //Data clusters
    Transform[] wheels;
    WheelCollider[] wheelCols;

    //UI
    private TMP_Text RPMtext, gearText;

    void Start()//Initialisation
    {
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

        //UI Initialisation
        RPMtext = GameObject.Find("rpmReadout").GetComponent<TMP_Text>();
        gearText = GameObject.Find("GearIndicator").GetComponent<TMP_Text>();
        RPMNeedle = GameObject.Find("NeedleObject").GetComponent<Transform>();

        //Configuring the car
        currentGear = 0; //-1: Reverse     0: Neutral      1: 1st etc
        engineOn = false;

        gearRatios = new float[] { 3f, 2.5f, 2f, 1.5f, 1f, 0.8f };
    }

    void Update()
    {
        //Update the RPM UI
        RPMtext.text = "RPM:" + (int)revs;
        gearText.text = (currentGear + 1).ToString();

        RPMNeedle.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(minRotation, maxRotation, revs / redline));

        leftBrake.enabled = false;
        rightBrake.enabled = false;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            engineOn = !engineOn;
        }

        //Match wheels up to suspension forces for aesthetics
        for (int i = 0; i < wheels.Length; i++)
        {
            wheelCols[i].GetWorldPose(out colliderPosition, out colliderRotation);

            wheels[i].position = colliderPosition;
            wheels[i].rotation = colliderRotation;
        }

        clutch = Input.GetKey(KeyCode.LeftShift) ? 0 : Mathf.Lerp(clutch, 1, Time.deltaTime);

        //Input Management
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");

        HorizontalInput();
        VerticalInput();
        ApplyPower();
    }
    private float CalculateTorque()
    {
        float torque = 0;

        if (engineOn)
        {
            if(clutch < 0.1f)
            {
                revs = Mathf.Lerp(revs, Mathf.Max(idle, redline * vAxis) + Random.Range(-50, 50), Time.deltaTime);
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
        for(int i=2; i<4; i++)
        {
            wheelCols[i].motorTorque = currentTorque * vAxis;
        }
    }

    private void HorizontalInput()
    {
        //Steering rotation relative to speed
        float speedOffset = ((carRb.velocity.magnitude * 2.23694f) / 10);

        if (carRb.velocity.magnitude > 5) //AND DAMPENING IS NOT BELOW SPECIFIED NUMBER
        {
            activeSteerDampening = steerDampening / speedOffset;
        }
        else
        {
            activeSteerDampening = steerDampening;
        }

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            if (wheelCols[0].steerAngle != 0)
            {
                wheelCols[0].steerAngle = 0;
                wheelCols[1].steerAngle = 0;
            }
        }

        if (hAxis != 0)
        {
            for (int i = 0; i < 2; i++)
            {
                wheelCols[i].steerAngle = Mathf.Lerp(wheelCols[i].steerAngle, (maxSteerAngle * hAxis), activeSteerDampening);
            }
        }
        else
        {
            for (int i = 0; i < 2; i++)
            {
                wheelCols[i].steerAngle = wheelCols[i].steerAngle = Mathf.Lerp(wheelCols[i].steerAngle, 0, steerDampening);
            }
        }
    }

    private void VerticalInput()
    {
        //Apply torque and brake to rear wheels
       if (vAxis < 0)
        {
            leftBrake.enabled = true;
            rightBrake.enabled = true;

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
    }


}