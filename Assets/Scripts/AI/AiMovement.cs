using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiMovement : MonoBehaviour
{
    private Vector3 currentObjective;
    private NavMeshAgent agent;

    // Start is called before the first frame update
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
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
        agent.SetDestination(currentObjective);
    }

    public void StopMoving()
    {
        MoveTo(transform.position);
    }

    public bool ReachedPosition()
    {
        return Vector3.Distance(transform.position, currentObjective) < 1f;
    }
}
