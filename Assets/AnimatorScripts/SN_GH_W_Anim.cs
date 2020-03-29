using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SN_GH_W_Anim : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("isMovable", false);
        animator.SetBool("ThrowWPotion", false);

    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime >= 0.55f && stateInfo.normalizedTime <= 0.65f)
        {
            animator.SetBool("ThrowWPotion", true);
        }
        else
            animator.SetBool("ThrowWPotion", false);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("ThrowWPotion", false);

        animator.SetBool("isCasting", false);
        animator.SetBool("WSkill", false);
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
