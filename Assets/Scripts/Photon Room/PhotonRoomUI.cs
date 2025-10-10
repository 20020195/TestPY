using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PhotonRoomUI : MonoBehaviour
{
    [Header("UI References")]
    public Text statusText;
    public InputField roomIdInput;
    public Text playerNameText;

    public Button loginOrJoinButton;
    public Button logoutButton;
    public Button leaveRoomButton;

    public string region = "hk";
    public bool autoStartInEditor = false;
    public bool isMasterClient = true;

    PhotonRoomManager mgr;

    private void OnEnable()
    {
        if (mgr == null) mgr = FindFirstObjectByType<PhotonRoomManager>();

        mgr.OnStatusChanged += UpdateStatus;
        mgr.OnJoinedRoomEvent += OnJoinedRoom;
    }

    private void OnDisable()
    {
        mgr.OnStatusChanged -= UpdateStatus;
        mgr.OnJoinedRoomEvent -= OnJoinedRoom;
    }

    private void Start()
    {
#if UNITY_EDITOR
        if (loginOrJoinButton != null)
        {
            loginOrJoinButton.onClick.AddListener(OnClickLoginOrJoin);
        }

        if (logoutButton != null)
        {
            logoutButton.onClick.AddListener(OnClickLogoutButtton);
        }

        if (leaveRoomButton != null)
        {
            leaveRoomButton.onClick.AddListener(OnClickLeaveRoom);
        }

        if (autoStartInEditor)
        {
            OnClickLoginOrJoin();
        }
#elif UNITY_WEBGL && !UNITY_EDITOR
        roomIdInput.gameObject.SetActive(false);
        loginOrJoinButton.gameObject.SetActive(false);
        logoutButton.gameObject.SetActive(false);
        leaveRoomButton.gameObject.SetActive(false);
#endif
    }

    public void OnClickLoginOrJoin()
    {
        string playerName = $"Player {Random.Range(1000, 10000)}";
        string roomId = string.IsNullOrEmpty(roomIdInput.text) ? "Room1" : roomIdInput.text.Trim();
        mgr.InitAndConnect(playerName, roomId, region, isMasterClient);

        if (playerNameText != null) playerNameText.text = playerName;
    }

    public void OnClickLogoutButtton()
    {
        mgr.DiconnectToServer();
    }

    public void OnClickLeaveRoom()
    {
        mgr.LeaveRoom();
    }

    private void UpdateStatus(string msg)
    {
        if (statusText != null) statusText.text = msg;
    }

    private void OnJoinedRoom()
    {
        Debug.Log("UI nhận event: đã join room!");
    }
}
