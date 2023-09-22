using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class TakingDamage : MonoBehaviourPunCallbacks
{
    [SerializeField] private Image _healthBar;

    public float _health;

    private float _startHealth = 100;

    void Start()
    {
        _health = _startHealth;
        _healthBar.fillAmount = _health / _startHealth;
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        _health -= damage;
        Debug.Log(_health);

        _healthBar.fillAmount = _health / _startHealth;

        if (_health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (photonView.IsMine)
        {
            GameManager._instance.LeaveRoom();
        }
    }
}
