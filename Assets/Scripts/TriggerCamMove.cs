using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCamMove : MonoBehaviour
{    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            EventManager.TriggerCamMoveEvent();
        }
    }
}
