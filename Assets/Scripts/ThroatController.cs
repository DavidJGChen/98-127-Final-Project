using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ThroatController : MonoBehaviour
{
    private JawController _jawController;

    [SerializeField]
    private GameObject[] _foodStages;
    private int _numStages;
    private GameObject _firstFoodStage;
    private GameObject _lastFoodStage;

    [SerializeField]
    private Transform[] _throatWaypoints;

    // Settings
    [SerializeField]
    private float foodPerChew = 0.2f;

    // Other stuff
    private float[] _currChewedFood;
    private float _totalChewedFood;
    private float[] _deltaChewedFood;
    private float _totalSwallowed = 0;
    public float TotalSwallowed {
        get => _totalSwallowed;
    }

    public event Action OnSwallow;

    private void Start()
    {
        _jawController = FindObjectOfType<JawController>();
        _jawController.OnChew += ChewFood;

        _numStages = _foodStages.Length;
        _currChewedFood = new float[_numStages];
        _deltaChewedFood = new float[_numStages - 1];

        _firstFoodStage = _foodStages[0];
        
        _lastFoodStage = _foodStages[_numStages - 1];
        
        for(int i = 0; i < _numStages; i++) {
            _currChewedFood[i] = 0f;
        }

        OnSwallow += PrintSwallow;
        OnSwallow += Swallow;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) {
            TrySwallow();
        }
    }
    private void TempSwallow() {
        GameObject newFood = Instantiate(_lastFoodStage, _lastFoodStage.transform.position, _lastFoodStage.transform.rotation);
        newFood.GetComponentInChildren<SpriteRenderer>().sortingOrder = 6; // Find a better way to do this
        StartCoroutine(TempSwallowCoroutine(newFood));
    }
    private IEnumerator TempSwallowCoroutine(GameObject food) {
        foreach(Transform waypoint in _throatWaypoints) {
            Vector2 waypointPos = waypoint.position;
            while (Vector2.Distance(waypointPos, food.transform.position) > 0.001f) {
                food.transform.position = Vector2.MoveTowards(food.transform.position, waypointPos, 4f * Time.deltaTime);
                yield return null;
            } 
        }
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
        TempSwallow(); // Graphics
        _totalChewedFood -= _currChewedFood[_numStages - 1];
        _totalSwallowed += _currChewedFood[_numStages - 1];
        _currChewedFood[_numStages - 1] = 0;
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
