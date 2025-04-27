using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Data/Player", fileName = "New Player")]
public class PlayerData : ScriptableObject
{
    [SerializeField] float maxStretch;                      // Max stretching distance   
    public float MaxStretch {  get { return maxStretch; } }

    [SerializeField] int maxLifes;                      // Max Lifes
    public int MaxLifes { get { return maxLifes; } }

    [Header("Audio Clips")]
    [SerializeField] AudioClip birdFlyingFx;
    public AudioClip BirdFlyingFx { get => birdFlyingFx; }
    [SerializeField] AudioClip birdHitFx;
    public AudioClip BirdHitFx { get => birdHitFx; }

    [Header("Ground Layer")]
    [SerializeField] LayerMask groundLayer;
    public LayerMask GroundLayer { get => groundLayer; }

    [Header("Dust Effect")]
    [SerializeField] GameObject dustEffect;
    public GameObject DustEffect { get => dustEffect; }
}
