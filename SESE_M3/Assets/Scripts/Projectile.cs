using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;

public class Projectile : MonoBehaviourPunCallbacks
{
    private float moveSpeed;
    public string owner;
    public int idOwner;

    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = 50f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(moveSpeed * Time.deltaTime * Vector3.forward);
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !other.gameObject.GetComponent<PhotonView>().IsMine
                && other.gameObject.GetComponent<Health>().GetHealth > 0)
        {
            other.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 25, idOwner, null);
            gameObject.SetActive(false);
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
