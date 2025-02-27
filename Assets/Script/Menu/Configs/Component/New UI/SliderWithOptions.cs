using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SliderWithOptions : MonoBehaviour {

    public bool sendToSettings = true;
    public string send;
    public List<int> options = new List<int>();
    public int position = 0;
    public float delayToMove;
    [HideInInspector] public int _selected = 0;
    private bool isSelect = false;
    private bool _canMove = true;

    private float _baseDelay;

    public TextMeshProUGUI text;
    private Slider _slider;

    private InputInterfaceSystem _inputs;
    private Settings _settings;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        _inputs = FindAnyObjectByType<InputInterfaceSystem>();
        _settings = FindAnyObjectByType<Settings>();
    }
    private void Start()
    {
        _baseDelay = delayToMove;
        _inputs.useMove += Move;
        _inputs.useSelect += SelectValues;

        _slider.minValue = 0;
        _slider.maxValue = (options.Count - 1);

        SetValues();
    }
    private void Update()
    {
        if (!isSelect) return;

        if(delayToMove > 0)
        {
            _canMove = false;
            delayToMove -= Time.deltaTime;
        }
        else
        {
            _canMove = true;
        }
    }
    private void OnDestroy()
    {
        _inputs.useMove -= Move;
        _inputs.useSelect -= SelectValues;
    }
    private void ResetValues()
    {
        isSelect = false;

        position = _selected;
        SetValues();
    }
    private void SelectValues()
    {
        if (isSelect)
        {
            _selected = position;

            if (sendToSettings) _settings.SendValue(_selected, send);
        }
    }
    private void Move()
    {
        if (EventSystem.current.currentSelectedGameObject == this.gameObject) { isSelect = true; }

        if (!_canMove || !isSelect) return;

        if (_inputs.movement.y != 0)
        {
            ResetValues();
            return;
        }

        int moveH = _inputs.movement.x != 0 ? _inputs.movement.x > 0 ? 1 : -1 : 0;

        if (moveH == 0) return;

        if(moveH > 0)
        {
            position++;

            if (position >= options.Count) position = 0;
        }
        else
        {
            position--;

            if (position < 0) position = options.Count - 1;
        }

        delayToMove = _baseDelay;

        SetValues();
    }
    public void SetValues()
    {
        if (sendToSettings) text.text = LanguageSystem.GetValue("menu", options[position]);
        else text.text = options[position].ToString();

        _slider.value = position;
    }
}