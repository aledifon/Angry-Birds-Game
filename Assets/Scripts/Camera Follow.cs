using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float damping;    

    private float cameraDistance;
    private float yCamPos;
    private float xCamPos;
    private float zCamPos;

    private bool bEnableUpdatePos;

    private void Awake()
    {
        // Set the Z and Y Cam Positions
        zCamPos = Camera.main.transform.position.z;
        yCamPos = Camera.main.transform.position.y;
        xCamPos = Camera.main.transform.position.x;
    }
    private void OnEnable()
    {
        EventManager.onTriggerCamMove += EnableUpdateCamPos;
    }
    private void OnDisable()
    {
        EventManager.onTriggerCamMove -= EnableUpdateCamPos;
    }
    // Start is called before the first frame update
    void FixedUpdate()
    {
       if (bEnableUpdatePos)
            UpdateCamPos();
        MoveCamera();        
    }
    void UpdateCamPos()
    {
        xCamPos = player.position.x;        
    }
    void EnableUpdateCamPos()
    {
        bEnableUpdatePos = true;
    }
    void MoveCamera()
    {
        // Calculate the camera position
        Vector3 desiredPosition = new Vector3(xCamPos, yCamPos, zCamPos);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * damping);
    }    
}
