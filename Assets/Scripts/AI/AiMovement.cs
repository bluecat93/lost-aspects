using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class AiMovement : MonoBehaviour
{
    private Vector3 currentObjective;

    private NavMeshAgent _navMeshAgent;

    private NavMeshAgent NvMshAgnt
    {
        get
        {
            if (this._navMeshAgent == null)
                this._navMeshAgent = GetComponent<NavMeshAgent>();

            return this._navMeshAgent;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ReachedPosition())
        {
            StopMoving();
        }
    }

    public void MoveTo(Vector3 currentObjective)
    {
        this.currentObjective = currentObjective;
        this.NvMshAgnt.SetDestination(currentObjective);
    }

    public void StopMoving()
    {
        MoveTo(transform.position);
    }

    public bool ReachedPosition()
    {
        return Vector3.Distance(transform.position, this.currentObjective) < 1f;
    }
}

