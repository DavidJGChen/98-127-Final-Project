using System;
using UnityEngine;

public class JawController : MonoBehaviour
{
    private ThroatController _throatController;
    private ParticleSystem _foodParticleSystem;
    [SerializeField]
    private Transform _jaw;

    [SerializeField]
    private Transform _jawUpperLimit;
    private float _jawUpperLimitY;

    [SerializeField]
    private Transform _jawLowerLimit;
    private float _jawLowerLimitY;

    [SerializeField]
    private Transform _jawOpenPoint;
    private float _jawOpenPointY;

    [SerializeField]
    private Transform _jawBiteLine;
    private float _jawBiteLineX;
    
    // Settings
    [SerializeField]
    private float _jawSpeed = 8f;
    [SerializeField]
    private float _jawBitingThreshold = 0.1f;
    [SerializeField]
    private int _particles = 8;

    // Other stuff
    private float _mouseVelocityY;
    private float _jawCurrentY;
    private bool _jawClosed;
    private bool _canAcceptFood;
    public bool CanAcceptFood {
        get => _canAcceptFood;
    }

    public event Action OnChew;

    private void Start() {
        _throatController = FindObjectOfType<ThroatController>();
        _foodParticleSystem = GetComponentInChildren<ParticleSystem>();

        _jawUpperLimitY = _jawUpperLimit.localPosition.y;
        _jawLowerLimitY = _jawLowerLimit.localPosition.y;
        _jawOpenPointY = _jawOpenPoint.localPosition.y;
        _jawBiteLineX = _jawBiteLine.position.x;

        OnChew += PrintNom;
    }
    private void Update() {
        _mouseVelocityY = Input.GetAxis("Mouse Y");
    }
    private void FixedUpdate() {
        if (_mouseVelocityY == 0f) {
            return;
        }

        Vector2 newPos = _jaw.localPosition;

        newPos.y += _mouseVelocityY * _jawSpeed * Time.deltaTime;
        newPos.y = Mathf.Clamp(newPos.y, _jawLowerLimitY, _jawUpperLimitY);

        if (_jawUpperLimitY - newPos.y < _jawBitingThreshold) {
            if (!_jawClosed) {
                _jawClosed = true;
                OnChew();
            }
        }
        else {
            _jawClosed = false;
        }

        _jaw.localPosition = newPos;
        _jawCurrentY = newPos.y;

        _canAcceptFood = _jawCurrentY - _jawOpenPointY < 0;
    }

    private void PrintNom() {
        print("nom");
    }

    public float GetBiteLineX() {
        return _jawBiteLineX;
    }
    public void InsertFood(float percentage) {
        _throatController.InsertFood(percentage);
    }
    public void EmitParticles() {
        _foodParticleSystem.Emit(_particles);
    }
}
