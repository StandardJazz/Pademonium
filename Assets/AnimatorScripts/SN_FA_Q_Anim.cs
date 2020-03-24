using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SN_FA_Q_Anim : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("isMovable", false);
    }
    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (0.55f < stateInfo.normalizedTime && stateInfo.normalizedTime < 0.75f)
            animator.SetBool("QSkill", true);
        else
            animator.SetBool("QSkill", false);

        if(stateInfo.normalizedTime > 0.80f)
            animator.SetBool("isMovable", true);
    }
    
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("isCasting", false);
        animator.SetBool("QSkill", false);
        animator.SetBool("isMovable", true);
    }
}
