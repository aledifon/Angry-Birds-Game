using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerWoodenCollision : MonoBehaviour
{
    AudioSource audioSourceWood;
    [SerializeField] AudioClip woodCollisionFx;

    private void Awake()
    {
        audioSourceWood = GetComponent<AudioSource>();
    }
    private void OnEnable()
    {
        EventManager.onPlayerHitWood += PlayWoodHitFx;
    }
    private void OnDisable()
    {
        EventManager.onPlayerHitWood -= PlayWoodHitFx;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Get the Bird Component attached to the PLayer GO
            Bird bird = collision.gameObject.GetComponent<Bird>();
            if (bird != null)
            {
                Vector2 hitPoint = collision.GetContact(0).point;
                bird.PLayDustAnimation(hitPoint);
            }
            // Trigger the Player Hit Wood Event
            EventManager.PlayerHitWood();                
        }
    }

    private void PlayAudioFx(AudioSource audiousource, AudioClip audioClip)
    {
        if (audiousource == null)
        {
            Debug.LogWarning("There is no Ref. of the AudioSource Catapult");
            return;
        }
        else if (audioClip == null)
        {
            Debug.LogWarning("There is no Ref. of the Catapult Audio Fx");
            return;
        }
        // If all the audio refs. are added. then we play the audio fx
        audiousource.PlayOneShot(audioClip);
    }
    private void PlayWoodHitFx()
    {
        PlayAudioFx(audioSourceWood, woodCollisionFx);
    }
}
