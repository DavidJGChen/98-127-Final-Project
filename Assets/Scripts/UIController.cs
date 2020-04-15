using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    private JawController _jawController;
    private ThroatController _throatController;
    [SerializeField]
    private TMP_Text _currHotdogText;

    private float _currHotdogs = 0f;

    private void Start()
    {
        _jawController = FindObjectOfType<JawController>();
        _throatController = FindObjectOfType<ThroatController>();

        _jawController.OnChew += OnChewOrSwallow;
        _throatController.OnSwallow += OnChewOrSwallow;
    }

    private void OnChewOrSwallow() {
        Invoke("UpdateHotdogCount", 0.05f); // Smoll delay
    }
    private void UpdateHotdogCount() {
        _currHotdogs = _throatController.TotalSwallowed;
        _currHotdogText.text = _currHotdogs.ToString("n2");
    }
}
