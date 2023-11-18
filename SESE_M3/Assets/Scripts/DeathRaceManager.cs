using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class DeathRaceManager : MonoBehaviour
{
    public GameObject[] vehiclePrefabs;
    public Transform[] startingPositions;
    public GameObject finisherTextUi;

    public static DeathRaceManager instance = null;

    public Text timeText;

    public List<Health> healthList = new List<Health>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            object playerSelectionNumber;

            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(Constants.PLAYER_SELECTION_NUMBER, out playerSelectionNumber))
            {
                Debug.Log((int)playerSelectionNumber);

                int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                Vector3 instantiatePosition = startingPositions[actorNumber - 1].position;
                PhotonNetwork.Instantiate(vehiclePrefabs[(int)playerSelectionNumber].name, instantiatePosition, Quaternion.Euler(0, 90, 0));
            }
        }
    }
}
