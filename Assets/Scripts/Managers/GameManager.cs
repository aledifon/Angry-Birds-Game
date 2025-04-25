using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [SerializeField] private PlayerDependencies playerDependencies;

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
        //EventManager.StartLevel();

        // Get the Canvas ref
        canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            Debug.LogError("The " + canvas + " object is null");
            return;
        }                    

        if (System.Enum.TryParse(SceneManager.GetActiveScene().name, out Scenes currentScene))
        {
            switch (currentScene)
            {
                case Scenes.Menu:
                    // Set the new Scene as the current one
                    sceneSelected = Scenes.Menu;

                    // Get the panels refs.
                    titlePanel = canvas.transform.Find("TitlePanel")?.gameObject;
                    if (titlePanel == null)
                    {
                        Debug.LogError("The " + titlePanel + " object is null");
                        return;
                    }

                    // Start playing Title Screen Audio
                    PlayMainTitleAudioClip();

                    // Enable the TitlePanel Screen
                    titlePanel.SetActive(true);

                    break;
                case Scenes.Level1:

                    // Stop Intro Song
                    gameAudioSource.Stop();

                    // Play Audio Fx
                    PlayStartLevelFx();

                    // Start the level
                    EventManager.StartLevel();
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

    #region Button Methods
    public void OnStartGameClick()
    {        
        // Disable the Title Panel
        //titlePanel.SetActive(false);

        SceneManager.LoadScene(Scenes.Level1.ToString());
    }
    #endregion

    #region Level Management    
    private void LevelStart()
    {        
        // Play Audio Fx
        PlayStartLevelFx();
    }
    private void LevelWin()
    {
        PlayWinLevelFx();
    }
    private void LevelFail()
    {
        PlayFailLevelFx();
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
    private void PlayStartLevelFx()
    {
        PlayAudioFx(gameAudioSource, startLevelClip);
    }
    private void PlayWinLevelFx()
    {
        PlayAudioFx(gameAudioSource, winLevelClip);
    }
    private void PlayFailLevelFx()
    {
        PlayAudioFx(gameAudioSource, failLevelClip);
    }
    private void PlayMainTitleAudioClip()
    {
        PlayAudioFx(gameAudioSource, mainTitleAudioClip);
    }
    #endregion
}
