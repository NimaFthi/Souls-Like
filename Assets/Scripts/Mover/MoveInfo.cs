using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class MoveInfo
{
    [Header("Info"), Space] 
    [SerializeField] private AnimationCurve moveCurve;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float staminaCost = 0f;
    [SerializeField] private  float minStaminaToPerform = 0f;

    #region Prperties

    public float StaminaCost => staminaCost;
    public float MinStaminaToPerform => minStaminaToPerform;

    #endregion

    public IEnumerator MoveRoutine(CharacterController characterController, Vector3 moveVector, Vector3 lookVector)
    {
        var moveTime = moveCurve[moveCurve.length - 1].time;
        
        var timer = 0f;
        while (timer < moveTime)
        {
            var curveValue = moveCurve.Evaluate(timer);
            var rollingVelocity = moveVector * curveValue * moveSpeed;
            
            characterController.Move(rollingVelocity * Time.deltaTime);
            characterController.transform.rotation = Quaternion.LookRotation(lookVector);
            
            timer += Time.deltaTime;
            yield return null;
        }
    }
}
