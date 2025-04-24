using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    AudioSource gameAudioSource;
    [SerializeField] AudioClip startLevelClip;
    [SerializeField] AudioClip winLevelClip;
    [SerializeField] AudioClip failLevelClip;
        
    void Awake()
    {
        gameAudioSource = GetComponent<AudioSource>();        
    }
    private void Start()
    {
        EventManager.StartLevel();
    }
    private void OnEnable()
    {
        EventManager.onStartLevel += LevelStart;
    }
    private void OnDisable()
    {
        EventManager.onStartLevel -= LevelStart;
    }
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
}
