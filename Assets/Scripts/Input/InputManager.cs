using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static PlayerInput PlayerInput;
    public static Vector2 Movement;

    public static bool JumpWasPressed;
    public static bool JumpIsHeld;
    public static bool JumpWasReleased;

    public static bool Attack_1_WasPressed;
    public static bool Attack_1_IsHeld;
    public static bool Attack_1_WasReleased;

    public static bool UiSubmitPressed;
    public static bool UiCancelPressed;

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _attack_1_Action;
    private InputAction _uiSubmit;
    private InputAction _uiCancel;

    void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();

        _moveAction = PlayerInput.actions["Move"];
        _jumpAction = PlayerInput.actions["Jump"];
        _attack_1_Action = PlayerInput.actions["Attack1"];

        _uiSubmit = PlayerInput.actions["Submit"];
        _uiCancel = PlayerInput.actions["Cancel"];
    }

    void Update()
    {
        Movement = _moveAction.ReadValue<Vector2>();

        JumpWasPressed = _jumpAction.WasPressedThisFrame();
        JumpIsHeld = _jumpAction.IsPressed();
        JumpWasReleased = _jumpAction.WasReleasedThisFrame();

        Attack_1_WasPressed = _attack_1_Action.WasPressedThisFrame();
        Attack_1_IsHeld = _attack_1_Action.IsPressed();
        Attack_1_WasReleased = _attack_1_Action.WasReleasedThisFrame();

        UiSubmitPressed = _uiSubmit.WasPressedThisFrame();
        UiCancelPressed = _uiCancel.WasPressedThisFrame();
    }
}
