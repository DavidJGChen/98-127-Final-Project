using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ThroatController : MonoBehaviour
{
    private JawController _jawController;
    private ChokingController _chokingController;

    [SerializeField]
    private GameObject[] _foodStages;
    private int _numStages;
    private GameObject _firstFoodStage;
    private GameObject _lastFoodStage;

    [SerializeField]
    private Transform[] _throatWaypoints;
    private int _numWaypoints;

    // Settings
    [SerializeField]
    private float foodPerChew = 0.2f;

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
        _currThroatFood = new float[_numWaypoints];

        OnSwallow += PrintSwallow;
        OnSwallow += Swallow;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && !_isChoking) {
            TrySwallow();
        }
    }
    private void TempSwallow(float foodAmount) {
        GameObject newFood = Instantiate(_lastFoodStage, _lastFoodStage.transform.position, _lastFoodStage.transform.rotation);
        newFood.GetComponentInChildren<SpriteRenderer>().sortingOrder = 6; // Find a better way to do this
        StartCoroutine(TempSwallowCoroutine(newFood, foodAmount));
    }
    private void FinishSwallow() {

    }
    private IEnumerator TempSwallowCoroutine(GameObject food, float foodAmount) {
        foreach(Transform waypoint in _throatWaypoints) {
            Vector2 waypointPos = waypoint.position;
            while (Vector2.Distance(waypointPos, food.transform.position) > 0.001f) {
                if (_isChoking) {
                    StartCoroutine(CoughCoroutine(food));
                    yield break;
                }
                food.transform.position = Vector2.MoveTowards(food.transform.position, waypointPos, 4f * Time.deltaTime);
                yield return null;
            } 
        }
        _totalSwallowed += foodAmount;
        Destroy(food);
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

    private void TrySwallow() {
        if (_currChewedFood[_numStages - 1] > 0) {
            OnSwallow();
        }
    }
    private void ChewFood() {
        for (int i = 0; i < _numStages - 1; i++) {
            float transfer = foodPerChew;
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
            _foodStages[i].transform.localScale = new Vector2(rootCurr, rootCurr);
        }
    }
    private void RotateSprites() {
        for (int i = 0; i < _numStages; i++) {
            float random = UnityEngine.Random.Range(0,360f);
            _foodStages[i].transform.Rotate(0,0,random);
        }
    }
    private void Swallow() {
        _totalChewedFood -= _currChewedFood[_numStages - 1];
        float foodAmount = _currChewedFood[_numStages - 1];
        _currChewedFood[_numStages - 1] = 0;
        TempSwallow(foodAmount); // Graphics
        ScaleSprites();
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
