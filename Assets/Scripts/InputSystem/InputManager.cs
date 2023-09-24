using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Header("Components"), Space]
    private Inputs _inputs;

    #region Events

    public Action OnDodgeInput;

    public Action OnAttackInput;

    #endregion

    #region Properties

    public Vector2 MoveInput { get; private set; }
    public bool IsSprinting { get; private set; }

    #endregion


    private void OnEnable()
    {
        InitializeInputs();

        _inputs.Enable();
    }

    private void OnDisable()
    {
        _inputs.Disable();
    }

    private void InitializeInputs()
    {
        if (_inputs != null) return;
        
        _inputs = new Inputs();
        
        _inputs.Player.Walk.performed += context => HandleMoveInput(context.ReadValue<Vector2>());
        
        _inputs.Player.Sprint.performed += context => HandleSprintInput(true);
        _inputs.Player.Sprint.canceled += context => HandleSprintInput(false);

        _inputs.Player.Roll.performed += context => HandleDodgeInput();

        _inputs.Player.Attack.performed += context => HandleAttackInput();
    }

    private void HandleMoveInput(Vector2 rawInput)
    {
        MoveInput = rawInput;
    }

    private void HandleSprintInput(bool state)
    {
        IsSprinting = state;
    }

    private void HandleDodgeInput()
    {
        OnDodgeInput?.Invoke();
    }

    private void HandleAttackInput()
    {
        OnAttackInput?.Invoke();
    }
}