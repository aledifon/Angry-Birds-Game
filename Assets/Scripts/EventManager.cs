using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void OnTriggerCamMove();                  // Delegate Template
    public static event OnTriggerCamMove onTriggerCamMove;     // Event

    public delegate void PlayerDisable();
    public static event PlayerDisable onPlayerDisable;
    
    public static void TriggerCamMoveEvent()
    {
        onTriggerCamMove?.Invoke();
    }
    public static void PlayerDisableEvent()
    {
        onPlayerDisable?.Invoke();
    }

}
