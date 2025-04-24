using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catapult : MonoBehaviour
{
    AudioSource catapultAudioSource;

    [Header("Audio Clips")]
    [SerializeField] AudioClip catapultStretchedFx;
    [SerializeField] AudioClip catapultReleasedFx;

    private void Awake()
    {        
        catapultAudioSource = GetComponent<AudioSource>();
    }

    #region Audio Fx
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
    public void PlayCatapultStretchedFx()
    {
        PlayAudioFx(catapultAudioSource, catapultStretchedFx);
    }
    public void PlayCatapultReleasedFx()
    {
        PlayAudioFx(catapultAudioSource, catapultReleasedFx);
    }    
    #endregion
}
