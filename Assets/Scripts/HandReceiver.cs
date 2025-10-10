using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

[Serializable]
public class HandData
{
    public string hand;
    public List<Landmark> landmarks;
}

public class HandReceiver : MonoBehaviour
{
    [Header("UDP Settings")]
    public int listenPort = 5005;
    public bool showDebugLog = false;

    public Hand leftHandController;
    public Hand rightHandController;

    private UdpClient udpClient;
    private Thread receiveThread;

    private Vector3[] leftHand = new Vector3[21];
    private Vector3[] rightHand = new Vector3[21];

    private readonly object lockObj = new object();

    void Start()
    {
        Application.runInBackground = true;
        StartUDP();
        //CreateHandObjects();
    }

    void OnDestroy()
    {
        StopUDP();
    }

    // --------------------------
    void StartUDP()
    {
        running = true;
        udpClient = new UdpClient(listenPort);
        receiveThread = new Thread(ReceiveData) { IsBackground = true };
        receiveThread.Start();
        Debug.Log($"[HandReceiver] Listening on UDP port {listenPort}...");
    }

    private bool running = true;

    void StopUDP()
    {
        running = false;
        udpClient?.Close();
        receiveThread?.Join();
    }

    private HandData latestLeft;
    private HandData latestRight;

    void ReceiveData()
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, listenPort);

        while (running)
        {
            try
            {
                byte[] data = udpClient.Receive(ref remoteEndPoint);
                string json = Encoding.UTF8.GetString(data);

                if (showDebugLog) Debug.Log($"Received: {json}");
                HandData hand = JsonConvert.DeserializeObject<HandData>(json);

                if (hand?.landmarks != null && hand.landmarks.Count == 21)
                {
                    lock (lockObj)
                    {
                        if (hand.hand == "left")
                        {
                            for (int i = 0; i < 21; i++)
                                leftHand[i] = new Vector3(hand.landmarks[i].x, hand.landmarks[i].y, -hand.landmarks[i].z);
                            latestLeft = hand;
                        }
                        else if (hand.hand == "right")
                        {
                            for (int i = 0; i < 21; i++)
                                rightHand[i] = new Vector3(hand.landmarks[i].x, hand.landmarks[i].y, -hand.landmarks[i].z);
                            latestRight = hand;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[HandReceiver] Receive error: {ex.Message}");
            }
        }
    }

    private void LateUpdate()
    {
        if (latestLeft == null || latestRight == null) return;
        leftHandController.MoveHand(latestLeft.landmarks[0], latestLeft.landmarks[5], latestLeft.landmarks[17]);
        rightHandController.MoveHand(latestRight.landmarks[0], latestRight.landmarks[5], latestRight.landmarks[17]);
    }
}
