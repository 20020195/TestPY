using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;

public class FruitPickerP2PSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _playerPrefab;

    [SerializeField] private List<GameObject> fruits = new List<GameObject>();
    [SerializeField] private List<Transform> _spawnPositions = new List<Transform>();
    public int NumberOfFruits { get { return _spawnPositions.Count; } }

    public void SpawnPlayer()
    {
        // Spawn ngoài màn hình
        GameObject player = PhotonNetwork.Instantiate(_playerPrefab.name, new Vector3(1500,0,0), Quaternion.identity);

        // Attach player cho tất cả client
        FruitPickerP2PGameManager.Instance.rpcManager.photonView.RPC(
            nameof(FruitPickerP2PRPCManager.RPC_AttachPlayerToHolder), 
            RpcTarget.AllBuffered, 
            player.GetComponent<PhotonView>().ViewID
            );
    }

    


    public void SpawnFruit()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log("❌ Chỉ MasterClient mới được spawn fruit!");
            return;
        }

        foreach (Transform spawnPoint in _spawnPositions)
        {
            int randomIndex = Random.Range(0, fruits.Count);
            string prefabName = fruits[randomIndex].name;

            GameObject fruit = PhotonNetwork.Instantiate(prefabName, spawnPoint.position, Quaternion.identity);

            FruitPickerP2PGameManager.Instance.rpcManager.photonView.RPC(
                nameof(FruitPickerP2PRPCManager.RPC_AttachFruitToUI), 
                RpcTarget.AllBuffered, 
                fruit.GetComponent<PhotonView>().ViewID, 
                spawnPoint.localPosition
                );
        }
    }
}
