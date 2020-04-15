using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    private JawController jawController;
    private ThroatController throatController;
    public TMP_Text currHotdogText;

    float currHotdogs = 0f;
    void Start()
    {
        jawController = FindObjectOfType<JawController>();
        throatController = FindObjectOfType<ThroatController>();

        jawController.ChewEvent += OnChewOrSwallow;
        throatController.SwallowEvent += OnChewOrSwallow;
    }

    private void OnChewOrSwallow() {
        Invoke("UpdateHotdogCount", 0.05f);
    }

    private void UpdateHotdogCount() {
        currHotdogs = throatController.TotalEaten;
        currHotdogText.text = currHotdogs.ToString("n2");
    }
}
