using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    public static SpawnPoints _instance;
    public Vector3[] _spawnPoints;

    private void Awake()
    {
        _instance = this;
    }
}
