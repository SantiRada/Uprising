using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputInterfaceSystem : MonoBehaviour {

    public event Action useBack;
    public event Action useSelect;
    public event Action useReset;
    public event Action useSave;

    public event Action usePrevious;
    public event Action useNext;
    public event Action useMoreInfo;
    public event Action changeScheme;

    private PlayerInput _input;
    private Coroutine _actionMapCoroutine;
    [HideInInspector] public Vector2 movement;
    [HideInInspector] public event Action useMove;

    private void Awake()
    {
        _input = FindAnyObjectByType<PlayerInput>();
    }
    public void ChangeControlScheme(bool isDisconnected)
    {
        if (isDisconnected)
            _input.SwitchCurrentControlScheme("Keyboard");
        else
            _input.SwitchCurrentControlScheme("Gamepad");
    }
    public void ChangeActionMap(bool isUI)
    {
        if (_actionMapCoroutine != null) StopCoroutine(_actionMapCoroutine);

        _actionMapCoroutine = StartCoroutine(ActionMap(isUI));
    }
    private IEnumerator ActionMap(bool isUI)
    {
        yield return new WaitForSeconds(0.18f);

        if (_input != null) _input.SwitchCurrentActionMap(isUI ? "Interface" : "Game");

        _actionMapCoroutine = null;
    }
    public void LaunchChangeScheme()
    {
        changeScheme?.Invoke();
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed) useMove?.Invoke();

        movement = context.ReadValue<Vector2>();
    }
    public void OnBack(InputAction.CallbackContext context)
    {
        if (context.performed) useBack?.Invoke();
    }
    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (context.performed) useSelect?.Invoke();
    }
    public void OnReset(InputAction.CallbackContext context)
    {
        if (context.performed) useReset?.Invoke();
    }
    public void OnSave(InputAction.CallbackContext context)
    {
        if(context.performed) useSave?.Invoke();
    }
    public void OnPrevious(InputAction.CallbackContext context)
    {
        if (context.performed) usePrevious?.Invoke();
    }
    public void OnNext(InputAction.CallbackContext context)
    {
        if(context.performed) useNext?.Invoke();
    }
    public void OnMoreInfo(InputAction.CallbackContext context)
    {
        if (context.performed) useMoreInfo?.Invoke();
    }
}