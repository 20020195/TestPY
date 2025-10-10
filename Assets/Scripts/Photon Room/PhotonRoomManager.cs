using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;

public class PhotonRoomManager : MonoBehaviourPunCallbacks, INetworkEvent
{
    [Header("Room Settings")]
    public byte maxPlayers = 4;
    public byte minPlayersToStart = 2;

    [Header("Canvas References")]
    public GameObject loadingCanvas;
    public GameObject gameScenePrefab;

    private bool isConnectedToMaster = false;
    private bool isJoinedRoom = false;

    private string roomId = string.Empty;


    // ===================== EVENTS ======================
    public event System.Action OnConnectedMasterEvent;
    public event System.Action OnDisconnectedEvent;
    public event System.Action OnJoinedRoomEvent;
    public event System.Action OnLeftRoomEvent;
    public event System.Action<Player> OnPlayerJoinedEvent;
    public event System.Action<Player> OnPlayerLeftEvent;
    public event System.Action<string> OnStatusChanged; // cho UI subscribe
    public event System.Action OnGameStartedEvent;

    // ===================== PUBLIC API ======================
    public void InitAndConnect(string playerName, string roomId = null, string region = "hk", bool isMasterClient = false)
    {
        PhotonNetwork.LocalPlayer.NickName = playerName;
        PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout = 15000;

        var settings = PhotonNetwork.PhotonServerSettings.AppSettings;
        settings.FixedRegion = region; 
        PhotonNetwork.ConnectUsingSettings(settings);
        SetStatus($"⏳ Kết nối tới region {region} với tên: {playerName}");
        this.roomId = roomId;

        StartCoroutine(Co_JoinOrCreateRoom(roomId, isMasterClient));
    }


    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            SetStatus("🚪 Đang rời room...");
        }
    }

    public void DiconnectToServer()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
            SetStatus("🚪 Đang ngắt kết nối...");
        }
    }

    // ===================== COROUTINE ======================
    private IEnumerator Co_JoinOrCreateRoom(string roomID, bool isMasterClient)
    {
        yield return new WaitUntil(() => isConnectedToMaster);

        RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers };

        if (isMasterClient) 
        { 
            PhotonNetwork.CreateRoom(roomID, options, TypedLobby.Default); 
            SetStatus($"🏠 Đang tạo room: {roomID}"); 
        } else 
        { 
            SetStatus($"🔍 Đang cố gắng join vào room: {roomID}");

            PhotonNetwork.JoinRoom(roomID);
        }

        yield return new WaitUntil(() => PhotonNetwork.InRoom);
    }


    // ===================== CALLBACKS ======================
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        SetStatus("✅ Kết nối Master Server thành công!");
        isConnectedToMaster = true;
        OnConnectedMasterEvent?.Invoke();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        isConnectedToMaster = false;
        isJoinedRoom = false;
        SetStatus("❌ Mất kết nối: " + cause);
        OnDisconnectedEvent?.Invoke();
    }

    public override void OnJoinedRoom()
    {
        isJoinedRoom = true;
        SetStatus("🎉 Đã join room: " + PhotonNetwork.CurrentRoom.Name);
        OnJoinedRoomEvent?.Invoke();
        CheckAutoStartGame();
    }

    public override void OnLeftRoom()
    {
        isJoinedRoom = false;
        SetStatus("🚪 Đã rời room.");
        OnLeftRoomEvent?.Invoke();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        SetStatus($"👤 {newPlayer.NickName} đã vào room.");
        OnPlayerJoinedEvent?.Invoke(newPlayer);
        CheckAutoStartGame();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        SetStatus($"👤 {otherPlayer.NickName} đã rời room.");
        OnPlayerLeftEvent?.Invoke(otherPlayer);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        SetStatus("❌ Tạo room thất bại: " + message);
        isJoinedRoom = false;
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"❌ Join room failed: {message}. Thử lại sau 1s...");
        StartCoroutine(TryJoinRoomAgain());
    }

    private IEnumerator TryJoinRoomAgain()
    {
        yield return new WaitForSeconds(1f);
        PhotonNetwork.JoinRoom(roomId);
    }


    // ===================== GAME START ======================
    private void CheckAutoStartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
            if (playerCount >= minPlayersToStart)
            {
                SetStatus($"🚀 Đủ {playerCount} người, bắt đầu game!");
                StartGame();
            }
            else
            {
                SetStatus($"👥 {playerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers} người. Đợi thêm...");
            }
        }
    }

    public void StartGame()
    {
        photonView.RPC(nameof(RPC_StartGame), RpcTarget.All);

    }

    [PunRPC]
    private void RPC_StartGame()
    {
        if (loadingCanvas != null) loadingCanvas.SetActive(false);
        if (gameScenePrefab != null)
        {
            GameObject gameSceneInstance = Instantiate(gameScenePrefab, Vector3.zero, Quaternion.identity);
            Debug.Log("🚀 Game scene spawned: " + gameSceneInstance.name);
            gameSceneInstance.SetActive(true);
        }
        else
        {
            Debug.LogError("❌ Chưa gán prefab GameScene vào PhotonRoomManager!");
        }

        OnGameStartedEvent?.Invoke();
    }

    // ===================== HELPER ======================
    private void SetStatus(string msg)
    {
        Debug.Log(msg);
        OnStatusChanged?.Invoke(msg); // gửi cho UI
    }
}
