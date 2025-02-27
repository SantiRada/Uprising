using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderValues : MonoBehaviour {

    public TextMeshProUGUI textValue;
    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }
    private void Update()
    {
        textValue.text = _slider.value.ToString();
    }
}
