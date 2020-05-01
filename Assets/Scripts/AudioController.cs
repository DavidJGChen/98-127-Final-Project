using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioClip _buttonHover;
    [SerializeField] private AudioClip _buttonClick;
    private AudioSource _audioSource;
    private void Awake() {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("GameController");

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(gameObject);
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayHover() {
        _audioSource.clip = _buttonHover;
        _audioSource.Play();
    }

    public void PlayClick() {
        _audioSource.clip = _buttonClick;
        _audioSource.Play();
    }
}
