using UnityEngine;

public class EnemySight : MonoBehaviour
{
    public Material coneMaterial;
    public GameObject target;
    public float viewDistance = 10f;
    [Range(0f, 360f)]
    public float viewAngle = 90f;
    public float radius;
    public LayerMask targetMask;
    public LayerMask wallMask;

    public bool canSeePlayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    public void CanSeeTarget()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, wallMask))
                    canSeePlayer = true;
                else
                    canSeePlayer = false;
            }
            else
                canSeePlayer = false;
        }
        else if (canSeePlayer)
            canSeePlayer = false;

        //Debug.Log($"rangeChecks is {(rangeChecks == null ? "null" : "not null")}");
        //Debug.Log($"rangeChecks length: {rangeChecks?.Length}");
    }

    //if (target == null) 
    //{
    //    return false;
    //}

    //Vector3 direction = target.transform.position - transform.position;
    //if (direction.sqrMagnitude > viewDistance * viewDistance)
    //{
    //    return false;
    //}

    //if (Physics.Raycast(transform.position + Vector3.up, direction.normalized, out RaycastHit hit, viewDistance))
    //{
    //    return hit.transform == target.transform || hit.transform.IsChildOf(target.transform);
    //}
    //return false;

}
