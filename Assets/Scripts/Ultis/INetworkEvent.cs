using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INetworkEvent
{
    public event System.Action OnConnectedMasterEvent;
    public event System.Action OnDisconnectedEvent;
    public event System.Action OnJoinedRoomEvent;
    public event System.Action OnLeftRoomEvent;
    public event System.Action<Photon.Realtime.Player> OnPlayerJoinedEvent;
    public event System.Action<Photon.Realtime.Player> OnPlayerLeftEvent;
    public event System.Action<string> OnStatusChanged; // cho UI subscribe
    public event System.Action OnGameStartedEvent;
}
