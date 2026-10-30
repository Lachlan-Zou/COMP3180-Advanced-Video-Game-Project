using UnityEngine;

public class CanvasInstance : MonoBehaviour
{
    public static CanvasInstance Instance { get; private set; }
    void Awake()
    {
        // If an instance already exists and it's not this one, destroy this copy
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return; // Exit early so the rest of Awake doesn't run
        }

        // Set this object as the definitive instance and protect it across scenes
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
