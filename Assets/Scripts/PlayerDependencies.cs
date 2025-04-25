using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDependencies : MonoBehaviour
{
    [Header("Script")]
    [SerializeField] private Catapult catapult;
    [Header("Line Renderers Refs")]
    [SerializeField] private LineRenderer catapultFrontLineRenderer;
    [SerializeField] private LineRenderer catapultBackLineRenderer;
    [Header("RigidBody Ref.")]
    [SerializeField] private Rigidbody2D catapultRb2D;

    public void InjectDependencies(Bird bird)
    {
        bird.SetDependencies(catapult,catapultFrontLineRenderer,catapultBackLineRenderer,catapultRb2D);
    }
}
