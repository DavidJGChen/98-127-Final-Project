using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ChokingController : MonoBehaviour
{
    [SerializeField]
    private GameObject _sweatParticles;
    [SerializeField]
    private GameObject _eyeShadow;
    [SerializeField]
    private GameObject _eyeBrow;
    [SerializeField]
    private GameObject _eyePupil;
    private AudioSource _coughSound;

    
    [SerializeField]
    private float _defaultChokeSeconds = 2f;

    private float _currentChokingTime;
    private bool _isChoking;
    public bool IsChoking {
        get => _isChoking;
    }

    public event Action OnChoke;
    public event Action OnUnchoke;

    private void Start() {
        _coughSound = GetComponent<AudioSource>();

        OnChoke += () => _sweatParticles.SetActive(true);
        OnUnchoke += () => _sweatParticles.SetActive(false);

        OnChoke += MoveEyebrowsUp;
        OnUnchoke += MoveEyebrowsDown;

        OnChoke += ShrinkPupil;
        OnUnchoke += ExpandPupil;

        OnChoke += Cough;
    }
    private void FixedUpdate() {
        if (_currentChokingTime > 0) {
            _currentChokingTime -= Time.deltaTime;
            if (_currentChokingTime <= 0) {
                Unchoke();
            }
        }
    }

    private void Cough() {
        _coughSound.Play();
    }

    private void MoveEyebrowsUp() {
        _eyeBrow.transform.Translate(Vector2.up * 0.1f);
        _eyeShadow.transform.Translate(Vector2.up * 0.1f);
    }
    private void MoveEyebrowsDown() {
        _eyeBrow.transform.Translate(Vector2.down * 0.1f);
        _eyeShadow.transform.Translate(Vector2.down * 0.1f);
    }

    private void ShrinkPupil() {
        _eyePupil.transform.localScale *= 0.5f;
    }
    private void ExpandPupil() {
        _eyePupil.transform.localScale *= 2f;
    }


    private void Unchoke() {
        _isChoking = false;
        OnUnchoke();
    }
    public void Choke() {
        Choke(_defaultChokeSeconds);
    }
    public void Choke(float seconds) {
        if (_isChoking) {
            _currentChokingTime = seconds;
        }
        else {
            _currentChokingTime = seconds;
            _isChoking = true;
            OnChoke();
        }
    }
}
