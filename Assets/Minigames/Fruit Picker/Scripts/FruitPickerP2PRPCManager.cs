using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.ComponentModel;
using UnityEngine.SocialPlatforms.Impl;

public class FruitPickerP2PRPCManager : MonoBehaviourPun
{
    [SerializeField] private Transform _playerHolder;
    [SerializeField] private Transform _fruitHolder;

    [PunRPC]
    public void RPC_AddScore(int playerViewId, int amount)
    {
        PhotonView pv = PhotonView.Find(playerViewId);
        if (pv != null && pv.TryGetComponent(out FruitPickerP2PPlayerManager player))
        {
            player.AddScore(amount);
        }
    }

    [PunRPC]
    public void RPC_RequestDestroyFruit(int fruitViewId)
    {
        PhotonView targetView = PhotonView.Find(fruitViewId);
        if (targetView != null)
        {
            PhotonNetwork.Destroy(targetView.gameObject);
        }
    }

    [PunRPC]
    public void RPC_AttachPlayerToHolder(int playerViewId)
    {
        PhotonView pv = PhotonView.Find(playerViewId);
        if (pv != null)
        {
            pv.transform.SetParent(_playerHolder, false);
            pv.transform.localPosition = Vector3.zero; // đảm bảo nằm trong holder
            pv.gameObject.SetActive(true);
        }
    }

    [PunRPC]
    public void RPC_AttachFruitToUI(int fruitViewId, Vector3 localPos)
    {
        PhotonView fruitView = PhotonView.Find(fruitViewId);
        if (fruitView != null)
        {
            fruitView.transform.SetParent(_fruitHolder, false);
            fruitView.transform.localPosition = localPos;
            fruitView.gameObject.SetActive(true);
        }
    }
}
