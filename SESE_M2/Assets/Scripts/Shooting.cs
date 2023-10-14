using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class Shooting : MonoBehaviourPunCallbacks
{
    public float GetHealth
    {
        get => _health;
    }

    public Camera _cam;
    public GameObject _hitEffectPrefab;

    [Header("HP Related Stuff")]
    public float _startHealth = 100;
    private float _health;
    public Image _healthBar;

    private Animator _animator;
    private GameObject _killAnnouncement;
    private GameObject _winnerAnnouncement;

    public int _killCount;
    private SpawnPoints _spawnManager;

    void Start()
    {
        _killCount = 0;
        _health = _startHealth;
        _healthBar.fillAmount = _health / _startHealth;
        _animator = this.GetComponent<Animator>();
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnPoints>();
        _winnerAnnouncement = GameObject.Find("WinnerAnnouncement");
        _killAnnouncement = GameObject.Find("KillAnnouncement");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Fire()
    {
        RaycastHit hit;
        Ray ray = _cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if (Physics.Raycast(ray, out hit, 200))
        {
            Debug.Log(hit.collider.gameObject.name);

            photonView.RPC("CreateHitEffects", RpcTarget.All, hit.point);

            if (hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine 
                && hit.collider.gameObject.GetComponent<Shooting>().GetHealth > 0)
            {
                int id = this.photonView.ViewID;
                hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 25, id);
            }
        }
    }

    [PunRPC]
    public void TakeDamage(int damage, int viewID, PhotonMessageInfo info)
    {
        this._health -= damage;
        this._healthBar.fillAmount = _health / _startHealth;

        if (_health <= 0)
        {
            Die();
            _killAnnouncement.GetComponent<TextMeshProUGUI>().text = info.Sender.NickName + " killed " + info.photonView.Owner.NickName;

            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            foreach(GameObject p in players)
            {
                if (p.GetComponent<PhotonView>().ViewID == viewID)
                {
                    p.GetComponent<Shooting>()._killCount++;
                }

                if (p.GetComponent<Shooting>()._killCount >= 10)
                {
                    photonView.RPC("EndGame", RpcTarget.AllBuffered, info.Sender.NickName);
                }
            }
        }
    }

    [PunRPC]
    public void CreateHitEffects(Vector3 position)
    {
        GameObject hitEffectGameObject = Instantiate(_hitEffectPrefab, position, Quaternion.identity);
        Destroy(hitEffectGameObject, 0.2f);
    }

    public void Die()
    {
        if (photonView.IsMine)
        {
            _animator.SetBool("isDead", true);
            StartCoroutine(CO_RespawnCountdown());
        }
    }

    IEnumerator CO_RespawnCountdown()
    {
        GameObject respawnText = GameObject.Find("RespawnText");
        float respawnTime = 5.0f;

        while (respawnTime > 0)
        {
            yield return new WaitForSeconds(1.0f);
            respawnTime--;

            transform.GetComponent<PlayerMovementController>().enabled = false;
            respawnText.GetComponent<Text>().text = "You are dead. Respawning in " + respawnTime.ToString(".00");
        }

        _animator.SetBool("isDead", false);
        respawnText.GetComponent<Text>().text = "";

        int random = Random.Range(0, 3);

        this.transform.position = _spawnManager._spawnPoints[random];
        transform.GetComponent<PlayerMovementController>().enabled = true;

        photonView.RPC("RegainHealth", RpcTarget.AllBuffered);
        photonView.RPC("ClearAnnouncement", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void ClearAnnouncement()
    {
        _killAnnouncement.GetComponent<TextMeshProUGUI>().text = "";
    }

    [PunRPC]
    public void RegainHealth()
    {
        _health = 100;
        _healthBar.fillAmount = _health / _startHealth;
    }

    [PunRPC]
    public void EndGame(string winner)
    {
        _winnerAnnouncement.GetComponent<TextMeshProUGUI>().text = winner + " WINS!";

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject p in players)
        {
            p.transform.GetComponent<PlayerMovementController>().enabled = false;
        }
    }
}
