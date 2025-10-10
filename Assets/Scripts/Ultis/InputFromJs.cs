using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class InputFromJs : MonoBehaviour
{
    [SerializeField] PhotonRoomManager mgr;
    [SerializeField] Hand rightHand;
    [SerializeField] Hand leftHand;

    private bool isRecivedCamera = false;

    private Landmark[] rightHandLandMark = new Landmark[21];
    private Landmark[] leftHandLandMark = new Landmark[21];

    public void SetHand(Hand player)
    {
        rightHand = player;
    }

    public void StartGame(string json)
    {
        StartCoroutine(WaitForCameraAndStartGame(json));
    }

    private IEnumerator WaitForCameraAndStartGame(string json)
    {
        yield return new WaitUntil(() => isRecivedCamera == true);

        PhotonRoomInfo info = JsonUtility.FromJson<PhotonRoomInfo>(json);
        if (mgr != null && info != null)
        {
            mgr.InitAndConnect(info.playerName, info.roomId, info.region, info.isMasterClient);
        }
    }

    public void GetRightHandPos(string message)
    {
        if (rightHand != null)
        {
            rightHandLandMark = JsonConvert.DeserializeObject<Landmark[]>(message);
            rightHand.MoveHand(rightHandLandMark[0], rightHandLandMark[5], rightHandLandMark[17]);
        }
    }

    public void GetLeftHandPos(string message)
    {
        if (rightHand != null)
        {
            leftHandLandMark = JsonConvert.DeserializeObject<Landmark[]>(message);
            leftHand.MoveHand(rightHandLandMark[0], rightHandLandMark[5], rightHandLandMark[17]);
        }
    }

    public void ConnectedCameraStatus(string message)
    {
        isRecivedCamera = true;
    }


}

public class PhotonRoomInfo
{
    public string roomId;
    public string playerName;
    public string region;
    public bool isMasterClient;
}

public class Landmark
{
    public float x;
    public float y;
    public float z;
}
