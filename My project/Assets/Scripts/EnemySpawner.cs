using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instantiate(enemy, new Vector3(0,1,0), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
