using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun; 
using Photon.Realtime;

public class PhotonStatus : MonoBehaviour
{
    public string photonStatus;
    public Text textStatus;

    private void Update()
    {
        if (textStatus == null) return;

        this.photonStatus = PhotonNetwork.NetworkClientState.ToString();

        string region = string.IsNullOrEmpty(PhotonNetwork.CloudRegion) ? "N/A" : PhotonNetwork.CloudRegion;

        this.textStatus.text = $"Status: {this.photonStatus}\nRegion: {region}";
    }
}
