using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private HotdogSpawnerController _hotdogSpawnerController;

    [SerializeField]
    private GameObject _pausePanel;

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
    private void Awake() {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("GameController");

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }
    }
    private void Start()
    {
        _hotdogSpawnerController = FindObjectOfType<HotdogSpawnerController>();

        InitGame();
    }
    public void InitGame() {

        _started = false;
        _timeLeft = _initialTime;
        _freezeTime = 3f;

        UnPauseGame();

        Invoke("PlayMusic", 0.5f);
    }
    private void PlayMusic() {
        GetComponent<AudioSource>().Play();
    }
    private void StopMusic() {
        GetComponent<AudioSource>().Stop();
    }
    // Update is called once per frame
    private void Update()
    {
        Scene currScene = SceneManager.GetActiveScene();

        if (currScene.name == "SampleScene") {

            if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape)) {
                if (!_pausePanel.activeInHierarchy) {
                    PauseGame();
                }
                else if (_pausePanel.activeInHierarchy) {
                    UnPauseGame();
                }
            }

            if (!_started) {
                if (_freezeTime > 0) {
                    _freezeTime -= Time.deltaTime;
                    if (_freezeTime <= 0) {
                        _started = true;
                        if (_hotdogSpawnerController == null) {
                            _hotdogSpawnerController = FindObjectOfType<HotdogSpawnerController>();
                        }
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
                        Cursor.visible = true;
                        Cursor.lockState = CursorLockMode.None;
                        // StopMusic();
                    }
                }
            }
        }
    }

    public void PauseGame() {
        Time.timeScale = 0;
        _pausePanel.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void UnPauseGame() {
        Time.timeScale = 1;
        _pausePanel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ResetGame() {
        Scene currScene = SceneManager.GetActiveScene();

        if (currScene.name != "SampleScene") {
            print("Error: Calling 'ResetGame' outside of SampleScene");
            return;
        }
        SceneManager.LoadScene("SampleScene");
        InitGame();
    }

    public void BackToMenu() {
        Scene currScene = SceneManager.GetActiveScene();

        if (currScene.name != "SampleScene") {
            print("Error: Calling 'BackToMenu' outside of SampleScene");
            return;
        }
        print("wtf");
        SceneManager.LoadScene("TitleScene");
    }
}
