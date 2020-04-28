using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    private JawController _jawController;
    private ThroatController _throatController;
    private ChokingController _chokingController;
    private GameController _gameController;
    [SerializeField]
    private TMP_Text _currHotdogText;
    [SerializeField]
    private GameObject _tempExclamation;
    [SerializeField]
    private GameObject _resultsPanel;
    [SerializeField]
    private TMP_Text _timerText;

    [SerializeField]
    private SpriteRenderer[] _keyRenderers;

    [SerializeField]
    private Sprite _keyUpSprite;
    [SerializeField]
    private Sprite _keyDownSprite;

    [SerializeField]
    private static readonly KeyCode[] _keySequence = {KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F};

    private float _currHotdogs = 0f;
    private bool _gameOver = false;
    private bool _gameStarted = false;

    private void Start()
    {
        _jawController = FindObjectOfType<JawController>();
        _throatController = FindObjectOfType<ThroatController>();
        _chokingController = FindObjectOfType<ChokingController>();
        _gameController = FindObjectOfType<GameController>();

        _jawController.OnChew += OnChewOrSwallow;
        _throatController.OnSwallow += OnChewOrSwallow;
        _chokingController.OnChoke += OnChoke;
        _chokingController.OnUnchoke += OnUnchoke;
    }
    private void Update() {

        for (int i = 0; i < _keySequence.Length; i++) {
            KeyCode keyCode = _keySequence[i];
            if (Input.GetKeyDown(keyCode)) {
                _keyRenderers[i].sprite = _keyDownSprite;
            }
            if (Input.GetKeyUp(keyCode)) {
                _keyRenderers[i].sprite = _keyUpSprite;
            }
        }

        if (_gameOver) {
            return;
        }

        if (_gameController.Started) {
            if (!_gameStarted) {
                _gameStarted = true;
            }
            _timerText.text = _gameController.TimeLeft.ToString("n2");
        }
        else {
            if (!_gameStarted) {
                _timerText.text = ((int)_gameController.FreezeTime + 1).ToString();
            }
            else {
                _gameOver = true;
                _timerText.text = "TIME!";
                Invoke("DisplayResults", 2f);
            }
        }
    }
    private void DisplayResults() {
        _resultsPanel.SetActive(true);
        _resultsPanel.GetComponentInChildren<TMP_Text>().text = $"x {_currHotdogs.ToString("n1")}";
    }

    private void OnChewOrSwallow() {
        Invoke("UpdateHotdogCount", 0.05f); // Smoll delay
    }
    private void UpdateHotdogCount() {
        _currHotdogs = _throatController.TotalSwallowed;
        _currHotdogText.text = $"x {_currHotdogs.ToString("n2")}";
    }

    private void OnChoke() {
        _tempExclamation.SetActive(true);
    }
    private void OnUnchoke() {
        _tempExclamation.SetActive(false);
    }
}
