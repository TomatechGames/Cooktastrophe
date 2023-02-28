using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavmeshAgentTarget : MonoBehaviour
{

    [SerializeField]
    NavMeshAgent agent;

    private void Start()
    {
        UpdateTarget();
    }

    [ContextMenu("Update Target")]
    private void UpdateTarget()
    {
        if(agent)
            agent.destination = transform.position;
    }
}
