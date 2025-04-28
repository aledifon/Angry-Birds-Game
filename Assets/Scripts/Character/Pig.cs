using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pig : MonoBehaviour
{
    Animator animator;
    CircleCollider2D circleCollider2D;

    AudioSource pigAudioSource;
    [SerializeField] AudioClip pigHitFx;
    [SerializeField] AudioClip pigInflateFx;
    [SerializeField] AudioClip pigExplodeFx;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        pigAudioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player") ||
            collision.gameObject.CompareTag("Ground"))
        {
            Die();
        }
    }

    private void Die()
    {
        // Decrease the Num of Enemies of the Level
        GameManager.Instance.CurrentNumOfEnemies--;        

        //circleCollider2D.enabled = false;
        PlayDieAnimation();
        Destroy(gameObject,4f);
    }

    #region Audio Methods
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
    public void PlayPigHitFx()
    {
        PlayAudioFx(pigAudioSource, pigHitFx);
    }
    public void PlayPigInflateFx()
    {
        PlayAudioFx(pigAudioSource, pigInflateFx);
    }
    public void PlayPigExplodeFx()
    {
        PlayAudioFx(pigAudioSource, pigExplodeFx);
    }
    #endregion

    #region Anim Methods
    private void PlayDieAnimation()
    {
        animator.SetTrigger("PigDie");
    }
    #endregion
}
