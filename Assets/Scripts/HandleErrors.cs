using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeadsUpDisplay;
using UnityEngine.SceneManagement;
using Mirror;

public class HandleErrors : MonoBehaviour
{
    public static void NetworkAnimatorSetTrigger(NetworkAnimator anim, string triggerName)
    {
        // TODO figure out how to disable this error
        anim.SetTrigger("triggerName");
    }
}