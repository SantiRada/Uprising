using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ResolutionComponent : MonoBehaviour {

    public int position = 0;
    public float delayToMove;

    private int _selected = 0;
    private bool isSelect = false;
    private bool _canMove = true;
    private float _baseDelay;

    public TextMeshProUGUI text;
    private Slider _slider;

    private InputInterfaceSystem _inputs;

    private List<Resolution> resOptions = new List<Resolution>();

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        _inputs = FindAnyObjectByType<InputInterfaceSystem>();
    }
    private void Start()
    {
        RefillResolution();

        _baseDelay = delayToMove;

        _inputs.useMove += Move;
        _inputs.useSelect += SelectValues;

        _slider.minValue = 0;
        _slider.maxValue = (resOptions.Count - 1);

        SetValues();
    }
    private void Update()
    {
        if (!isSelect) return;

        if (delayToMove > 0)
        {
            _canMove = false;
            delayToMove -= Time.deltaTime;
        }
        else { _canMove = true; }
    }
    private void OnDestroy()
    {
        _inputs.useMove -= Move;
        _inputs.useSelect -= SelectValues;
    }
    private void RefillResolution()
    {
        Resolution[] resolutions = Screen.resolutions;

        for (int i = 6; i < resolutions.Length; i++)
        {
            resOptions.Add(resolutions[i]);
        }
    }
    private void ResetValues()
    {
        isSelect = false;

        position = _selected;
        SetValues();
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

        if (moveH > 0)
        {
            position++;

            if (position >= resOptions.Count) position = 0;
        }
        else
        {
            position--;

            if (position < 0) position = resOptions.Count - 1;
        }

        delayToMove = _baseDelay;

        SetValues();
    }
    public void SetValues()
    {
        text.text = resOptions[position].width + "x" + resOptions[position].height;
        _slider.value = position;
    }
    private void SelectValues()
    {
        if (isSelect)
        {
            _selected = position;
            SetResolution(_selected, Screen.fullScreen);
        }
    }
    public void SetResolutionExtern(int num, bool fullscreen)
    {
        _selected = num;
        SetResolution(_selected, fullscreen);
    }
    private void SetResolution(int index, bool fullscreen)
    {
        if (resOptions.Count <= index) return;

        Resolution resolution = resOptions[index];
        Screen.SetResolution(resolution.width, resolution.height, fullscreen);
    }
}