using UnityEngine;

public class MainCameraCentre : MonoBehaviour
{
    public Transform target;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void RepositionCamera()
    {
        transform.position = new Vector3(target.position.x, 5f, target.position.z);
        Debug.Log(target.position);
    }
}
