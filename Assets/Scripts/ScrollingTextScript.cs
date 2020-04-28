using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingTextScript : MonoBehaviour
{
    [SerializeField]
    private float _textSpeed = 2f;

    private RectTransform _rectTransform;

    private void Start() {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Update() {

        _rectTransform.Translate(Vector2.left * _textSpeed * Time.deltaTime);

        if (_rectTransform.anchoredPosition.x + _rectTransform.rect.width / 2 < -1920 / 2) {
            Vector2 oldPos = _rectTransform.anchoredPosition;
            _rectTransform.anchoredPosition = new Vector2(oldPos.x + _rectTransform.rect.width * 2, oldPos.y);
        }
    }
}
