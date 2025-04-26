using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void OnTriggerCamMove();                  // Delegate Template
    public static event OnTriggerCamMove onTriggerCamMove;     // Event

    public delegate void OnPlayerInitialSetup();
    public static event OnPlayerInitialSetup onPlayerInitialSetup;

    public delegate void OnPlayerTouchGround();
    public static event OnPlayerTouchGround onPlayerTouchGround;

    public delegate void OnPlayerHitWood();
    public static event OnPlayerHitWood onPlayerHitWood;

    public delegate void OnSetLevelReferences();
    public static event OnSetLevelReferences onSetLevelReferences;

    public delegate void OnStartLevel();
    public static event OnStartLevel onStartLevel;

    public static void TriggerCamMoveEvent()
    {
        onTriggerCamMove?.Invoke();
    }
    public static void PlayerDisableEvent()
    {
        onPlayerTouchGround?.Invoke();
    }
    public static void PlayerHitWood()
    {
        onPlayerHitWood?.Invoke();
    }
    public static void PlayerInitialSetup()
    {
        onPlayerInitialSetup?.Invoke();
    }
    public static void StartLevel()
    {
        onStartLevel?.Invoke();
    }
    public static void SetLevelReferences()
    {
        onSetLevelReferences?.Invoke();
    }
}
