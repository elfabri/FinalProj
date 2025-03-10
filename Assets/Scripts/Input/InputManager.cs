using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static PlayerInput PlayerInput;
    public static Vector2 Movement;

    public static bool JumpWasPressed;
    public static bool JumpIsHeld;
    public static bool JumpWasReleased;

    public static bool UiSubmitPressed;
    public static bool UiCancelPressed;

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _uiSubmit;
    private InputAction _uiCancel;

    void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();

        _moveAction = PlayerInput.actions["Move"];
        _jumpAction = PlayerInput.actions["Jump"];

        _uiSubmit = PlayerInput.actions["Submit"];
        _uiCancel = PlayerInput.actions["Cancel"];
    }

    void Update()
    {
        Movement = _moveAction.ReadValue<Vector2>();

        JumpWasPressed = _jumpAction.WasPressedThisFrame();
        JumpIsHeld = _jumpAction.IsPressed();
        JumpWasReleased = _jumpAction.WasReleasedThisFrame();

        UiSubmitPressed = _uiSubmit.WasPressedThisFrame();
        UiCancelPressed = _uiCancel.WasPressedThisFrame();
    }
}
