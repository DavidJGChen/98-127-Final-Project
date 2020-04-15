using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotdogController : MonoBehaviour
{
    public JawController jawController;
    public Transform biteMask;
    private new Collider2D collider2D;
    [SerializeField]
    private float acceleration = 16f;
    private float decceleration = 4f;
    private float moveSpeed;
    private bool accelerate = false;
    private bool collidingJaw = false;
    private bool collidingThroat = false;
    private float percentageEaten = 0f;
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
            accelerate = true;
        }
        else {
            accelerate = false;
        }
    }
    private void FixedUpdate() {
        if (accelerate) {
            moveSpeed += acceleration * Time.deltaTime;
        }
        else {
            if (moveSpeed != 0) {
                moveSpeed -= decceleration * Time.deltaTime;
            }
        }
        if (moveSpeed < 0) {
            moveSpeed = 0;
        }


        if (!collidingThroat && (!collidingJaw || jawController.CanAcceptFood)) {
            this.transform.Translate(Vector2.right * Time.deltaTime * moveSpeed);
        }
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Jaw")) {
            collidingJaw = true;
        }
        if (other.CompareTag("Throat")) {
            collidingThroat = true;
        }
    }

    private void GetBitten() {
        if (collidingJaw) {
            float currentX = collider2D.bounds.max.x;

            float amountBitten = currentX - jawBitingLine;

            if (amountBitten > 0) {
                float oldPercentageEaten = percentageEaten;
                percentageEaten = amountBitten / hotdogWidth;
                if (percentageEaten > 1f) {
                    percentageEaten = 1f;
                    DestroyHotdog();
                }
                print(percentageEaten);
                if (percentageEaten - oldPercentageEaten > 0) {
                    jawController.InsertFood(percentageEaten - oldPercentageEaten);
                    jawController.EmitParticles();
                    UpdateSpriteMask();
                }
            }
        }
    }

    private void UpdateSpriteMask() {
        Vector2 newPos = Vector2.zero;

        newPos.x = hotdogWidth / 2 - percentageEaten * hotdogWidth;

        biteMask.localPosition = newPos;
    }

    private void DestroyHotdog() {
        jawController.ChewEvent -= GetBitten;
        Destroy(this.gameObject);
        Destroy(this);
    }
}
