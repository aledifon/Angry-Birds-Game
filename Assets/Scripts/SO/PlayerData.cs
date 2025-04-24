using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Data/Player", fileName = "New Player")]
public class PlayerData : ScriptableObject
{
    [Header("Line Renderers")]
    LineRenderer catapultFront;
    LineRenderer catapultBack;

    [Header("Catapult Ref")]
    Catapult catapult;

    [Header("Audio Clips")]
    [SerializeField] AudioClip birdFlyingFx;
    [SerializeField] AudioClip birdHitFx;

    [Header("Ground Layer")]
    [SerializeField] LayerMask groundLayer;

    [Header("Dust Effect")]
    [SerializeField] GameObject dustEffect;
}
