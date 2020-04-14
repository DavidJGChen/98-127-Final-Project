using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ThroatController : MonoBehaviour
{
    private JawController jawController;
    [SerializeField]
    private GameObject chewedStage1;
    [SerializeField]
    private GameObject chewedStage2;

    [SerializeField]
    private Transform[] throatWaypoints; 

    private float currHotdogsStage1 = 0;
    private float currHotdogsStage2 = 0;


    public event Action SwallowEvent;
    // Start is called before the first frame update
    private void Start()
    {
        jawController = FindObjectOfType<JawController>();

        jawController.ChewEvent += ChewFood;

        SwallowEvent += PrintSwallow;
        SwallowEvent += Swallow;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) {
            TrySwallow();
        }
    }

    private void TempSwallow() {
        GameObject newFood = Instantiate(chewedStage2, chewedStage2.transform.position, chewedStage2.transform.rotation);

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
        if (currHotdogsStage2 > 0) {
            SwallowEvent();
        }
    }

    private void ChewFood() {
        if (currHotdogsStage1 > 0 || currHotdogsStage2 > 0) {
            float transfer = 0.2f;
            if (currHotdogsStage1 < transfer) {
                transfer = currHotdogsStage1;
            }
            currHotdogsStage1 -= transfer;
            currHotdogsStage2 += transfer;
            print("food being chewed");
            ScaleSprites();
            RotateSprites();
        }
    }

    private void ScaleSprites() {
        float rootcurr1 = Mathf.Sqrt(currHotdogsStage1);
        float rootcurr2 = Mathf.Sqrt(currHotdogsStage2);

        Vector2 newScaleStage1 = new Vector2(rootcurr1, rootcurr1);
        Vector2 newScaleStage2 = new Vector2(rootcurr2, rootcurr2);
        chewedStage1.transform.localScale = newScaleStage1;
        chewedStage2.transform.localScale = newScaleStage2;
    }

    private void RotateSprites() {
        float random1 = UnityEngine.Random.Range(0,360f);
        float random2 = UnityEngine.Random.Range(0,360f);

        chewedStage1.transform.Rotate(0,0,random1);
        chewedStage2.transform.Rotate(0,0,random2);
    }

    private void Swallow() {
        TempSwallow();
        currHotdogsStage2 = 0;
        ScaleSprites();
    }

    public void InsertFood(float percentage) {
        currHotdogsStage1 += percentage;
        ScaleSprites();
    }

    public float CurrentHotDogs {
        get => currHotdogsStage1 + currHotdogsStage2;
    }

    private void PrintSwallow() {
        print("swallow");
    }
}
