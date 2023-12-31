using System;
using System.Collections.Generic;
using UnityEngine;

public class HandleParametersAnimationBehaviour : StateMachineBehaviour
{
    public List<ParametersToChange> parametersToChangeOnStart;
    public List<ParametersToChange> parametersToChangeOnEnd;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (var parameter in parametersToChangeOnStart)
        {
            animator.SetBool(parameter.parameterName,parameter.parameterNewState);
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (var parameter in parametersToChangeOnEnd)
        {
            animator.SetBool(parameter.parameterName,parameter.parameterNewState);
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}

    [Serializable]
    public class ParametersToChange
    {
        public string parameterName;
        public bool parameterNewState;
    }
}


