using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [Header("Components"), Space] 
    public InputManager inputManager;
    public PlayerController playerController;
    public PlayerAttackHandler playerAttackHandler;
    public PlayerAnimationHandler playerAnimationHandler;
}
