using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;
using TMPro;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public GameObject _fpsModel;
    public GameObject _nonFpsModel;

    public GameObject _playerUiPrefab;

    public PlayerMovementController _playerMovementController;
    public Camera _fpsCamera;

    private Animator _animator;
    public Avatar _fpsAvatar, _nonFpsAvatar;

    private Shooting _shooting;
    [SerializeField] private TextMeshProUGUI _playerNameText;

    void Start()
    {
        _playerMovementController = this.GetComponent<PlayerMovementController>();
        _animator = this.GetComponent<Animator>();

        _fpsModel.SetActive(photonView.IsMine);
        _nonFpsModel.SetActive(!photonView.IsMine);

        _animator.avatar = photonView.IsMine ? _fpsAvatar : _nonFpsAvatar;

        _shooting = this.GetComponent<Shooting>();
        
        if (photonView.IsMine)
        {
            GameObject playerUi = Instantiate(_playerUiPrefab);
            _playerMovementController._fixedTouchField = playerUi.transform.Find("RotationTouchField").GetComponent<FixedTouchField>();
            _playerMovementController._joystick = playerUi.transform.Find("Fixed Joystick").GetComponent<Joystick>();
            _fpsCamera.enabled = true;

            playerUi.transform.Find("FireButton").GetComponent<Button>().onClick.AddListener(() => _shooting.Fire());
        }
        else
        {
            _playerMovementController.enabled = false;
            GetComponent<RigidbodyFirstPersonController>().enabled = false;
            _fpsCamera.enabled = false;
        }

        _playerNameText.text = photonView.Owner.NickName;
    }

    void Update()
    {
        _animator.SetBool("isLocalPlayer", photonView.IsMine);
    }
}
