using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public Camera camera;
    private Shooting shootingScript;
    [SerializeField] TextMeshProUGUI playerNameText;
    [SerializeField] GameObject[] deathRaceGameObjects;

    void Start()
    {
        shootingScript = GetComponent<Shooting>();
        this.camera = transform.Find("Camera").GetComponent<Camera>();

        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))
        {
            GetComponent<LapController>().enabled = photonView.IsMine;
            GetComponent<Shooting>().enabled = false;
            GetComponent<Health>().enabled = false;
            camera.enabled = photonView.IsMine;

            foreach (GameObject go in deathRaceGameObjects)
            {
                go.SetActive(false);
            }
        }
        else if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
        {
            GetComponent<LapController>().enabled = false;
            GetComponent<Shooting>().enabled = photonView.IsMine;
            shootingScript.winnerAnnouncement = GameObject.Find("Winner Announcement");
            shootingScript.killAnnouncement = GameObject.Find("Kill Announcement");
            shootingScript.winnerAnnouncement.GetComponent<Text>().text = "";
            shootingScript.killAnnouncement.GetComponent<Text>().text = "";

            DeathRaceManager dM = GameObject.Find("GameManager").GetComponent<DeathRaceManager>();
            dM.healthList.Add(gameObject.GetComponent<Health>());
        }

        GetComponent<VehicleMovement>().enabled = photonView.IsMine;
        camera.enabled = photonView.IsMine;
        playerNameText.text = photonView.Owner.NickName;
        gameObject.name = photonView.Owner.NickName;
    }
}
