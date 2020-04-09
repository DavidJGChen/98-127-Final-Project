using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ThroatController : MonoBehaviour
{
    private JawController jawController;
    public event Action SwallowEvent;
    // Start is called before the first frame update
    private void Start()
    {
        jawController = FindObjectOfType<JawController>();

        SwallowEvent += PrintSwallow;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) {
            TrySwallow();
        }
    }

    private void TrySwallow() {
        if (jawController.CurrentHotDogs > 0) {
            SwallowEvent();
        }
    }

    private void PrintSwallow() {
        print("swallow");
    }
}
