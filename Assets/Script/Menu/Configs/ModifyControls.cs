using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ModifyControls : MonoBehaviour {

    [Header("Content Modifier")]
    public TextMeshProUGUI[] _textProblems;

    public GameObject windowsControls;
    public TextMeshProUGUI textTime;
    public float delayToQuit;

    private float timerDelay;
    [HideInInspector] public bool inControls = false;

    public PlayerInput _playerInput;
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    private void Start()
    {
        LoadingScreen.finishLoading += InitialValues;
    }
    private void OnDestroy()
    {
        LoadingScreen.finishLoading -= InitialValues;
    }
    private void InitialValues()
    {
        for (int i = 0; i < _textProblems.Length; i++)
        {
            _textProblems[i].gameObject.SetActive(false);
        }

        windowsControls.gameObject.SetActive(false);
        timerDelay = delayToQuit;
        inControls = false;
    }
    private void Update()
    {
        if (!inControls) return;

        delayToQuit -= Time.deltaTime;
        textTime.text = delayToQuit.ToString("f0");

        if (delayToQuit <= 0) OnRebindingCancelled();
    }
    public void StartRebind(string actionName)
    {
        if (inControls) return;

        string actionMapName = "Game";

        windowsControls.SetActive(true);
        delayToQuit = timerDelay;
        inControls = true;

        if (rebindingOperation != null) rebindingOperation.Cancel();

        var actionMap = _playerInput.actions.FindActionMap(actionMapName, true);
        if (actionMap == null) return;

        var action = actionMap.FindAction(actionName, true);
        if (action == null) return;

        rebindingOperation = action.PerformInteractiveRebinding()
            .OnComplete(callback => OnRebindingComplete(action))
            .OnCancel(callback => OnRebindingCancelled())
            .Start();
    }
    private void OnRebindingComplete(InputAction action)
    {
        if (action.bindings.Count == 0) return;
        string newBindingPath = action.bindings[action.bindings.Count - 1].effectivePath;

        InputAction conflictingAction = null;
        int conflictingBindingIndex = -1;

        foreach (var otherAction in action.actionMap.actions)
        {
            if (otherAction == action) continue;

            for (int i = 0; i < otherAction.bindings.Count; i++)
            {
                if (otherAction.bindings[i].effectivePath == newBindingPath)
                {
                    conflictingAction = otherAction;
                    conflictingBindingIndex = i;
                    break;
                }
            }

            if (conflictingAction != null) break;
        }

        if (Keyboard.current.anyKey.wasPressedThisFrame)
        {
            _playerInput.SwitchCurrentControlScheme("keyboard", Keyboard.current);
        }
        else if (Gamepad.current?.buttonSouth.wasPressedThisFrame == true)
        {
            _playerInput.SwitchCurrentControlScheme("gamepad", Gamepad.current);
        }

        if (conflictingAction != null && conflictingBindingIndex >= 0)
        {
            conflictingAction.ApplyBindingOverride(conflictingBindingIndex, "");
            GenerateConflict();
        }

        OnRebindingCancelled();
    }
    private void OnRebindingCancelled()
    {
        inControls = false;
        delayToQuit = timerDelay;
        windowsControls.SetActive(false);
    }
    private void GenerateConflict()
    {
        string[] listText = { "Run", "Dig", "Interact", "Jetpack", "Inventory" };

        for (int i = 0; i < _textProblems.Length; i++)
        {
            bool exists = CheckBindingExists("Game", listText[i], _playerInput.currentControlScheme);
            _textProblems[i].gameObject.SetActive(!exists);
        }
    }
    private bool CheckBindingExists(string mapName, string actionName, string scheme)
    {
        var actionMap = _playerInput.actions.FindActionMap(mapName, false);
        if (actionMap == null) return false;

        var action = actionMap.FindAction(actionName, false);
        if (action == null) return false;

        foreach (var binding in action.bindings)
        {
            if (!string.IsNullOrEmpty(binding.effectivePath) &&
                _playerInput.actions.controlSchemes.Any(s => s.name == scheme && binding.groups.Contains(s.name)))
            {
                return true;
            }
        }

        return false;
    }
}