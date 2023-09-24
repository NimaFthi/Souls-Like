using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Components"), Space] 
    [SerializeField] private PlayerData playerData;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform cameraTransform;

    [Header("Movement Setting"), Space] 
    [SerializeField] private float normalMoveSpeed;
    [SerializeField] private float sprintMoveSpeed;
    private Vector3 _moveInput;
    private Vector3 _moveVector;

    [Header("Gravity Setting"), Space]
    [SerializeField] private float gravityAmount = -9.81f;
    [SerializeField] private float maxFallSpeed = -20f;
    private float gravitySpeed = 0f;

    [Header("RotationSetting"), Space] 
    [SerializeField] private float rotationLerpFactor = 0.5f;

    [Header("MoveInfos"), Space] 
    [SerializeField] private MoveInfo rollMoveInfo;
    [SerializeField] private MoveInfo backStepMoveInfo;

    private void OnEnable()
    {
        playerData.inputManager.OnDodgeInput += HandleDodge;
    }

    private void OnDisable()
    {
        playerData.inputManager.OnDodgeInput -= HandleDodge;
    }

    private void Update()
    {
        HandleMovement();
        HandleGravity();
        HandleRotation();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, _moveVector * 2);
    }

    #region Movement

    private void HandleMovement()
    {
        CalculateMoveInput();

        if (playerData.playerAnimationHandler.IsInteracting || playerData.playerAttackHandler.IsBetweenCombo)
        {
            _moveVector = Vector3.zero;
            return;
        }

        var motionSpeed = playerData.inputManager.IsSprinting ? sprintMoveSpeed : normalMoveSpeed;
        _moveVector = _moveInput * motionSpeed;
        _moveVector.y = 0f;

        characterController.Move(_moveVector * Time.deltaTime);
    }

    private void HandleGravity()
    {
        if (characterController.isGrounded)
        {
            gravitySpeed = 0f;
            return;
        }

        
        gravitySpeed += gravityAmount;
        if (gravitySpeed <= maxFallSpeed)
        {
            gravitySpeed= maxFallSpeed;
        }

        characterController.Move(Vector3.up * gravitySpeed * Time.deltaTime);
    }

    private void CalculateMoveInput()
    {
        var input = playerData.inputManager.MoveInput;
        var cameraForward = Quaternion.AngleAxis(-cameraTransform.eulerAngles.x, cameraTransform.right) * cameraTransform.forward;

        _moveInput = cameraTransform.right * input.x + cameraForward * input.y;
        _moveInput.Normalize();
        _moveInput.y = 0;
    }

    public bool IsMoving()
    {
        var surfaceVelocity = new Vector2(_moveVector.x,_moveVector.z);
        return surfaceVelocity.normalized.sqrMagnitude > 0;
    }

    #endregion

    #region Rotation

    private void HandleRotation()
    {
        if(playerData.playerAnimationHandler.IsInteracting || playerData.playerAttackHandler.IsBetweenCombo) return;
        if (!characterController.isGrounded) return;
        if (_moveInput.magnitude == 0) return;
        
        var targetRot = Quaternion.LookRotation(_moveInput);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationLerpFactor);
    }

    #endregion

    #region Dodge

    private void HandleDodge()
    {
        if (playerData.playerAnimationHandler.IsInteracting || playerData.playerAttackHandler.IsBetweenCombo) return;
        
        if (IsMoving())
        {
            Roll();
        }
        else
        {
            // BackStep();
        }
    }

    private void Roll()
    {
        MoveWithAnimation("Roll", true, rollMoveInfo, transform.forward, transform.forward);
    }

    private void BackStep()
    {
        MoveWithAnimation("BackStep", true, backStepMoveInfo, -transform.forward, transform.forward);
    }
    
    private void MoveWithAnimation(string animationName, bool isInteracting, MoveInfo moveInfo, Vector3 moveVector, Vector3 lookVector)
    {
        playerData.playerAnimationHandler.PlayAnimation(animationName, isInteracting, true);
        StartCoroutine(moveInfo.MoveRoutine(characterController, moveVector, lookVector));
    }

    #endregion

    #region Attack

    public void HandleAttackMovement(MoveInfo moveInfo)
    {
        var moveVector = _moveInput.magnitude > 0 ? _moveInput : transform.forward;
        StartCoroutine(moveInfo.MoveRoutine(characterController, moveVector, moveVector));
    }

    #endregion
}