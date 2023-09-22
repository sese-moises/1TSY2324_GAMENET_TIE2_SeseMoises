using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LaunchManager : MonoBehaviourPunCallbacks
{
    public GameObject _enterGamePanel;
    public GameObject _connectionStatusPanel;
    public GameObject _lobbyPanel;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        _enterGamePanel.SetActive(true);
        _connectionStatusPanel.SetActive(false);
        _lobbyPanel.SetActive(false);
    }

    void Update()
    {
        
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.NickName + " connected to photon servers");
        _connectionStatusPanel.SetActive(false);
        _lobbyPanel.SetActive(true);
    }

    public override void OnConnected()
    {
        Debug.Log("Connected to internet");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarning(message);
        CreateAndJoinRoom();
    }

    public void ConnectToPhotonServer()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            _connectionStatusPanel.SetActive(true);
            _enterGamePanel.SetActive(false);
        }
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    private void CreateAndJoinRoom()
    {
        string randomRoomName = "Room " + Random.Range(0, 10000);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 20;

        PhotonNetwork.CreateRoom(randomRoomName, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.NickName + " has entered " + PhotonNetwork.CurrentRoom.Name);
        PhotonNetwork.LoadLevel("GameScene");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " has entered room " + PhotonNetwork.CurrentRoom.Name + ". (" + PhotonNetwork.CurrentRoom.PlayerCount + "/20)");
    }
}
