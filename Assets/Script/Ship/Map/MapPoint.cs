using TMPro;
using UnityEngine;

public class MapPoint : MonoBehaviour {

    [Header("Content UI")]
    public GameObject factionUI;
    public TextMeshProUGUI titleFaction;

    [Header("")]
    public float detectionRange = 50f;

    private MapPlayer _player;
    private RectTransform _thisTransform;
    private RectTransform _playerTransform;

    private void Start()
    {
        _thisTransform = GetComponent<RectTransform>();

        _player = FindAnyObjectByType<MapPlayer>();
        _playerTransform = _player.GetComponent<RectTransform>();

        factionUI.SetActive(false);
    }
    private void Update()
    {
        float distance = Vector2.Distance(_playerTransform.anchoredPosition, GetComponent<RectTransform>().anchoredPosition);

        if (distance <= detectionRange)
        {
            _isOpen = false;
        }
        else factionUI.SetActive(false);
    }
}
