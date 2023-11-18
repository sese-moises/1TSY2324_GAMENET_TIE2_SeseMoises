using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using ExitGames.Client.Photon;
using System.Reflection;

public class Shooting : MonoBehaviourPunCallbacks
{
    public Camera cam;
    public GameObject _hitEffectPrefab;
    public GameObject _projectilePrefab;
    public GameObject _projectileSpawnLocation;

    private bool canShoot = true;

    public GameObject killAnnouncement;
    public GameObject winnerAnnouncement;
    public int remaining;

    public enum RaiseEventsCode
    {
        WhoLivedEventCode = 1
    }

    private int finishOrder = 0;

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (byte)RaiseEventsCode.WhoLivedEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;

            string nickNameofFinishedPlayer = (string)data[0];
            int ranking = (int)data[1];
            int viewId = (int)data[2];

            GameObject eliminatedText = winnerAnnouncement;

            if (viewId == photonView.ViewID)
            {
                eliminatedText.GetComponent<Text>().text = "ELIMINATED";
                eliminatedText.GetComponent<Text>().color = Color.red;
            }
        }
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && canShoot)
        {
            canShoot = false;
            Laser();
        }
        if (Input.GetKeyDown(KeyCode.Mouse1) && canShoot)
        {
            canShoot = false;
            Projectile();
        }
    }

    public void Laser()
    {
        RaycastHit hit;
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if (Physics.Raycast(ray, out hit, 500))
        {
            Debug.Log(hit.collider.gameObject.name);
            photonView.RPC("CreateHitEffects", RpcTarget.All, hit.point);

            if (hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine
                && hit.collider.gameObject.GetComponent<Health>().GetHealth > 0)
            {
                int id = this.photonView.ViewID;
                hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 10, id, null);
            }
        }
        Invoke("Shoot", 0.5f);
    }

    public void Projectile()
    {
        GameObject projectileGameObject = PhotonNetwork.Instantiate("Projectile", _projectileSpawnLocation.transform.position, _projectileSpawnLocation.transform.rotation);
        projectileGameObject.GetComponent<Projectile>().owner = PhotonNetwork.NickName;
        projectileGameObject.GetComponent<Projectile>().idOwner = this.photonView.ViewID;
        Invoke("Shoot", 2f);
    }

    public void Shoot()
    {
        canShoot = true;
    }

    [PunRPC]
    public void TakeDamage(int damage, int viewID, string owner, PhotonMessageInfo info)
    {
        Health targetHealth = this.GetComponent<Health>();
        targetHealth.health -= damage;
        targetHealth.healthBar.fillAmount = targetHealth.health / targetHealth.startHealth;

        if (targetHealth.health <= 0)
        {
            DeathRaceManager dM = GameObject.Find("GameManager").GetComponent<DeathRaceManager>();
            dM.healthList.Remove(gameObject.GetComponent<Health>());
            killAnnouncement.GetComponent<Text>().text = info.Sender.NickName + " killed " + info.photonView.Owner.NickName;
            photonView.RPC("CO_ClearAnnouncement", RpcTarget.AllBuffered);
            if (owner != null)
            {
                killAnnouncement.GetComponent<Text>().text = owner + " killed " + info.photonView.Owner.NickName;
            }

            Die();
            if (dM.healthList.Count < 2)
            {
                photonView.RPC("EndGame", RpcTarget.AllBuffered, info.Sender.NickName);
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
            GetComponent<PlayerSetup>().camera.transform.parent = null;
            GetComponent<VehicleMovement>().enabled = false;
        }

        string nickName = photonView.Owner.NickName;
        int viewId = photonView.ViewID;

        object[] data = new object[] { nickName, finishOrder, viewId };
        finishOrder--;

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOption = new SendOptions
        {
            Reliability = false
        };

        PhotonNetwork.RaiseEvent((byte)RaiseEventsCode.WhoLivedEventCode, data, raiseEventOptions, sendOption);
    }

    [PunRPC]
    IEnumerator CO_ClearAnnouncement()
    {
        yield return new WaitForSeconds(3f);
        killAnnouncement.GetComponent<Text>().text = "";
    }

    [PunRPC]
    public void EndGame(string winner)
    {
        winnerAnnouncement.GetComponent<Text>().text = winner + " WINS!";
        winnerAnnouncement.GetComponent<Text>().color = Color.black;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject p in players)
        {
            p.transform.GetComponent<VehicleMovement>().enabled = false;
        }
    }
}
