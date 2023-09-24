using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/AttackData", fileName = "SO_AttackData")]
public class SO_AttackData : ScriptableObject
{
    [Header("Data"), Space] 
    public AnimatorOverrideController animatorOverrideController;
    public MoveInfo moveInfo;
    //We can add other custom data to this part later
}
