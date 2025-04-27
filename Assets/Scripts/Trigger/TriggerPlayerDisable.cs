using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPlayerDisable : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Bird player = collision.gameObject.GetComponent<Bird>();
            if (player != null)
            {
                if (!player.TouchedGround)
                {
                    player.TouchedGround = true;
                    EventManager.PlayerDisableEvent();
                }
            }            
        }
    }
}
