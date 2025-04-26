using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDependencies : MonoBehaviour
{
    [Header("Start Game Position")]
    [SerializeField] private Transform startPos;
    [Header("Catapult Script")]
    [SerializeField] private Catapult catapult;
    [Header("Catapult Line Renderers Refs")]
    [SerializeField] private LineRenderer catapultFrontLineRenderer;
    [SerializeField] private LineRenderer catapultBackLineRenderer;
    [Header("Catapult RigidBody Ref.")]
    [SerializeField] private Rigidbody2D catapultRb2D;

    public void InjectDependencies(Bird bird)
    {
        bird.SetDependencies(catapult,catapultFrontLineRenderer,catapultBackLineRenderer,catapultRb2D, startPos);
        // Trigger the Player Dependencies Injected Event
        EventManager.PlayerDepsInjected();
    }
}
