using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    [Header("Components"), Space] 
    [SerializeField] private PlayerData playerData;
    [SerializeField] private Animator animator;
    private AnimatorOverrideController defaultAnimatorOverrideController;

    [Header("Motion Setting"), Space] 
    [SerializeField] [Range(0f, 1f)] private float motionTransitionTime = 0.5f;

    [Header("Animator Params"), Space] 
    private readonly int _motionBlendFactor = Animator.StringToHash("MotionBlendParameter");
    private readonly int _isInteracting = Animator.StringToHash("IsInteracting");

    [Header("Default Setting"), Space] 
    [SerializeField] private float TransitionTime = 0.2f;

    #region Properties

    public bool IsInteracting => animator.GetBool(_isInteracting);

    public bool CanAttack
    {
        get => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.75f && animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack");
    }

    #endregion

    private void Start()
    {
        defaultAnimatorOverrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
    }

    private void Update()
    {
        HandleMoveAnimation();
    }


    private void HandleMoveAnimation()
    {
        var motionFactor = playerData.playerController.IsMoving() ? 1f : 0f;
        var sprintFactor = playerData.inputManager.IsSprintingInput && playerData.playerStatsHandler.canUseStamina ? 2f : 1f;

        var value = motionFactor * sprintFactor;
        animator.SetFloat(_motionBlendFactor, value, motionTransitionTime, Time.deltaTime);
    }

    #region Public Methods

    public void PlayAnimation(string animationName, bool isInteracting , bool crossFade)
    {
        animator.SetBool(_isInteracting, isInteracting);

        if (crossFade)
        {
            animator.CrossFade(animationName, TransitionTime);
        }
        else
        {
            animator.Play(animationName, 0 , TransitionTime);
        }
    }

    public void ChangeAnimatorOverrideController(AnimatorOverrideController animatorOverrideController)
    {
        animator.runtimeAnimatorController = animatorOverrideController;
    }

    public void ResetAnimatorOverrideController()
    {
        animator.runtimeAnimatorController = defaultAnimatorOverrideController;
    }

    #endregion
}