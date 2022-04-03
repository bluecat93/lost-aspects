using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animation
{
    public class Functions : MonoBehaviour
    {
        public bool AnimatorIsPlaying(Animator animator)
        {
            return animator.GetCurrentAnimatorStateInfo(0).length >
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        }

        public bool AnimatorIsPlaying(Animator animator, string stateName)
        {
            return AnimatorIsPlaying(animator) && animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
        }
    }
}

