﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotdogController : MonoBehaviour
{
    public JawController jawController;
    public Transform biteMask;
    private new Collider2D collider2D;
    [SerializeField]
    private float moveSpeed = 8;
    private bool move = false;
    private bool collidingJaw = false;
    private float percentageEaten = .0f;
    private float hotdogWidth;
    private float jawBitingLine;

    private void Start() {
        collider2D = GetComponent<Collider2D>();

        hotdogWidth = collider2D.bounds.size.x;

        jawController = FindObjectOfType<JawController>();

        jawController.ChewEvent += GetBitten;

        jawBitingLine = jawController.GetBitingLineX();
    }
    private void Update() {
        if (Input.GetKey(KeyCode.Space)) {
            move = true;
        }
        else {
            move = false;
        }
    }
    private void FixedUpdate() {
        if (move) {
            if (!collidingJaw || jawController.CanAcceptFood) {
                this.transform.Translate(Vector2.left * Time.deltaTime * moveSpeed);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Jaw")) {
            collidingJaw = true;
        }
    }

    private void GetBitten() {
        if (collidingJaw) {
            float currentX = collider2D.bounds.min.x;

            float amountBitten = jawBitingLine - currentX;

            if (amountBitten > 0) {
                percentageEaten = amountBitten / hotdogWidth;
                if (percentageEaten > 1f) {
                    DestroyHotdog();
                }
                print(percentageEaten);
                UpdateSpriteMask();
            }
        }
    }

    private void UpdateSpriteMask() {
        Vector2 newPos = Vector2.zero;

        newPos.x = percentageEaten * hotdogWidth;

        biteMask.localPosition = newPos;
    }

    private void DestroyHotdog() {
        jawController.ChewEvent -= GetBitten;
        Destroy(this.gameObject);
        Destroy(this);
    }
}