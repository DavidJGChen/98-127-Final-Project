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

    public void Credits() {
        if (SceneManager.GetSceneByName("CreditsScene") == null) {
            print("Credits does not exist yet!");
            return;
        }

        SceneManager.LoadScene("CreditsScene");
    }

    public void Quit() {
        Application.Quit(0);
    }
}
