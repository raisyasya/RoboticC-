using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;


public class OSCSender : MonoBehaviour
{

    public string outIP;
    public int outPort;

    void Awake()
    {
        // init OSC
        OSCHandler.Instance.Init();
        Debug.Log("connecting ip: " + outIP + " port: " + outPort);
        // client
        OSCHandler.Instance.CreateClient("myClient", IPAddress.Parse(outIP), outPort);
    }

    public void SendOSC(string pattern, string message)
    {
        OSCHandler.Instance.SendMessageToClient("myClient", pattern, message);
    }

    void Update()
    {
        //if (Input.GetKeyDown (KeyCode.S)) {
        //  OSCHandler.Instance.SendMessageToClient("myClient", "/position", "hhh");
        //} 
    }
}
