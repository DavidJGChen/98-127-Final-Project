using System;
using UnityEngine;

public class JawController : MonoBehaviour
{
    public Transform Jaw;
    public Transform JawStartPoint;
    public Transform JawEndPoint;
    [SerializeField]
    private float jawSpeed;
    [SerializeField]
    private float jawAccelerationTime;
    private float mouseVelocityY;
    private float currentJawVelocityY;
    private float refJawVelocityY;
    private float jawStartY;
    private float jawEndY;
    private float jawCurrentY;
    private bool jawOpen;

    public event Action ChewEvent;

    private void Start() {
        jawStartY = JawStartPoint.localPosition.y;
        jawEndY = JawEndPoint.localPosition.y;

        ChewEvent += OnJawClose;
    }

    private void Update() {
        mouseVelocityY = Input.GetAxis("Mouse Y");
    }

    private void FixedUpdate() {
        Vector2 newPos = Jaw.localPosition;

        // currentJawVelocityY = 
        //     Mathf.SmoothDamp(currentJawVelocityY,
        //         mouseVelocityY * jawSpeed, 
        //         ref refJawVelocityY, 
        //         jawAccelerationTime);

        // newPos.y += currentJawVelocityY * Time.deltaTime;
        newPos.y += mouseVelocityY * jawSpeed * Time.deltaTime;
        newPos.y = Mathf.Clamp(newPos.y, jawEndY, jawStartY);

        if (jawStartY - newPos.y < 0.05f) {
            if (jawOpen) {
                ChewEvent();
                jawOpen = false;
            }
        }
        else {
            jawOpen = true;
        }

        Jaw.localPosition = newPos;
    }

    private void OnJawClose() {
        print("nom");
    }
}
