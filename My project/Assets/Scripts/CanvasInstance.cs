using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;

public class CanvasInstance : MonoBehaviour
{
    public static CanvasInstance Instance { get; private set; }
    public MySceneManager mySceneManager;
    public static bool Paused { get; private set; } = false;
    public TextMeshProUGUI pausedText;
    public TextMeshProUGUI timerText;
    private InputSystem_Actions inputActions;
    private InputAction pauseAction;
    public float currentTime;
    private void OnEnable()
    {
        pauseAction.Enable();
    }
    private void OnDisable()
    {
        pauseAction.Disable();
    }
    void Awake()
    {
        inputActions = new InputSystem_Actions();
        pauseAction = inputActions.UI.Pause;

        // If an instance already exists and it's not this one, destroy this copy
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return; // Exit early so the rest of Awake doesn't run
        }

        // Set this object as the definitive instance and protect it across scenes
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (!Paused)
        {
            pausedText.enabled = false;
        }
    }
    void Start()
    {
        currentTime = 0;
    }
    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        TimeSpan time = TimeSpan.FromSeconds(currentTime);
        timerText.text = time.Minutes.ToString() + ":" + time.Seconds.ToString();

        pausedText.enabled = Paused;
        if (pauseAction.WasPressedThisFrame())
        {
            if (!Paused)
            {
                Pause();   
            } else
            {
                Resume();
            }
        } 
    }
    void Pause()
    {
        Paused = true;
        Time.timeScale = 0f;
    }
    void Resume()
    {
        Paused = false;
        Time.timeScale = 1f;
    }
}
