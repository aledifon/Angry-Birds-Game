using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get 
        { 
            if (instance == null)
            {
                instance = FindAnyObjectByType<GameManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    instance = go.AddComponent<GameManager>();
                }                
            }   
            return instance; 
        }
    }

    // Ref. to the Player Refs.
    public GameDependencies Dependencies { get; private set; }
    public Bird Player { get; private set; }
    // Audio
    AudioSource gameAudioSource;
    [SerializeField] AudioClip startLevelClip;
    [SerializeField] AudioClip winLevelClip;
    [SerializeField] AudioClip failLevelClip;

    [SerializeField] AudioClip mainTitleAudioClip;

    // UI Refs.
    private GameObject canvas;
    private GameObject titlePanel;

    #region Enums
    public enum Scenes { Menu, Level1, Level2, Level3 }
    private Scenes sceneSelected = Scenes.Menu;
    #endregion

    #region Unity API
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;  // Subscribe to the event.
        }            
        else
            Destroy(gameObject);

        gameAudioSource = GetComponent<AudioSource>();        
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {        
        if (System.Enum.TryParse(SceneManager.GetActiveScene().name, out Scenes currentScene))
        {
            switch (currentScene)
            {
                case Scenes.Menu:
                    // Set the new Scene as the current one
                    sceneSelected = Scenes.Menu;

                    // Get the corresponding Canvas's Refs
                    if(!GetCanvasRefs())
                        return;

                    // Start playing Title Screen Audio
                    PlayMainTitleAudioClip();

                    // Set the Canvas Panels Initial State
                    SetCanvasPanelsState();                    
                    break;
                case Scenes.Level1:
                case Scenes.Level2:
                case Scenes.Level3:

                    // Set the new Scene as the current one
                    if (SceneManager.GetActiveScene().name == Scenes.Level1.ToString())
                        sceneSelected = Scenes.Level1;
                    if (SceneManager.GetActiveScene().name == Scenes.Level2.ToString())
                        sceneSelected = Scenes.Level2;
                    if (SceneManager.GetActiveScene().name == Scenes.Level3.ToString())
                        sceneSelected = Scenes.Level3;

                    // Get the corresponding Canvas's Refs
                    if (!GetCanvasRefs())
                        return;

                    // Stop Intro Song
                    gameAudioSource.Stop();
                    // Play Audio Fx
                    PlayStartLevelFx();

                    // Set the Canvas Panels Initial State
                    SetCanvasPanelsState();

                    // Trigger the Level Reference Setting Event
                    EventManager.SetLevelReferences();
                    break;
                default:
                break;
            }
        }
    }

    private void OnEnable()
    {
        EventManager.onStartLevel += LevelStart;
    }
    private void OnDisable()
    {
        EventManager.onStartLevel -= LevelStart;
    }
    #endregion

    #region Canvas Methods
    private bool GetCanvasRefs()
    {
        if (System.Enum.TryParse(SceneManager.GetActiveScene().name, out Scenes currentScene))
        {
            switch (currentScene)
            {
                case Scenes.Menu:
                    // Get the Canvas ref
                    canvas = GameObject.Find("Canvas");
                    if (canvas == null)
                    {
                        Debug.LogError("The " + canvas + " object is null");
                        return false;
                    }
                    // Get the panels refs.
                    titlePanel = canvas.transform.Find("TitlePanel")?.gameObject;
                    if (titlePanel == null)
                    {
                        Debug.LogError("The " + titlePanel + " object is null");
                        return false;
                    }
                    break;
                case Scenes.Level1:
                case Scenes.Level2:
                case Scenes.Level3:
                    // Get the Canvas ref
                    canvas = GameObject.Find("Canvas");
                    if (canvas == null)
                    {
                        Debug.LogError("The " + canvas + " object is null");
                        return false;
                    }
                    //// Get the Pause, Win & Loose Panels GOs                
                    //pausePanel = canvas.transform.Find("PausePanel")?.gameObject;
                    //if (pausePanel == null)
                    //{
                    //    Debug.LogError("The " + pausePanel + " object is null");
                    //    return false;
                    //}
                    //winPanel = canvas.transform.Find("WinPanel")?.gameObject;
                    //if (winPanel == null)
                    //{
                    //    Debug.LogError("The " + winPanel + " object is null");
                    //    return false;
                    //}
                    //loosePanel = canvas.transform.Find("LoosePanel")?.gameObject;
                    //if (loosePanel == null)
                    //{
                    //    Debug.LogError("The " + loosePanel + " object is null");
                    //    return false;
                    //}
                    //LevelPassedPanel = canvas.transform.Find("LevelPassedPanel")?.gameObject;
                    //if (LevelPassedPanel == null)
                    //{
                    //    Debug.LogError("The " + LevelPassedPanel + " object is null");
                    //    return false;
                    //}
                    break;
                default:
                    break;
            }
        }
        return true;
    }
    private void SetCanvasPanelsState()
    {
        if (System.Enum.TryParse(SceneManager.GetActiveScene().name, out Scenes currentScene))
        {
            switch (currentScene)
            {
                case Scenes.Menu:
                    // Enable the TitlePanel Screen
                    titlePanel.SetActive(true);
                    break;
                case Scenes.Level1:
                case Scenes.Level2:
                case Scenes.Level3:
                    //// Disable all the Panels screens
                    //pausePanel.SetActive(false);
                    //SetWinPanel(false);
                    //SetLoosePanel(false);
                    //SetLevelPassedPanel(false);
                    break;
                default:
                    break;
            }
        }        
    }
    #endregion

    #region Button Methods
    public void OnStartGameClick()
    {                
        SceneManager.LoadScene(Scenes.Level1.ToString());
    }
    #endregion

    #region Level Management    
    public void SetLeveLReferences(Bird player, GameDependencies dependencies)
    {
        this.Player = player;
        this.Dependencies = dependencies;                
    }
    private void LevelStart()
    {
        Dependencies.InjectPlayerDependencies();
    }
    private void LevelWin()
    {
        
    }
    private void LevelFail()
    {
        
    }
    #endregion

    #region Audio Management
    private void PlayAudioFx(AudioSource audiousource, AudioClip audioClip)
    {
        if (audiousource == null)
        {
            Debug.LogWarning("There is no Ref. of the Audio Source " + audiousource);
            return;
        }
        else if (audioClip == null)
        {
            Debug.LogWarning("There is no Ref. of the Audio Fx " + audioClip);
            return;
        }
        // If all the audio refs. are added. then we play the audio fx
        audiousource.PlayOneShot(audioClip);
    }
    public void PlayStartLevelFx()
    {
        PlayAudioFx(gameAudioSource, startLevelClip);
    }
    public void PlayWinLevelFx()
    {
        PlayAudioFx(gameAudioSource, winLevelClip);
    }
    public void PlayFailLevelFx()
    {
        PlayAudioFx(gameAudioSource, failLevelClip);
    }
    public void PlayMainTitleAudioClip()
    {
        PlayAudioFx(gameAudioSource, mainTitleAudioClip);
    }
    #endregion
}
