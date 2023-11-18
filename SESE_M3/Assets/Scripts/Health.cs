using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;


public class Health : MonoBehaviour
{

    public float GetHealth
    {
        get => health;
    }

    [Header("HP Related Stuff")]
    public float startHealth = 100;
    public float health;
    public Image healthBar;

    void Start()
    {
        health = startHealth;
        healthBar.fillAmount = health / startHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
