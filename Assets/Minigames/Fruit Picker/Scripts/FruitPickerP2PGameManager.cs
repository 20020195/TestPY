using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitPickerP2PGameManager : MonoBehaviourSingleton<FruitPickerP2PGameManager>
{
    [SerializeField] private FruitPickerP2PSpawner _spawner;
    [SerializeField] private FruitPickerP2PUIManager uiManager;
    public FruitPickerP2PRPCManager rpcManager;
    INetworkEvent photonRoomManager;

    public bool isPlaying = false;
    public bool IsMasterClient => PhotonNetwork.IsMasterClient;

    private void OnEnable()
    {
        photonRoomManager = FindFirstObjectByType<PhotonRoomManager>().GetComponent<INetworkEvent>();
        photonRoomManager.OnGameStartedEvent += HandleGameStarted;
        photonRoomManager.OnPlayerLeftEvent += HandlePlayerLeft;
    }


    private void OnDisable()
    {
        photonRoomManager.OnGameStartedEvent -= HandleGameStarted;
        photonRoomManager.OnPlayerLeftEvent -= HandlePlayerLeft;
    }

    private void HandleGameStarted()
    {
        Debug.Log("🚀 Game started → Spawning player...");
        _spawner.SpawnPlayer();
        _spawner.SpawnFruit();

        isPlaying = true;
    }

    private void Replay()
    {
        uiManager.ResetUI();

        Debug.Log("🚀 Game started → Spawning player...");
        FindFirstObjectByType<FruitPickerP2PPlayerManager>().ResetPlayer();
        _spawner.SpawnFruit();

        isPlaying = true;
    }

    public void PauseGame()
    {
        isPlaying = false;
    }

    public void ResumGame()
    {
        isPlaying = true;
    }

    private void HandlePlayerLeft(Player player)
    {
        //pause game and show popup
        PauseGame();
        uiManager.ShowPlayerLeftPopup(player.NickName);
    }

    public void Rematch()
    {
        if (!IsMasterClient)
        {
            Debug.LogWarning("⚠️ Only MasterClient can trigger Rematch!");
            return;
        }

        Debug.Log("🔄 Master pressed Rematch → restarting for all players!");
        rpcManager.photonView.RPC(nameof(StartRematch), RpcTarget.All);
    }

    [PunRPC]
    private void StartRematch()
    {
        Debug.Log("🔄 Rematch started by Master → All players restart!");
        Replay();
    }

    private void OnGUI()
    {
        //Hiển thị ping
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 30; // chỉnh cỡ chữ 30
        style.normal.textColor = Color.white; // màu chữ (có thể đổi sang đỏ, xanh...)

        if (Photon.Pun.PhotonNetwork.IsConnected)
        {
            int ping = Photon.Pun.PhotonNetwork.GetPing();
            GUI.Label(new Rect(10, 10, 400, 50), $"Ping: {ping} ms", style);
        }

        //Hiển thị FPS
        GUI.Label(new Rect(10, 40, 400, 50), $"FPS: {(int)(1.0f / Time.deltaTime)}", style);
    }

    public void EndGame(FruitPickerP2PPlayerManager mine, FruitPickerP2PPlayerManager op)
    {
        isPlaying = false;

        uiManager.ShowResult(mine, op);
    }
}
