using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackHandler : MonoBehaviour
{
    [Header("Components"), Space] 
    [SerializeField] private PlayerData playerData;

    [Header("ComboSystem"), Space] 
    [SerializeField] private List<SO_AttackData> comboData;
    [SerializeField] private float continueComboWindow = 1f;
    private int _comboIndex;
    private bool _isBetweenCombo;
    private bool _continueCombo;
    private Coroutine currentAttackRoutine;

    #region Properties

    public bool IsBetweenCombo => _isBetweenCombo;

    #endregion

    private void OnEnable()
    {
        playerData.inputManager.OnAttackInput += HandleAttack;
    }

    private void OnDisable()
    {
        playerData.inputManager.OnAttackInput -= HandleAttack;
    }

    private void HandleAttack()
    {
        var playerAnimationHandler = playerData.playerAnimationHandler;
        
        if(playerAnimationHandler.IsInteracting) return;
        
        if (_isBetweenCombo)
        {
            _continueCombo = true;
        }
        else
        {
            if (currentAttackRoutine != null)
            {
                StopCoroutine(currentAttackRoutine);
            }
            
            currentAttackRoutine = StartCoroutine(AttackRoutine());
            _isBetweenCombo = true;
        }
    }

    private IEnumerator AttackRoutine()
    {
        PlayComboAnimation(_comboIndex);
        HandleComboMovement(_comboIndex);
        HandleComboIndex();
        
        var timer = 0f;
        while (timer < continueComboWindow)
        {
            if (_continueCombo && playerData.playerAnimationHandler.CanAttack)
            {
                PlayComboAnimation(_comboIndex);
                HandleComboMovement(_comboIndex);
                HandleComboIndex();

                _continueCombo = false;
                timer = 0f;
            }
            timer += Time.deltaTime;
            yield return null;
        }
        
        _comboIndex = 0;
        _isBetweenCombo = false;
        _continueCombo = false;
    }

    private void HandleComboIndex()
    {
        _comboIndex++;
        if (_comboIndex >= comboData.Count)
        {
            _comboIndex = 0;
        }
    }

    private void PlayComboAnimation(int index)
    {
        var playerAnimationHandler = playerData.playerAnimationHandler;

        playerAnimationHandler.ChangeAnimatorOverrideController(comboData[index].animatorOverrideController);
        playerAnimationHandler.PlayAnimation("Attack",false, false);
    }

    private void HandleComboMovement(int index)
    {
        playerData.playerController.HandleAttackMovement(comboData[index].moveInfo);
    }
}
