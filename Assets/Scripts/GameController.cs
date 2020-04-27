using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private HotdogSpawnerController _hotdogSpawnerController;

    [SerializeField]
    private float _initialTime = 15f;

    private float _freezeTime = 3f;
    public float FreezeTime {
        get => _freezeTime;
    }
    private float _timeLeft;
    public float TimeLeft {
        get => _timeLeft;
    }

    private bool _started = false;
    public bool Started {
        get => _started;
    }
    // Start is called before the first frame update
    private void Start()
    {
        _hotdogSpawnerController = FindObjectOfType<HotdogSpawnerController>();

        _started = false;
        _timeLeft = _initialTime;

        DontDestroyOnLoad(this.gameObject);
    }
    private void InitGame() {
        _started = false;
        _timeLeft = _initialTime;
        _freezeTime = 3f;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0)){
            SceneManager.LoadScene("SampleScene");
            InitGame();
            return;
        }

        Scene currScene = SceneManager.GetActiveScene();

        if (currScene.name == "SampleScene") {
            if (!_started) {
                if (_freezeTime > 0) {
                    _freezeTime -= Time.deltaTime;
                    if (_freezeTime <= 0) {
                        _started = true;
                        _hotdogSpawnerController.SpawnHotdog();
                    }
                }
            }
            else {
                if (_timeLeft > 0) {
                    _timeLeft -= Time.deltaTime;
                    if (_timeLeft < 0) {
                        _timeLeft = 0;
                        _started = false;
                    }
                }
            }
        }
    }
}
