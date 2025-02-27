using UnityEngine;
using TMPro;

public class InterfaceInWorld : MonoBehaviour {

    public GameObject prefabUI;
    public TextMeshProUGUI textTitleUI;
    public GameObject keyUI;
    public TextMeshProUGUI textKeyUI;

    private void Start() { prefabUI.gameObject.SetActive(false); }
    public void OpenUI(string title, string key = "", bool show = false)
    {
        prefabUI.SetActive(true);

        keyUI.gameObject.SetActive(show);

        textTitleUI.text = title;
        textKeyUI.text = key;
    }
    public void CloseUI() { prefabUI.SetActive(false); }
}