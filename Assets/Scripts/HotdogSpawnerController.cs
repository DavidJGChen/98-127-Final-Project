using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotdogSpawnerController : MonoBehaviour
{
    [SerializeField]
    private GameObject _hotdogPrefab;
    private ParticleSystem _particleSystem;

    [SerializeField]
    private int _particles = 20;

    private void Start() {
        _particleSystem = GetComponentInChildren<ParticleSystem>();
    }

    public void SpawnHotdog() {
        Instantiate(_hotdogPrefab, transform.position, Quaternion.identity);
        _particleSystem.Emit(_particles);
    }
}
