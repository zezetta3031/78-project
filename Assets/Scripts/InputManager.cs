using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    public static PlayerInput PlayerInput;

    public static Vector2 Movement;
    public static bool JumpWasPressed;
    public static bool JumpIsHeld;
    public static bool JumpWasReleased;
    public static bool RunIsHeld;
    public static bool slowTimeAction;

    private InputAction timestop;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction runAction;
    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();

        timestop = PlayerInput.actions["Time Stop"];
        moveAction = PlayerInput.actions["Move"];
        jumpAction = PlayerInput.actions["Jump"];
        runAction = PlayerInput.actions["Run"];
        
    }
    private void Update()
    {
        Movement = moveAction.ReadValue<Vector2>();

        slowTimeAction = timestop.WasPressedThisFrame();
        JumpWasPressed = jumpAction.WasPressedThisFrame();
        JumpIsHeld = jumpAction.IsPressed();
        JumpWasReleased = jumpAction.WasReleasedThisFrame();

        RunIsHeld = runAction.IsPressed();
    }
}
