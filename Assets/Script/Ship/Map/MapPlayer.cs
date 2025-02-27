using UnityEngine;

public class MapPlayer : MonoBehaviour {

    public float moveSpeed = 300f;
    private RectTransform _character;

    private Vector2 _movement;
    private Vector2 _lastDirection = Vector2.right;

    private InputInterfaceSystem _inputs;

    private void Awake()
    {
        _inputs = FindAnyObjectByType<InputInterfaceSystem>();
        _character = GetComponent<RectTransform>();
    }
    private void Update()
    {
        _movement = _inputs.movement.normalized;

        _character.anchoredPosition += _movement * moveSpeed * Time.deltaTime;

        if (_movement != Vector2.zero)
        {
            _lastDirection = _movement;
            float angle = Mathf.Atan2(_movement.y, _movement.x) * Mathf.Rad2Deg;
            _character.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
    }
}