using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Transform showLevelCamPos;
    [SerializeField] Transform startGameCamPos;
    [SerializeField] float damping;    
    [SerializeField] float initDamping;    

    private float cameraDistance;
    private float yCamPos;
    private float xCamPos;
    private float zCamPos;

    private bool bEnableFollowPlayerPos;           

    private void Awake()
    {
        // Set the initial Cam Position to thow the level
        transform.position = showLevelCamPos.position;

        // Set the Z and Y Cam Positions
        zCamPos = Camera.main.transform.position.z;
        yCamPos = Camera.main.transform.position.y;
        xCamPos = Camera.main.transform.position.x;

        // Set the initial Boolean flags values
        bEnableFollowPlayerPos = false;        
    }
    private void OnEnable()
    {
        EventManager.onTriggerCamMove += EnableFollowPlayerPos;
        EventManager.onStartLevel += EnableGoToInitCamPos;
        EventManager.onRestartLevel += EnableGoToInitCamPos;
    }
    private void OnDisable()
    {
        EventManager.onTriggerCamMove -= EnableFollowPlayerPos;
        EventManager.onStartLevel -= EnableGoToInitCamPos;
        EventManager.onRestartLevel -= EnableGoToInitCamPos;
    }
    // Start is called before the first frame update
    void Update()
    {
        // Set the corresponding Target Camera position
        if (bEnableFollowPlayerPos)
            FollowPlayerCamPos();        

        // Camera movement to target position
        MoveCamera();

        // Check if the end position will be reached soon
        if (bEnableFollowPlayerPos)
            CheckReachedPosition();
    }
    void FollowPlayerCamPos()
    {
        xCamPos = player.position.x;        
    }
    void SetEndLevelTargetPos()
    {
        xCamPos = showLevelCamPos.position.x;
    }
    void EnableFollowPlayerPos()
    {
        bEnableFollowPlayerPos = true;
    }
    void EnableGoToInitCamPos()
    {
        bEnableFollowPlayerPos = false;
        StartCoroutine(nameof(SetCamToInitPos));
    }
    private IEnumerator SetCamToInitPos()
    {
        yield return new WaitForSeconds(2f);
        xCamPos = startGameCamPos.position.x;
    }
    void MoveCamera()
    {
        // Calculate the camera position
        Vector3 desiredPosition = new Vector3(xCamPos, yCamPos, zCamPos);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * damping);
    }    
    void CheckReachedPosition()
    {
        if (transform.position.x >= showLevelCamPos.position.x - 5f)
        {
            bEnableFollowPlayerPos = false;
            SetEndLevelTargetPos();
        }
    }
}
