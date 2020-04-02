using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    public JawController jawController;
    public TMP_Text count;

    int chewCount = 0;
    void Start()
    {
        jawController.ChewEvent += OnChew;
    }

    private void OnChew() {
        count.text = chewCount++.ToString();
    }
}
