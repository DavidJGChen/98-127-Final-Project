using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ThroatController : MonoBehaviour
{
    private JawController jawController;

    [SerializeField]
    private GameObject[] chewedStages;
    private GameObject firstChewedStage;
    private GameObject lastChewedStage;
    private int numStages;

    [SerializeField]
    private Transform[] throatWaypoints; 


    private float[] currHotdogs;
    private float sumHotdogs;
    private float[] temp;

    private float totalEaten = 0;

    public event Action SwallowEvent;
    // Start is called before the first frame update
    private void Start()
    {
        jawController = FindObjectOfType<JawController>();

        jawController.ChewEvent += ChewFood;

        SwallowEvent += PrintSwallow;
        SwallowEvent += Swallow;

        numStages = chewedStages.Length;

        firstChewedStage = chewedStages[0];
        lastChewedStage = chewedStages[numStages - 1];

        currHotdogs = new float[numStages];
        temp = new float[numStages - 1];
        for(int i = 0; i < numStages; i++) {
            currHotdogs[i] = 0f;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) {
            TrySwallow();
        }
    }

    private void TempSwallow() {
        GameObject newFood = Instantiate(lastChewedStage, lastChewedStage.transform.position, lastChewedStage.transform.rotation);
        newFood.GetComponentInChildren<SpriteRenderer>().sortingOrder = 6;
        StartCoroutine(TempSwallowCoroutine(newFood));
    }

    private IEnumerator TempSwallowCoroutine(GameObject food) {
        foreach(Transform waypoint in throatWaypoints) {
            Vector2 waypointPos = waypoint.position;
            while (Vector2.Distance(waypointPos, food.transform.position) > 0.001f) {
                food.transform.position = Vector2.MoveTowards(food.transform.position, waypointPos, 4f * Time.deltaTime);
                yield return null;
            } 
        }
        Destroy(food);
    }

    private void TrySwallow() {
        if (currHotdogs[numStages - 1] > 0) {
            SwallowEvent();
        }
    }

    private void ChewFood() {
        for (int i = 0; i < numStages - 1; i++) {
            float transfer = 0.2f;
            if (currHotdogs[i] < transfer) transfer = currHotdogs[i];
            temp[i] = transfer;
        }

        for (int i = 0; i < numStages - 1; i++) {
            currHotdogs[i] -= temp[i];
            currHotdogs[i+1] += temp[i];
        }

        ScaleSprites();
        RotateSprites();
    }

    private void ScaleSprites() {
        for (int i = 0; i < numStages; i++) {
            float rootCurr = Mathf.Sqrt(currHotdogs[i]);
            chewedStages[i].transform.localScale = new Vector2(rootCurr, rootCurr);
        }
    }

    private void RotateSprites() {
        for (int i = 0; i < numStages; i++) {
            float random = UnityEngine.Random.Range(0,360f);
            chewedStages[i].transform.Rotate(0,0,random);
        }
    }

    private void Swallow() {
        TempSwallow(); // Graphics
        sumHotdogs -= currHotdogs[numStages - 1];
        totalEaten += currHotdogs[numStages - 1];
        currHotdogs[numStages - 1] = 0;
        ScaleSprites();
    }

    public void InsertFood(float percentage) {
        currHotdogs[0] += percentage;
        sumHotdogs += percentage;
        ScaleSprites();
    }

    public float CurrentHotDogs {
        get => sumHotdogs;
    }

    public float TotalEaten {
        get => totalEaten;
    }

    private void PrintSwallow() {
        print("swallow");
    }
}
