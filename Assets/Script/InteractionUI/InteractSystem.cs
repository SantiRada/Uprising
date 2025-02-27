using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractSystem : MonoBehaviour {

    [Range(0.5f, 1.5f)] public float spaceDetection = 1.25f;

    [Header("UI Key")]
    public GameObject objectUI;
    public int title;
    public bool showKey;
    public bool needCursor;

    private Collider thisCollider;
    private InterfaceInWorld _interface;
    private Player _player;
    [HideInInspector] public bool inUI = false;
    private bool inDetect = false;
    private bool _canOpen = false;

    public event Action actionOpenUI;
    public event Action actionCloseUI;

    private InputPlayerSystem _inputs;
    private InputInterfaceSystem _interfaceSystem;
    private PlayerInput _inputPlayer;

    private void Awake()
    {
        _player = FindAnyObjectByType<Player>();
        thisCollider = GetComponent<Collider>();
        _interface = FindAnyObjectByType<InterfaceInWorld>();

        _inputs = FindAnyObjectByType<InputPlayerSystem>();
        _inputPlayer = _inputs.GetComponent<PlayerInput>();
        _interfaceSystem = FindAnyObjectByType<InputInterfaceSystem>();
    }
    private void Start()
    {
        objectUI.gameObject.SetActive(false);

        _inputs.interact += Interact;
        _inputs.useInventory += ClicClose;
        _inputs.useEscape += ClicClose;
        _interfaceSystem.useBack += ClicClose;
    }
    private void Interact()
    {
        if(_canOpen && !inUI) OpenUI();
    }
    private void OnDestroy()
    {
        _inputs.useInventory -= ClicClose;
        _inputs.useEscape -= ClicClose;
        _interfaceSystem.useBack -= ClicClose;

        _inputs.interact -= Interact;
    }
    private void ClicClose()
    {
        if (inUI) CloseUI();
    }
    private void Update()
    {
        if (PauseMenu.state == StatePlayer.Pause) return;

        if (Vector3.Distance(_player.transform.position, transform.position) < spaceDetection)
        {
            inDetect = true;

            // Crear el rayo desde el centro de la cámara hacia adelante
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (inUI)
                {
                    _interface.CloseUI();
                    return;
                }

                if (hit.collider == thisCollider)
                {
                    string keyName = KeyManager.GetNameKey("Interact");

                    _interface.OpenUI(LanguageSystem.GetValue("game", title), keyName, showKey);
                    _canOpen = true;
                }
                else
                {
                    _canOpen = false;
                    _interface.CloseUI();
                }
            }
        }
        else if (inDetect)
        {
            _interface.CloseUI();
            inDetect = false;
        }
    }
    private void OpenUI()
    {
        actionOpenUI?.Invoke();

        PauseMenu.SetState(StatePlayer.UserInterface);

        objectUI.gameObject.SetActive(true);
        objectUI.transform.SetAsLastSibling();
        _player.canMove = false;
        inUI = true;

        if (needCursor)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
    private void CloseUI()
    {
        actionCloseUI?.Invoke();

        PauseMenu.SetState(StatePlayer.Game);

        objectUI.transform.SetAsFirstSibling();
        objectUI.gameObject.SetActive(false);
        
        _canOpen = false;

        _player.canMove = true;
        inUI = false;

        if (needCursor)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}