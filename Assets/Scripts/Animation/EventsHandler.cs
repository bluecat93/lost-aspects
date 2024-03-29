using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Animation
{
    public class EventsHandler : MonoBehaviour
    {
        [HideInInspector] public UnityEvent<bool> OnAttackEventTrigger;

        public void AttackStarted()
        {
            //Debug.Log("ATTACK STARTED");
            OnAttackEventTrigger.Invoke(true);
        }
        public void AttackEnded()
        {
            //Debug.Log("ATTACK ENDED");
            OnAttackEventTrigger.Invoke(false);
        }
    }
}