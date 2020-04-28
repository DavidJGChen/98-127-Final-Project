using UnityEngine;

public class HotdogController : MonoBehaviour
{
    private JawController _jawController;
    private ChokingController _chokingController;
    private HotdogSpawnerController _hotdogSpawnerController;
    private GameController _gameController;
    private Collider2D _collider2D;
    private AudioSource _biteSound;

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
    private bool _isChoking = false;
    private float _percentageEaten = 0f;
    private float _hotdogWidth;
    private float _jawBitingLineX;

    private void Start() {
        _collider2D = GetComponent<Collider2D>();
        _hotdogWidth = _collider2D.bounds.size.x;

        _biteSound = GetComponent<AudioSource>();

        _gameController = FindObjectOfType<GameController>();

        _jawController = FindObjectOfType<JawController>();
        _jawController.OnChew += GetBitten;
        _jawBitingLineX = _jawController.GetBiteLineX();

        _hotdogSpawnerController = FindObjectOfType<HotdogSpawnerController>();

        _chokingController = FindObjectOfType<ChokingController>();
        _chokingController.OnChoke += PushBack;
        _chokingController.OnChoke += () => _isChoking = true;
        _chokingController.OnUnchoke += () => _isChoking = false;
    }
    private void Update() {
        if (!_gameController.Started) {
            return;
        }
        if (Input.GetKey(KeyCode.Space)) {
            _accelerate = true;
        }
        else {
            _accelerate = false;
        }

        if (_collidingThroat && _percentageEaten < 0.2f) {
            _chokingController.Choke();
        }
    }
    private void FixedUpdate() {
        if (!_gameController.Started) {
            return;
        }

        if (_accelerate) {
            _moveSpeed += _acceleration * Time.deltaTime;
        }
        else {
            if (_moveSpeed != 0f) {
                _moveSpeed -= _decceleration * Time.deltaTime;
            }
        }
        if (_moveSpeed < 0f) {
            _moveSpeed += 2f * _decceleration * Time.deltaTime;
            if (_moveSpeed > 0f) {
                _moveSpeed = 0;
            }
        }

        if (_isChoking) {
            _moveSpeed = -1f;
        }


        if (!_collidingThroat && (!_collidingJaw || _jawController.CanAcceptFood)) {
            this.transform.Translate(Vector2.right * _moveSpeed * Time.deltaTime);
        }
        else if (_isChoking && _moveSpeed < 0f) {
            this.transform.Translate(Vector2.right * _moveSpeed * Time.deltaTime);
        }
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Jaw")) {
            _collidingJaw = true;
        }
        if (other.CompareTag("Throat")) {
            _collidingThroat = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Jaw")) {
            _collidingJaw = false;
        }
        if (other.CompareTag("Throat")) {
            _collidingThroat = false;
        }
    }

    private void PushBack() {
        _moveSpeed = -4f;
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
                    BiteSound();
                }
                else {
                    _percentageEaten = oldPercentageEaten;
                }
                if (_percentageEaten == 1f) {
                    _hotdogSpawnerController.SpawnHotdog();
                    DestroyHotdog();
                }
            }
        }
    }
    private void BiteSound() {
        _biteSound.Play();
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
