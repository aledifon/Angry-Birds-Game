using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDependencies : MonoBehaviour
{
    [Header("Enemies")]
    [SerializeField] private int maxNumOfEnemies;
    public int MaxNumOfEnemies => maxNumOfEnemies;

    [Header("Player")]
    [SerializeField] private Bird player;
    public Bird Player => player;
    [SerializeField] private Transform startPlayerPos;
    public Transform StartPlayerPos => startPlayerPos;
    [SerializeField] private List<GameObject> playerLifesPos;
    public List<GameObject> PlayerLifesPos => playerLifesPos;

    [Header("Catapult Script")]
    [SerializeField] private Catapult catapult;
    public Catapult Catapult => catapult;

    [Header("Catapult Line Renderers Refs")]
    [SerializeField] private LineRenderer catapultFrontLineRenderer;
    public LineRenderer CatapultFrontLineRenderer => catapultFrontLineRenderer;
    [SerializeField] private LineRenderer catapultBackLineRenderer;
    public LineRenderer CatapultBackLineRenderer => catapultBackLineRenderer;

    [Header("Catapult RigidBody Ref.")]
    [SerializeField] private Rigidbody2D catapultRb2D;
    public Rigidbody2D CatapultRb2D => catapultRb2D;
    
    private void OnEnable()
    {
        EventManager.onSetLevelReferences += SetLevelReferences;
    }
    private void OnDisable()
    {
        EventManager.onSetLevelReferences -= SetLevelReferences;
    }

    private void SetLevelReferences()
    {
        GameManager.Instance.SetLeveLReferences(player, this);
        // Once the Level References has been asigned then trigger the Level Start Event
        EventManager.StartLevel();
    }
    public void InjectPlayerDependencies()
    {
        player.SetDependencies(catapult,catapultFrontLineRenderer,catapultBackLineRenderer,
                                catapultRb2D, startPlayerPos, playerLifesPos);
        // Once the Player Dependencies has been injected then Trigger the Player Dep. Inj. Event
        EventManager.PlayerInitialSetup();
    }
}
