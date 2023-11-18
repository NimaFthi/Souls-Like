using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackHandler : MonoBehaviour
{
    [Header("Components"), Space]
    [SerializeField] private PlayerData playerData;
    [SerializeField] private Hitter hitter;

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
        hitter.OnHit += OnHittingEnemy;
    }

    private void OnDisable()
    {
        playerData.inputManager.OnAttackInput -= HandleAttack;
        hitter.OnHit -= OnHittingEnemy;
    }

    #region Attack & Combo

    private void HandleAttack()
    {
        var playerAnimationHandler = playerData.playerAnimationHandler;
        
        if(playerAnimationHandler.IsInteracting) return;
        if(!playerData.playerStatsHandler.canUseStamina) return;
        
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
        DoAttack();
        
        
        var timer = 0f;
        while (timer < continueComboWindow)
        {
            if (_continueCombo && playerData.playerAnimationHandler.CanAttack)
            {
                DoAttack();

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

    private void DoAttack()
    {
        PlayComboAnimation(_comboIndex);
        HandleComboMovement(_comboIndex);
        HandleComboStamina(_comboIndex);
        HandleComboIndex();
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

    private void HandleComboStamina(int index)
    {
        playerData.playerStatsHandler.UseStamina(comboData[index].staminaCost);
    }

    #endregion

    #region Hit

    private void OnHittingEnemy(GameObject hitGO)
    {
        //temp code
        Debug.Log(hitGO.name);
        var meshRenderer = hitGO.GetComponent<MeshRenderer>();
        meshRenderer.material.color = Random.ColorHSV();
    }

    //Animation Event
    public void ActivateHitterColliders()
    {
        hitter.ChangeColliderState(true);
    }

    //Animation Event
    public void DeActivateHitterColliders()
    {
        hitter.ChangeColliderState(false);
    }

    #endregion
}
