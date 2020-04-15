using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotdogSpawnerController : MonoBehaviour
{
    public GameObject hotdogPrefab;
    private new ParticleSystem particleSystem;

    private void Start() {
        particleSystem = GetComponentInChildren<ParticleSystem>();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            Instantiate(hotdogPrefab, transform.position, Quaternion.identity);
            particleSystem.Emit(20);
        }
    }
}
