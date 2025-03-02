using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MoveInUI : MonoBehaviour {

    [Header("Sectioners")]
    public RectTransform selector;
    private bool _isOpen = false;

    [Header("Inputs")]
    private InputInterfaceSystem _inputs;

    private void Awake()
    {
        _inputs = FindAnyObjectByType<InputInterfaceSystem>();
    }
    private void OnEnable()
    {
        _inputs.useBack += CloseUI;
        _inputs.useMove += ReviewMove;

        OpenUI();
    }
    private void OnDisable()
    {
        _inputs.useBack -= CloseUI;
        _inputs.useMove -= ReviewMove;
    }
    private void ReviewMove()
    {
        if (gameObject.activeInHierarchy) StartCoroutine("ReviewMovement");
    }
    private IEnumerator ReviewMovement()
    {
        yield return null;

        if (_isOpen)
        {
            RectTransform selection = EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>();

            Vector3 newPosition = new Vector3(selector.position.x, selection.position.y, selector.position.z);
            selector.SetPositionAndRotation(newPosition, Quaternion.identity);
        }
    }
    private void OpenUI()
    {
        if (_isOpen) return;

        MenuController.state = StatePlayer.UserInterface;
        
        _isOpen = true;

        selector.SetAsFirstSibling();

        Selectable selection = gameObject.GetComponentInChildren<Selectable>();
        selection.Select();

        Vector3 newPosition = new Vector3(selector.position.x, selection.GetComponent<RectTransform>().position.y, selector.position.z);
        selector.SetPositionAndRotation(newPosition, Quaternion.identity);
    }
    private void CloseUI()
    {
        if (!_isOpen) return;

        MenuController.state = StatePlayer.Game;
        _isOpen = false;
    }
}