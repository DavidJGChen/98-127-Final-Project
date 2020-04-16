using UnityEngine;

public class HotdogController : MonoBehaviour
{
    private JawController _jawController;
    private Collider2D _collider2D;

    [SerializeField]
    private Transform _biteMask;
    
    // Hotdog settings
    [SerializeField]
    private float _acceleration = 16f;
    [SerializeField]
    private float _decceleration = 4f;
    private float _moveSpeed = 0f;

    // Flags and stuff
    private bool _accelerate = false;
    private bool _collidingJaw = false;
    private bool _collidingThroat = false;
    private float _percentageEaten = 0f;
    private float _hotdogWidth;
    private float _jawBitingLineX;

    private void Start() {
        _collider2D = GetComponent<Collider2D>();
        _hotdogWidth = _collider2D.bounds.size.x;

        _jawController = FindObjectOfType<JawController>();
        _jawController.OnChew += GetBitten;
        _jawBitingLineX = _jawController.GetBiteLineX();
    }
    private void Update() {
        if (Input.GetKey(KeyCode.Space)) {
            _accelerate = true;
        }
        else {
            _accelerate = false;
        }
    }
    private void FixedUpdate() {
        if (_accelerate) {
            _moveSpeed += _acceleration * Time.deltaTime;
        }
        else {
            if (_moveSpeed != 0f) {
                _moveSpeed -= _decceleration * Time.deltaTime;
            }
        }
        if (_moveSpeed < 0f) {
            _moveSpeed = 0f;
        }


        if (!_collidingThroat && (!_collidingJaw || _jawController.CanAcceptFood)) {
            this.transform.Translate(Vector2.right * _moveSpeed * Time.deltaTime);
        }
    }
    private void OnTriggerEnter2D(Collider2D other) {
        // Remember, there is no way to unset these flags
        if (other.CompareTag("Jaw")) {
            _collidingJaw = true;
        }
        if (other.CompareTag("Throat")) {
            _collidingThroat = true;
        }
    }

    private void GetBitten() {
        if (_collidingJaw) {
            float currentX = _collider2D.bounds.max.x;

            float amountBitten = currentX - _jawBitingLineX;

            if (amountBitten > 0) {
                float oldPercentageEaten = _percentageEaten;

                _percentageEaten = amountBitten / _hotdogWidth;
                
                if (_percentageEaten > 1f) {
                    _percentageEaten = 1f;
                }
                if (_percentageEaten - oldPercentageEaten > 0) {
                    _jawController.InsertFood(_percentageEaten - oldPercentageEaten);
                    _jawController.EmitParticles();
                    UpdateSpriteMask();
                }
                if (_percentageEaten == 1f) {
                    DestroyHotdog();
                }
            }
        }
    }
    private void UpdateSpriteMask() {
        Vector2 newPos = Vector2.zero;

        newPos.x = _hotdogWidth / 2 - _percentageEaten * _hotdogWidth;

        _biteMask.localPosition = newPos;
    }
    private void DestroyHotdog() {
        _jawController.OnChew -= GetBitten;
        Destroy(this.gameObject);
        Destroy(this); // redundant?
    }
}
