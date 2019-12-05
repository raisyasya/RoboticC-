using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class DxlController : MonoBehaviour
{
    public Button sendOSC;
    public InputField idValue;
    public InputField patternKywrd;
    private OSCSender oscSend;


    // internal paramters/ sent parameters
    public int wheelMode_1;
    public int wheelMode_2;

    public float goalPosition_1;
    public float goalPosition_2;

    public float dxl_speed_1;
    public float dxl_speed_2;


    // message and address pattern 
    string msg;
    string patternOsc;

    //parameters recieved from pi
    public int positionFeedback;
    public int loadFeedback;
    public int tempFeedback;

    //
    private float deg_rotPos_1; //現在的位置
    private float deg_rotPos_2;
    private bool servoOneStart;
    private bool servoTwoStart;
    public bool servoOnewhMode;
    public bool servoTwowhMode;
    private int counter;

    //
    Transform servoOne;
    Transform servoTwo;

    string[] command = {"/writeSpeed", "/writeGoalPos", "/wheelMode", "readPosition"};

    void Start()
    {
        oscSend = GetComponent<OSCSender>();
        servoOne = transform.GetChild(0);
        servoTwo = transform.GetChild(1);
        //resetServo();
        counter = 0;
        //BothRotateMax();

    }

    void Update()
    {


        //if (servoOneStart) ServoOneRotate(goalPosition_1, dxl_speed_1); //看要動哪個
        //if (servoTwoStart) ServoTwoRotate(goalPosition_2, dxl_speed_2);
        //if (servoOnewhMode) ServoOneContinuous(dxl_speed_1); //看要哪種模式
        //if (servoTwowhMode) ServoTwoContinuous(dxl_speed_2); //看要哪種模式
        resetServo();

        if (counter == 80)
        {
            BothRotateMax();
        }
        else if (counter == 160)
        {
            resetServo();
        }
        else if (counter == 240)
        {
            Stretch();

        }
        else if (counter > 320)
        {
            resetServo();

        }

        Debug.Log(counter);
        counter+=2;


    }
    public void ServoOneRotate(float goalStep, float speedinSimulation)
    {
        float goalDegree = goalStep;
        float speed = speedinSimulation;
        if (deg_rotPos_1 > goalDegree)
        {
            speed = speed * -1;
        }
        else if (deg_rotPos_1 < goalDegree)
        {
            speed = speed;
        }
        else if (deg_rotPos_1 == goalDegree)
        {
            dxl_speed_1 = 0;
        }
        servoOne.RotateAround(servoOne.position, servoOne.forward, speed);
        deg_rotPos_1 += speed;
        if (deg_rotPos_1 >= goalDegree && speed > 0)
        {
            servoOneStart = false;
        }
        else if (deg_rotPos_1 <= goalDegree && speed < 0)
        {
            servoOneStart = false;
        }
    }

    public void ServoTwoRotate(float goalStep, float speedinSimulation)
    {
        float goalDegree = goalStep;
        float speed = speedinSimulation;
        if (deg_rotPos_2 > goalDegree)
        {
            speed *= -1;
        }
        else if (deg_rotPos_2 < goalDegree)
        {
            speed = speed;
        }
        else if (deg_rotPos_2 == goalDegree)
        {
            deg_rotPos_2 = 0;
        }
        servoTwo.RotateAround(servoTwo.position, servoTwo.forward, speed);
        deg_rotPos_2 += speed;
        if (deg_rotPos_2 >= goalDegree && speed > 0)
        {
            servoTwoStart = false;
        }
        else if (deg_rotPos_2 <= goalDegree && speed < 0)
        {
            servoTwoStart = false;
        }
    }

    public void ServoOneContinuous(float speedinSimulation)
    {
        float speed = speedinSimulation;
        float speedinSteps = speedinSimulation / 0.293f;
        if (speedinSteps > 1024 && speedinSteps < 2047)
        {
            speed *= -1;
        }
        else if (speedinSteps > 0 && speedinSteps < 1024)
        {
            speed = speed;
        }

        servoOne.RotateAround(servoOne.position, servoOne.forward, speed);
        deg_rotPos_1 += speed;
    }
    public void ServoTwoContinuous(float speedinSimulation)
    {
        float speed = speedinSimulation;
        float speedinSteps = speedinSimulation / 0.293f;
        if (speedinSteps > 1024 && speedinSteps < 2047)
        {
            speed *= -1;
        }
        else if (speedinSteps > 0 && speedinSteps < 1024)
        {
            speed = speed;
        }

        servoTwo.RotateAround(servoTwo.position, servoTwo.forward, speed);
        deg_rotPos_2 += speed;
    }


    public void setMsg()
    {
        msg = idValue.text.ToString();
    }
    public void setPattern()
    {
        patternOsc = "/" + patternKywrd.text.ToString();
    }
    public void printPatternKwrd()
    {
        setPattern();
        Debug.Log(patternOsc);
    }
    public void printMsg()
    {
        setMsg();
        Debug.Log(msg);
    }

    public void setInternalParameter(string addressPattern, string inputMessage)    //addressPattern 是輸入的看你要改哪個東西 inputMessage某種字串 好像用來判斷哪個SERVO
    {
        string[] idAndValue;
        idAndValue = inputMessage.Split('#'); //把#當成分隔?
        switch (addressPattern)
        {
            case "/writeSpeed":
                if(idAndValue[0] == "0")
                {
                    dxl_speed_1= int.Parse(idAndValue[1]) * 0.2f;
                }
                else if(idAndValue[0] == "1")
                {
                    dxl_speed_2 = int.Parse(idAndValue[1]) * 0.2f;

                }
                break;

            case "/writeGoalPos":
                if (idAndValue[0] == "0")
                {
                    goalPosition_1 = int.Parse(idAndValue[1]) * 0.293f;
                    servoOneStart = true;
                }
                else if (idAndValue[0] == "1")
                {
                    goalPosition_2 = int.Parse(idAndValue[1]) * 0.293f;
                    servoTwoStart = true;

                }
                break;

            case "/wheelMode":
                if (idAndValue[0] == "0")
                {
                    wheelMode_1 = int.Parse(idAndValue[1]);
                    if(wheelMode_1==1) servoOnewhMode = true; 


                }
                else if (idAndValue[0] == "1")
                {
                    wheelMode_2 = int.Parse(idAndValue[1]);
                    if (wheelMode_2 == 1) servoTwowhMode = true;

                }
                break;
        }
    }

    public void sendMsgOSC() //把資料傳給servo (輸入完後)
    {
        oscSend.SendOSC(patternOsc, msg);
        Debug.Log("sending " + patternOsc + ", " + msg + " to " + oscSend.outIP + " port: " + oscSend.outPort);
        //
        setInternalParameter(patternOsc, msg);
    }

    public void readPos(string number)
    {
        oscSend.SendOSC(command[3], "number");
    }


    public void resetServo()
    {
        oscSend.SendOSC(command[0], "0#100");
        oscSend.SendOSC(command[1], "0#0");
        oscSend.SendOSC(command[0], "1#100");
        oscSend.SendOSC(command[1], "1#0");
        Thread.Sleep(250);
    }

    public void BothRotateMax()
    {
        oscSend.SendOSC(command[0], "0#100");
        oscSend.SendOSC(command[1], "0#1023");
        oscSend.SendOSC(command[0], "1#100");
        oscSend.SendOSC(command[1], "1#1023");
        Thread.Sleep(250);
    }

    public void Stretch()
    {
        oscSend.SendOSC(command[0], "0#100");
        oscSend.SendOSC(command[1], "0#1023");
        oscSend.SendOSC(command[0], "1#100");
        oscSend.SendOSC(command[1], "1#0");
        Thread.Sleep(250);
    }

    /*
    public void KeepRotate(float goalStep, float speedinSimulation)
    {
        float goalDegree01 = goalStep;
        float goalDegree02 = 1023f - goalStep;

        float speed01 = speedinSimulation;
        float speed02 = speedinSimulation;

        if (deg_rotPos_1 > goalDegree01)
        {
            speed01 = speed01 * -1;
        }
        else if (deg_rotPos_1 < goalDegree01)
        {
            speed01 = speed01;
        }
        else if (deg_rotPos_1 == goalDegree01)
        {
            dxl_speed_1 = 0;
        }

        if (deg_rotPos_2 > goalDegree02)
        {
            speed02 = speed02 * -1;
        }
        else if (deg_rotPos_2 < goalDegree02)
        {
            speed02 = speed02;
        }
        else if (deg_rotPos_2 == goalDegree02)
        {
            dxl_speed_2 = 0;
        }
        
        servoOne.RotateAround(servoOne.position, servoOne.forward, speed01);
        deg_rotPos_1 += speed01;

        servoTwo.RotateAround(servoTwo.position, servoTwo.forward, speed02);
        deg_rotPos_2 += speed02;

        if (deg_rotPos_1 >= goalDegree01 && speed01 > 0)
        {
            servoOneStart = false;
        }
        else if (deg_rotPos_1 <= goalDegree01 && speed01 < 0)
        {
            servoOneStart = false;
        }

        if (deg_rotPos_2 >= goalDegree02 && speed02 > 0)
        {
            servoTwoStart = false;
        }
        else if (deg_rotPos_2 <= goalDegree02 && speed02 < 0)
        {
            servoTwoStart = false;
        }

    }*/
}
