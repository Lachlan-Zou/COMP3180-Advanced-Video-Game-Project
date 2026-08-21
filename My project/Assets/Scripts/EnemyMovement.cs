using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Transform target;
    private NavMeshAgent agent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the NavMeshAgent component attached to this GameObject
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (target != null)
        {
            // Tell the agent to calculate a path and move to the target position
            agent.SetDestination(target.position);

                //     Debug.Log($"On NavMesh: {agent.isOnNavMesh}, " +
                //   $"Has Path: {agent.hasPath}, " +
                //   $"Status: {agent.pathStatus}");
        }
    }
}
