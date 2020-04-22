using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    private JawController _jawController;
    private ThroatController _throatController;
    private ChokingController _chokingController;
    [SerializeField]
    private TMP_Text _currHotdogText;
    [SerializeField]
    private GameObject _tempExclamation;

    private float _currHotdogs = 0f;

    private void Start()
    {
        _jawController = FindObjectOfType<JawController>();
        _throatController = FindObjectOfType<ThroatController>();
        _chokingController = FindObjectOfType<ChokingController>();

        _jawController.OnChew += OnChewOrSwallow;
        _throatController.OnSwallow += OnChewOrSwallow;
        _chokingController.OnChoke += OnChoke;
        _chokingController.OnUnchoke += OnUnchoke;
    }

    private void OnChewOrSwallow() {
        Invoke("UpdateHotdogCount", 0.05f); // Smoll delay
    }
    private void UpdateHotdogCount() {
        _currHotdogs = _throatController.TotalSwallowed;
        _currHotdogText.text = _currHotdogs.ToString("n2");
    }

    private void OnChoke() {
        _tempExclamation.SetActive(true);
    }
    private void OnUnchoke() {
        _tempExclamation.SetActive(false);
    }
}
