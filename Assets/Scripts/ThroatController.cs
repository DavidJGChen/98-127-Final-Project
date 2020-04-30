using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ThroatController : MonoBehaviour
{
    private JawController _jawController;
    private ChokingController _chokingController;
    private GameController _gameController;

    [SerializeField]
    private GameObject[] _foodStages;
    private int _numStages;
    private GameObject _firstFoodStage;
    private GameObject _lastFoodStage;

    [SerializeField]
    private Transform[] _throatWaypoints;
    private int _numWaypoints;
    private GameObject[] _throatStages;

    // Settings
    [SerializeField]
    private float _foodPerChew = 0.2f;
    [SerializeField]
    private float _chokingThreshold = 0.5f;
    [SerializeField]
    private static readonly KeyCode[] _keySequence = {KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F};

    // Other stuff
    private float[] _currChewedFood;
    private float _totalChewedFood;
    private float[] _deltaChewedFood;
    private float[] _currThroatFood;
    private bool _isChoking;
    private float _totalSwallowed = 0;
    public float TotalSwallowed {
        get => _totalSwallowed;
    }

    public event Action OnSwallow;

    private void Start()
    {
        _gameController = FindObjectOfType<GameController>();

        _jawController = GetComponent<JawController>();
        _jawController.OnChew += ChewFood;

        _chokingController = GetComponent<ChokingController>();
        _chokingController.OnChoke += () => _isChoking = true;
        _chokingController.OnUnchoke += () => _isChoking = false;

        _numStages = _foodStages.Length;
        _currChewedFood = new float[_numStages];
        _deltaChewedFood = new float[_numStages - 1];

        _firstFoodStage = _foodStages[0];
        
        _lastFoodStage = _foodStages[_numStages - 1];
        
        for(int i = 0; i < _numStages; i++) {
            _currChewedFood[i] = 0f;
        }

        _numWaypoints = _throatWaypoints.Length;
        _currThroatFood = new float[_numWaypoints - 2];
        _throatStages = new GameObject[_numWaypoints - 2];

        if (_keySequence.Length != _numWaypoints - 1) {
            print("SOMTING WONG");
        }

        OnSwallow += PrintSwallow;
    }
    private void Update()
    {
        if (!_gameController.Started) {
            return;
        }
        if (!_isChoking){
            for (int i = 0; i < _numWaypoints - 1; i++) {
                KeyCode keyCode = _keySequence[i];
                if (Input.GetKeyDown(keyCode)) {
                    TrySwallow(i);
                }
            }
        }
        else {
            for (int i = 0; i < _numWaypoints - 2; i++) {
                GameObject food = _throatStages[i];
                if (food != null) {
                    _currThroatFood[i] = 0;
                    StartCoroutine(CoughCoroutine(food));
                }
                _throatStages[i] = null;
            }
        }
    }
    private void ChewFood() {
        for (int i = 0; i < _numStages - 1; i++) {
            float transfer = _foodPerChew;
            if (_currChewedFood[i] < transfer) {
                transfer = _currChewedFood[i];
            }
            _deltaChewedFood[i] = transfer;
        }

        for (int i = 0; i < _numStages - 1; i++) {
            _currChewedFood[i] -= _deltaChewedFood[i];
            _currChewedFood[i+1] += _deltaChewedFood[i];
        }

        ScaleSprites();
        RotateSprites();
    }
    private void ScaleSprites() {
        for (int i = 0; i < _numStages; i++) {
            float rootCurr = Mathf.Sqrt(_currChewedFood[i]);
            rootCurr = Mathf.Clamp(rootCurr, 0f, 0.8f);
            _foodStages[i].transform.localScale = new Vector2(rootCurr, rootCurr);
        }
    }
    private void RotateSprites() {
        for (int i = 0; i < _numStages; i++) {
            float random = UnityEngine.Random.Range(0,360f);
            _foodStages[i].transform.Rotate(0,0,random);
        }
    }
    private IEnumerator BeginSwallowCoroutine(GameObject food, float foodAmount) {
        Vector2 entrancePos = _throatWaypoints[0].position;
        while (Vector2.Distance(entrancePos, food.transform.position) > 0.001f) {
            if (_isChoking) {
                StartCoroutine(CoughCoroutine(food));
                yield break;
            }
            food.transform.position = Vector2.MoveTowards(food.transform.position, entrancePos, 8f * Time.deltaTime);
            yield return null;
        }
        StartCoroutine(SwallowCoroutine(food, 0, foodAmount));
    }
    private IEnumerator SwallowCoroutine(GameObject food, int stage, float foodAmount) {
        print($"Stage: {stage}");

        Vector2 waypointPos = _throatWaypoints[stage + 1].position;
        while (Vector2.Distance(waypointPos, food.transform.position) > 0.001f) {
            if (_isChoking) {
                StartCoroutine(CoughCoroutine(food));
                yield break;
            }
            food.transform.position = Vector2.MoveTowards(food.transform.position, waypointPos, 4f * Time.deltaTime);
            yield return null;
        }

        if (stage == _numWaypoints - 2) {
            _totalSwallowed += foodAmount;
            OnSwallow();
            Destroy(food);
            yield break;
        }

        // Merging
        if (_currThroatFood[stage] > 0) {
            float newFoodAmount = _currThroatFood[stage] + foodAmount;
            _currThroatFood[stage] = newFoodAmount;

            Destroy(_throatStages[stage]);

            float rootCurr = Mathf.Sqrt(newFoodAmount);

            food.transform.localScale = new Vector2(rootCurr, rootCurr);
        }
        else {
            _currThroatFood[stage] = foodAmount;
        }
        _throatStages[stage] = food;

        if (_currThroatFood[stage] > _chokingThreshold) {
                _chokingController.Choke();
        }
    }
    private IEnumerator CoughCoroutine(GameObject food) {
        Rigidbody2D rb2D = food.GetComponent<Rigidbody2D>();
        Vector2 waypointPos = _throatWaypoints[0].position;
        while (Vector2.Distance(waypointPos, food.transform.position) > 0.001f) {
                food.transform.position = Vector2.MoveTowards(food.transform.position, waypointPos, 16f * Time.deltaTime);
                yield return null;
        }
        rb2D.isKinematic = false;
        rb2D.AddForce(Vector2.left * 16f, ForceMode2D.Impulse);
        yield return new WaitForSeconds(1f);
        Destroy(food);
    }
    private void TrySwallow(int stage) {
        if (stage == 0) {
            if (_currChewedFood[_numStages - 1] > 0) {
                MouthToThroat();
            }
        }
        else {
            if (_currThroatFood[stage - 1] > 0) {
                ThroatSwallow(stage);
            }
        }
    }
    private void MouthToThroat() {
        float foodAmount = _currChewedFood[_numStages - 1];

        if (foodAmount > _chokingThreshold) {
            foodAmount = 0.4f;
            _currChewedFood[_numStages - 1] -= foodAmount;
        }
        else {
            _currChewedFood[_numStages - 1] = 0f;
        }
        _totalChewedFood -= foodAmount;

        GameObject newFood = Instantiate(_lastFoodStage, _lastFoodStage.transform.position, _lastFoodStage.transform.rotation);

        float rootCurr = Mathf.Sqrt(foodAmount);
        rootCurr = Mathf.Clamp(rootCurr, 0f, 0.8f);
        newFood.transform.localScale = new Vector2(rootCurr, rootCurr);

        newFood.GetComponentInChildren<SpriteRenderer>().sortingOrder = 6; // Find a better way to do this

        StartCoroutine(BeginSwallowCoroutine(newFood, foodAmount));
        ScaleSprites();
    }
    private void ThroatSwallow(int stage) {
        if (stage == 0) {
            print("What");
            return;
        }
        GameObject food = _throatStages[stage - 1];

        if (food == null) {
            print("NULL");
            return;
        }

        float foodAmount = _currThroatFood[stage - 1];
        _currThroatFood[stage - 1] = 0;
        _throatStages[stage - 1] = null;

        StartCoroutine(SwallowCoroutine(food, stage, foodAmount));
    }
    private void PrintSwallow() {
        print("swallow");
    }

    public void InsertFood(float percentage) {
        _currChewedFood[0] += percentage;
        _totalChewedFood += percentage;
        
        ScaleSprites();
    }
}
