using System;
using System.Collections;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Header("Components"), Space] 
    private Inputs _inputs;

    [Header("Sprint And Dodge"), Space] 
    [SerializeField] private float holdTimeToSprint = 0.5f;
    private Coroutine _currentSprintAndDodgeCoroutine;
    private bool _isCoroutineRunning = false;

    #region Events

    public Action<bool> OnSprintingInputChange;

    public Action OnDodgeInput;

    public Action OnAttackInput;

    public Action OnLockInput;

    #endregion

    #region Properties

    public Vector2 MoveInput { get; private set; }
    public bool IsSprintingInput { get; private set; }

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
        
        _inputs.Player.SprintAndDodge.started += context => HandleSprintAndDodgeInput(true);
        _inputs.Player.SprintAndDodge.canceled += context => HandleSprintAndDodgeInput(false);

        _inputs.Player.Attack.performed += context => HandleAttackInput();

        _inputs.Player.LockOn.performed += context => HandleLockInput();
    }

    private void HandleMoveInput(Vector2 rawInput)
    {
        MoveInput = rawInput;
    }

    private void HandleSprintAndDodgeInput(bool state)
    {
        if (state)
        {
            _currentSprintAndDodgeCoroutine = StartCoroutine(HandleSprintAndDodgeCoroutine());
            return;
        }

        if (_isCoroutineRunning)
        {
            StopCoroutine(_currentSprintAndDodgeCoroutine);
            OnDodgeInput?.Invoke();
            _isCoroutineRunning = false;
        }
        else
        {
            IsSprintingInput = false;
            OnSprintingInputChange?.Invoke(false);
        }
    }

    private IEnumerator HandleSprintAndDodgeCoroutine()
    {
        _isCoroutineRunning = true;
        var timer = 0f;

        while (timer < holdTimeToSprint)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        IsSprintingInput = true;
        OnSprintingInputChange?.Invoke(true);
        _isCoroutineRunning = false;
    }

    public void CancelSprintInput()
    {
        IsSprintingInput = false;
    }

    private void HandleAttackInput()
    {
        OnAttackInput?.Invoke();
    }
    
    private void HandleLockInput()
    {
        OnLockInput?.Invoke();
    }
}