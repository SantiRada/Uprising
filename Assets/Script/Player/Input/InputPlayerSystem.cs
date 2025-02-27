using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputPlayerSystem : MonoBehaviour {

    [HideInInspector] public Vector3 moveInput;
    [HideInInspector] public Vector3 moveCamera;

    [HideInInspector] public event Action interact;
    [HideInInspector] public event Action excavation;
    [HideInInspector] public event Action useInventory;
    [HideInInspector] public event Action useEscape;
    [HideInInspector] public event Action useJetpackEvent;
    [HideInInspector] public int useJetpack = 0; // 0 = NO SE USA // 1 = SE USA // 2 = SE DEJÓ DE USAR
    [HideInInspector] public int useRun = 0; // 0 = NO SE USA // 1 = SE USA

    [HideInInspector] public event Action useSelect;

    // VECTORES 2D
    public void OnMovement(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 movement = (Vector3)context.ReadValue<Vector2>();
            moveInput = new Vector3(movement.x, 0, movement.y);
        }
        if (context.canceled) moveInput = Vector3.zero;
    }
    public void OnMoveCamera(InputAction.CallbackContext context)
    {
        if (context.performed) moveCamera = (Vector3)context.ReadValue<Vector2>();
        if (context.canceled) moveCamera = Vector3.zero;
    }
    // EVENTS
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed) interact?.Invoke();
    }
    public void OnExcavation(InputAction.CallbackContext context)
    {
        if (context.performed) excavation?.Invoke();
    }
    public void OnJetpack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            useJetpackEvent?.Invoke();
            useJetpack = 1;
        }
        if (context.canceled) { useJetpack = 2; }
    }
    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.performed) useInventory?.Invoke();
    }
    public void OnEscape(InputAction.CallbackContext context)
    {
        if (context.performed) useEscape?.Invoke();
    }
    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.performed) useRun = 1;
        if (context.canceled) useRun = 0;
    }
}