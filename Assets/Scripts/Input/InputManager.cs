using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private PlayerInputAction playerInputAction;
    public static InputManager Instance;

    public event EventHandler OnJumpAction;
    public event EventHandler OnCrouchAction;
    public event EventHandler OnCrouchCanceled;
    public event EventHandler OnSprintAction;
    public event EventHandler OnSprintCanceled;
    public event EventHandler OnInteractAction;
    public event EventHandler OnPauseAction;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        playerInputAction = new PlayerInputAction();
        playerInputAction.Player.Enable();

        playerInputAction.Player.Jump.performed += Jump_performed;
        playerInputAction.Player.Crouch.performed += Crouch_performed;
        playerInputAction.Player.Crouch.canceled += Crouch_canceled;
        playerInputAction.Player.Sprint.performed += Sprint_performed;
        playerInputAction.Player.Sprint.canceled += Sprint_canceled;
        playerInputAction.Player.Interact.started += Interact_started;
        playerInputAction.Player.Pause.started += Pause_started;
    }
    private void OnDestroy()
    {
        playerInputAction.Player.Jump.performed -= Jump_performed;
        playerInputAction.Player.Crouch.performed -= Crouch_performed;
        playerInputAction.Player.Crouch.canceled -= Crouch_canceled;
        playerInputAction.Player.Sprint.performed -= Sprint_performed;
        playerInputAction.Player.Sprint.canceled -= Sprint_canceled;
        playerInputAction.Player.Interact.started -= Interact_started;
        playerInputAction.Player.Pause.started -= Pause_started;
        playerInputAction.Dispose();
    }

    private void Pause_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    private void Sprint_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnSprintCanceled?.Invoke(this, EventArgs.Empty);
    }

    private void Sprint_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnSprintAction?.Invoke(this, EventArgs.Empty);
    }

    public void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnJumpAction?.Invoke(this, EventArgs.Empty);
    }

    private void Crouch_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnCrouchCanceled?.Invoke(this, EventArgs.Empty);
    }

    private void Crouch_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnCrouchAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMouseValue()
    {
        Vector2 mouseVector = playerInputAction.Player.Mouse.ReadValue<Vector2>();
        return mouseVector;
    }
    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = playerInputAction.Player.Movement.ReadValue<Vector2>();
        inputVector = inputVector.normalized;
        return inputVector;
    }
}
