using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject _playerPrefab;
    private SpawnPoints _spawnManager;

    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (_playerPrefab != null)
            {
                StartCoroutine(CO_DelayedPlayerSpawn());
            }
        }
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnPoints>();
    }

    private IEnumerator CO_DelayedPlayerSpawn()
    {
        yield return new WaitForSeconds(3);
        int random = Random.Range(0, 3);
        PhotonNetwork.Instantiate(_playerPrefab.name, _spawnManager._spawnPoints[random], Quaternion.identity);
    }
}
