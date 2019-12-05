using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class DxlReadWrite : MonoBehaviour
{
    OSCSender osc;
    private int[] goalPos;
    private int[] servoId;
    public int maxPos;
    public int minPos;
    private string torque = "/torque";
    private string wheelMode = "/wheelMode";
    private string setSpeed = "/writeSpeed";
    private string readPosition = "/readPosition";
    private string writePosition = "/writeGoalPos";
    public const int servoNum = 3;
    public const int servoID_1 = 0;
    public const int servoID_2 = 1;
    public const int servoID_3 = 2;
    private const int torqueEnable = 1;
    private const int torqueDisable = 0;
    private const int wheelModeEnable = 1;
    private const int wheelModeDisable = 0;
    private int goalIndex;
    public int speed;
    public int posFeedback1;
    public int posFeedback2;
    private int sleeptime;
    private int positionThreshold;
    private bool reached = false;
    private bool run;
    int counter = 0;

    Transform servoOne;
    Transform servoTwo;
    Transform servoThree;

    // Start is called before the first frame update
    void Start()
    {
        goalIndex = 0;
        sleeptime = 100;
        positionThreshold = 3;
        osc = transform.GetComponent<OSCSender>();

        servoOne = transform.GetChild(0);
        servoTwo = transform.GetChild(1);
        servoThree = transform.GetChild(2);

        goalPos = new int[2] { maxPos, minPos };
        servoId = new int[servoNum] { servoID_1, servoID_2, servoID_3};
        
        initialSetting();

    }

    // Update is called once per frame
    void Update()
    {

        counter++; // tanya gmn cara bikin counter ke hao
      

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            sendOSC(setSpeed, createMsg(servoId[0], 0));
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            sendOSC(setSpeed, createMsg(servoId[1], 0));
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            initialSetting();
        }


        if (Input.GetKeyDown(KeyCode.A))
        {
            reverseMotion();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            sendOSC(wheelMode, createMsg(servoId[0], wheelModeDisable));
            Thread.Sleep(sleeptime);
            sendOSC(wheelMode, createMsg(servoId[1], wheelModeDisable));
            Thread.Sleep(sleeptime);
            sendOSC(setSpeed, createMsg(servoId[0], 20));
            Thread.Sleep(sleeptime);
            sendOSC(setSpeed, createMsg(servoId[1], 20));
            Thread.Sleep(sleeptime);
            run = true;
        }

        if (run)
        {
            checkifReached(servoID_1, servoID_2);

            if (reached)
            {
                if (goalIndex == 0)
                {
                    goalIndex = 1;
                }
                else
                {
                    goalIndex = 0;
                }


                reached = false;

            }
        }


    }
    public string createMsg(int id, int value)
    {
        string msg = id.ToString() + "#" + value.ToString();
        return msg;
    }

    public void sendOSC(string pattern, string msg)
    {
        osc.SendOSC(pattern, msg);
    }

    public void checkifReached(int id1, int id2)
    {
        // ask for position for all servos
        sendOSC(readPosition, servoId[id1].ToString());
        Thread.Sleep(sleeptime);
        sendOSC(readPosition, servoId[id2].ToString());
        //if(Mathf.Abs(posFeedback1 - goalPos[goalIndex])< positionThreshold)
        //{
        //    reached = true;
        //}
        if (posFeedback1 == goalPos[goalIndex])
        {
            reached = true;

        }

    }

    public void initialSetting()
    {
        for (int i = 0; i <= 3; i++)
        {
            sendOSC(torque, createMsg(servoId[i], torqueEnable));
            sendOSC(wheelMode, createMsg(servoId[i], wheelModeEnable));
            sendOSC(setSpeed, createMsg(servoId[i], speed));
            sendOSC(writePosition, createMsg(servoId[i], goalPos[goalIndex]));
            Thread.Sleep(sleeptime);

            for (float j = 0; j < speed ; j--)
                servoOne.TransformVector(0, j, 0);
        }
    }

    public void reverseMotion()
    {
        for (int i = 0; i < 3; i++)
        {
            sendOSC(torque, createMsg(servoId[i], torqueEnable));
            Thread.Sleep(sleeptime);
            sendOSC(wheelMode, createMsg(servoId[i], wheelModeDisable));
            Thread.Sleep(sleeptime);
            sendOSC(setSpeed, createMsg(servoId[i], 20));
            Thread.Sleep(sleeptime);
            sendOSC(writePosition, createMsg(servoId[i], goalPos[0]));
            Thread.Sleep(sleeptime);

    

        }
    }

}

