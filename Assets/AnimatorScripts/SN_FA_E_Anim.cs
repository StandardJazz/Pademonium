using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SN_FA_E_Anim : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("isMovable", false);

    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (0.45f < stateInfo.normalizedTime && stateInfo.normalizedTime < 0.55f)
            animator.SetBool("ESkill", true);
        else
            animator.SetBool("ESkill", false);

        if (stateInfo.normalizedTime > 0.8f)
            animator.SetBool("isMovable", true);


    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("isCasting", false);
        animator.SetBool("ESkill", false);
        animator.SetBool("isMovable", true);

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
}
