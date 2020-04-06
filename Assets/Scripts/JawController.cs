using System;
using UnityEngine;

public class JawController : MonoBehaviour
{
    public Transform Jaw;
    public Transform JawStartPoint;
    public Transform JawEndPoint;
    public Transform JawHotdogThresholdPoint;
    public Transform JawBitingLine;
    [SerializeField]
    private float jawSpeed = 0;
    [SerializeField]
    private float jawAccelerationTime;
    private float mouseVelocityY;
    private float currentJawVelocityY;
    private float refJawVelocityY;
    private float jawStartY;
    private float jawEndY;
    private float jawHotdogThresholdY;
    private float jawCurrentY;
    private bool jawOpen;

    public event Action ChewEvent;

    private void Start() {
        jawStartY = JawStartPoint.localPosition.y;
        jawEndY = JawEndPoint.localPosition.y;
        jawHotdogThresholdY = JawHotdogThresholdPoint.localPosition.y;

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
        jawCurrentY = newPos.y;
    }

    private void OnJawClose() {
        print("nom");
    }

    public bool CanAcceptFood {
        get {
            return jawCurrentY - jawHotdogThresholdY < 0;
        }
    }

    public float GetBitingLineX() {
        return JawBitingLine.transform.position.x;
    }
}
