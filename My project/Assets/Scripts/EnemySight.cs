using UnityEngine;

public class EnemySight : MonoBehaviour
{
    public Material coneMaterial;
    public GameObject target;
    public float viewDistance = 10f;
    [Range(0f, 360f)]
    public float viewAngle = 90f;
    public LayerMask targetMask;
    public LayerMask wallMask;   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    public bool CanSeeTarget()
    {
        if (target == null) 
        {
            return false;
        }

        Vector3 direction = target.transform.position - transform.position;
        if (direction.sqrMagnitude > viewDistance * viewDistance)
        {
            return false;
        }

        if (Physics.Raycast(transform.position + Vector3.up, direction.normalized, out RaycastHit hit, viewDistance))
        {
            return hit.transform == target.transform || hit.transform.IsChildOf(target.transform);
        }
        return false;
    }
}
