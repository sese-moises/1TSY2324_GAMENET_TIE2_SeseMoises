using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _cam;
    [SerializeField] private TextMeshProUGUI _playerNameText;

    void Start()
    {
        if (photonView.IsMine)
        {
            transform.GetComponent<MovementController>().enabled = true;
            _cam.GetComponent<Camera>().enabled = true;
        }
        else
        {
            transform.GetComponent<MovementController>().enabled = false;
            _cam.GetComponent<Camera>().enabled = false;
        }

        _playerNameText.text = photonView.Owner.NickName;
    }
}
