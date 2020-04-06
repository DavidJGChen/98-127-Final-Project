using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotdogSpawnerController : MonoBehaviour
{
    public GameObject hotdogPrefab;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            Instantiate(hotdogPrefab, transform.position, Quaternion.identity);
        }
    }
}
