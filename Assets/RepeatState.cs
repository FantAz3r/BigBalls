using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatState : StateMachineBehaviour
{
    [SerializeField] private int _repeatTime = 2;

    public override void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_repeatTime <= 0)
            return;

        _repeatTime--;
        animator.Play(stateInfo.fullPathHash);
    }
}
