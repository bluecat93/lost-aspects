using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animation
{
    public class Functions : MonoBehaviour
    {
        public static bool AnimatorIsPlaying(Animator animator)
        {
            return animator.GetCurrentAnimatorStateInfo(0).length >
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        }

        public static bool AnimatorIsPlaying(Animator animator, string stateName)
        {
            return AnimatorIsPlaying(animator) && animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
        }

        public static bool AnimatorIsPlaying(Animator animator, int stateNameHash)
        {
            Debug.Log(animator.GetCurrentAnimatorStateInfo(0).GetHashCode() == stateNameHash);
            return AnimatorIsPlaying(animator) && animator.GetCurrentAnimatorStateInfo(0).GetHashCode() == stateNameHash;
        }

        public static void SetAnimatorController(Animator animator, RuntimeAnimatorController newAnimatorController)
        {
            animator.runtimeAnimatorController = newAnimatorController as RuntimeAnimatorController;
        }
    }
}