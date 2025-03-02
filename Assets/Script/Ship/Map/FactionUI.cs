using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FactionUI : MonoBehaviour {

    public float offsetY;
    public Vector2 minPosition;
    public Vector2 maxPosition;
    private RectTransform _thisTransform;

    [Header("Basic")]
    public TextMeshProUGUI titleFaction;

    [Header("Recibe")]
    public Image receiveMineral;
    public TextMeshProUGUI receiveText;

    [Header("Vende")]
    public Image giveMineral;
    public TextMeshProUGUI giveText;

    private void OnEnable()
    {
        _thisTransform = GetComponent<RectTransform>();
    }
    public void OpenUI(int namePlanet, Mineral receiveMineral, Mineral givesMineral, RectTransform target)
    {
        float valueX = Mathf.Clamp(target.localPosition.x, minPosition.x, maxPosition.x);
        float valueY = Mathf.Clamp((target.localPosition.y + offsetY), minPosition.y, maxPosition.y);

        Vector3 newPosition = new Vector3(valueX, valueY, target.localPosition.z);


        // Establece posicionamiento y orden de jerarquía
        _thisTransform.SetAsLastSibling();
        _thisTransform.SetLocalPositionAndRotation(newPosition, Quaternion.identity);

        // Define valores de texto
        titleFaction.text = LanguageSystem.GetValue("rogue", namePlanet);
        receiveText.text = LanguageSystem.GetValue("rogue", receiveMineral.id);
        giveText.text = LanguageSystem.GetValue("rogue", givesMineral.id);
    }
}
