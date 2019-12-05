using System.Net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityOSC;

public class OSCReceiver : MonoBehaviour, OSCListener
{
    OSCServer myServer;
    public int inPort = 9998;
    //private int buffersize = 100;
    DxlReadWrite dxl;
    void Start()
    {
        OSCHandler.Instance.Init();
        myServer = OSCHandler.Instance.CreateServer("myServer", inPort);
        OSCHandler.Instance.AddCallback(this);

        myServer.ReceiveBufferSize = 1024;
        myServer.SleepMilliseconds = 10;
        dxl = GetComponent<DxlReadWrite>();

    }

    public void OnOSC(OSCPacket pckt)  //接收資料
    {

        if (pckt.Address.Equals("/position_feedback"))
        {
            string receivedData = pckt.Data[0].ToString();
            string[] idWithValue = receivedData.Split(new string[] { "#" }, StringSplitOptions.None);


            if (int.Parse(idWithValue[0]) == 0)
            {
                dxl.posFeedback1 = int.Parse(idWithValue[1]);
                Debug.Log("servo1 position feedback: " + dxl.posFeedback1);
            }
            else if (int.Parse(idWithValue[0]) == 1)
            {
                dxl.posFeedback2 = int.Parse(idWithValue[1]);
                Debug.Log("servo2 position feedback: " + dxl.posFeedback2);

            }

        }
         


        else if(pckt.Address.Equals("/load_feedback"))
        {
            string receivedData = pckt.Data[0].ToString();
            //dxlValue.loadFeedback = int.Parse(receivedData);
        }

    }
}
