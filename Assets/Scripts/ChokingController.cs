using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ChokingController : MonoBehaviour
{
    [SerializeField]
    private GameObject _sweatParticles;
    
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
        OnChoke += () => _sweatParticles.SetActive(true);
        OnUnchoke += () => _sweatParticles.SetActive(false);
    }
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            Choke();
        }
    }
    private void FixedUpdate() {
        if (_currentChokingTime > 0) {
            _currentChokingTime -= Time.deltaTime;
            if (_currentChokingTime <= 0) {
                Unchoke();
            }
        }
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
