using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Required for managing scenes
using TMPro;

public class MySceneManager : MonoBehaviour
{
    public static MySceneManager Instance { get; private set; }
    public int mazeDepth = 5;
    public int mazeWidth = 5;
    public TextMeshProUGUI depthValue;
    public TextMeshProUGUI widthValue;
    public MazeGenerator mazeGenerator;
    public GameObject UICanvas;
    public CanvasInstance canvasInstance;
    public List<string> recordedTimes = new List<string>();
    public int started = 0;
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
    void Start()
    {
        GameObject mazeObject = GameObject.Find("MazeGenerator");

        if (mazeObject != null)
        {
            mazeGenerator = mazeObject.GetComponent<MazeGenerator>();
        }
        mazeGenerator._mazeDepth = mazeDepth;
        mazeGenerator._mazeWidth = mazeWidth;

        UICanvas = GameObject.Find("Canvas");
        canvasInstance = UICanvas.GetComponent<CanvasInstance>();
        //Debug.Log(mazeDepth);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject mazeObject = GameObject.Find("Maze Generator");

        if (mazeObject != null)
        {
            mazeGenerator = mazeObject.GetComponent<MazeGenerator>();

            mazeGenerator._mazeDepth = mazeDepth;
            mazeGenerator._mazeWidth = mazeWidth;

            //Debug.Log("Maze dimensions: " + mazeDepth + " x " + mazeWidth);
        }
        else
        {
            Debug.LogWarning("Couldn't find MazeGenerator!");
        }

        UICanvas = GameObject.Find("Canvas");
        canvasInstance.currentTime = 0f;
        Time.timeScale = 1f;
        //started = 1;
    }
    public void StartGame()
    {
        PlayerPrefs.SetInt("HasStarted", 1);
    }
    public void OnDepthChange(float value)
    {
        mazeDepth = (int)value;
        depthValue.text = value.ToString();
    }
    public void OnWidthChange(float value)
    {
        mazeWidth = (int)value;
        widthValue.text = value.ToString();
    }
    // Public method that will be linked to the UI Button
    public void ReloadScene()
    {
        // Gets the current active scene and reloads it using its build index
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitApplication()
    {
        Time.timeScale = 0f;
    }
}
