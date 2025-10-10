using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FruitPickerP2PPlayerController : Hand
{
    [SerializeField] private FruitPickerP2PPlayerManager _playerManager;
    [SerializeField] private HoldToSelect _holdToSelect;
    [SerializeField] private InputFromJs _inputFromJs;

    private void Start()
    {
        _canvas = FindFirstObjectByType<Canvas>().GetComponent<RectTransform>();

        if (_playerManager.IsMine && _holdToSelect.enabled)
        {
            _holdToSelect.OnStartCount += StartSelectFruit;
            _holdToSelect.OnStopHold += StopSelectFruit;
            _holdToSelect.OnCountEnd += SelectedFruit;
        }

        if (_playerManager.IsMine)
        {
            _inputFromJs = FindFirstObjectByType<InputFromJs>();
            _inputFromJs.SetHand(this);
        }
    }

    private void StartSelectFruit(GameObject hand, GameObject fruit)
    {
        if (fruit == null) return;
        if (!fruit.TryGetComponent(out PhotonView fruitView)) return;

        fruitView.RPC("RPC_MarkAsTaken", RpcTarget.Others);
    }

    private void StopSelectFruit(GameObject hand, GameObject fruit)
    {
        if (fruit == null) return;
        if (!fruit.TryGetComponent(out PhotonView fruitView)) return;

        fruitView.RPC("RPC_ResetFruit", RpcTarget.Others);
    }


    private void SelectedFruit(GameObject hand, GameObject fruit)
    {
        if (!_playerManager.IsMine) return;

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(fruit);
        }
        else
        {
            var fruitController= fruit.GetComponent<FruitPickerP2PFruitController>();

            FruitPickerP2PGameManager.Instance.rpcManager.photonView.RPC(
                nameof(FruitPickerP2PRPCManager.RPC_RequestDestroyFruit), 
                RpcTarget.MasterClient,
                fruitController.ViewID
                );
        }

        FruitPickerP2PGameManager.Instance.rpcManager.photonView.RPC(
            nameof(FruitPickerP2PRPCManager.RPC_AddScore), 
            RpcTarget.All, 
            _playerManager.ViewID, 1
            );
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (!_playerManager.IsMine) return;

        if (FruitPickerP2PGameManager.Instance.isPlaying)
        {
            Vector3 mousePos = Input.mousePosition;

            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            mousePos.z = transform.position.z;
            transform.position = Vector3.SmoothDamp(transform.position, mousePos, ref _velocity, _smoothTime);
        }
#endif
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_playerManager.IsMine) return;
        if (collision.CompareTag("Fruit"))
        {
            _holdToSelect.TryToStartCountToHold(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!_playerManager.IsMine) return;

        if (collision.CompareTag("Fruit"))
        {
            _holdToSelect.TryToStopCountToHold(collision);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!_playerManager.IsMine) return;

        if (collision.CompareTag("Fruit"))
        {
            if (FruitPickerP2PGameManager.Instance.isPlaying == true)
            {
                _holdToSelect.TryToStartCountToHold(collision);
            }
            else
            {
                _holdToSelect.ForceStopHold();
            }
        }
    }

    public override void MoveHand(Landmark v0, Landmark v5, Landmark v17)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (!_playerManager.IsMine) return;
        if (FruitPickerP2PGameManager.Instance.isPlaying)
        {
            base.MoveHand(v0, v5, v17);
        }
#endif
    }
}
