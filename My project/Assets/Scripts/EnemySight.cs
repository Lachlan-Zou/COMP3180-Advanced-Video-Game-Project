using UnityEngine;

public class EnemySight : MonoBehaviour
{
    public GameObject target;
    public float viewDistance = 10f;
    [Range(0f, 360f)]
    public float viewAngle = 90f;
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
        return true;
    }
}
