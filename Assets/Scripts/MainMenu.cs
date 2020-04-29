using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Play() {
        SceneManager.LoadScene("SampleScene");

        GameController temp = FindObjectOfType<GameController>();

        if (temp != null) {
            temp.InitGame();
        }
    }

    public void Controls() {
        SceneManager.LoadScene("ControlsScene");
    }

    public void Credits() {
        SceneManager.LoadScene("CreditsScene");
    }

    public void Quit() {
        Application.Quit(0);
    }

    public void Back() {
        SceneManager.LoadScene("TitleScene");
    }
}
