using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{        
    // Catapult Refs.
    private LineRenderer catapultFrontLR;
    private LineRenderer catapultBackLR;
    private Rigidbody2D catapultRb2D;
    [Header("Catapult Ref")]
    private Catapult catapult;

    // GO Components
    SpringJoint2D spring;
    Rigidbody2D rb2D;

    Ray rayToMouse;
    Ray leftCatapultToBird;

    Vector2 prevVelocity;       // To save the Bird Previous Frame Speed
    float circleRadius;         // Collider radius
    bool clickedOn;             // To know if the player has clicked over the bird

    [Header("Audio Refs")]    
    AudioSource birdAudioSource;

    private PlayerData playerData;    

    #region Unity API
    private void Awake()
    {
        spring = GetComponent<SpringJoint2D>();
        rb2D = GetComponent<Rigidbody2D>();
        birdAudioSource = GetComponent<AudioSource>();
    }
    void Start()
    {
        LineRendererSetup();

        // Initial Raycasts Positions (direction will be set afterwards)
        rayToMouse = new Ray(catapultBackLR.transform.position, Vector3.zero);
        leftCatapultToBird = new Ray(catapultFrontLR.transform.position, Vector3.zero);
        circleRadius = GetComponent<CircleCollider2D>().radius;
    }
    private void OnEnable()
    {
        EventManager.onPlayerTouchGround += PlayerDisable;
        EventManager.onPlayerHitWood += PlayBirdHitFx;    
    }
    private void OnDisable()
    {
        EventManager.onPlayerTouchGround -= PlayerDisable;
        EventManager.onPlayerHitWood -= PlayBirdHitFx;
    }
    void FixedUpdate()
    {
        // Launch Raycast from the bird to the left catapult
        // to continuously check the dist Bird-Catapult
        // This will be used to limitate the max. dist. Bird-Catapult

        // Launch Raycast from Screen to Mouse to know
        // the continuously know the mouse pos. on world space coord.

        if (clickedOn)        
            Dragging();        
        
        if(spring != null)
        {
            // If the Rb is not kinematic and the Prev. Frame Velocity > Current Frame Velocity
            // --> Destroy the Spring Joint Component
            if(!rb2D.isKinematic && (prevVelocity.magnitude > rb2D.velocity.magnitude))
            {
                //spring.connectedBody = null;
                Destroy(spring);
                rb2D.velocity = prevVelocity;
                // Set Game Feeling (Audio)
            }
            // Save the current value of the RigidBody Bird Velocity
            if (!clickedOn)                                            
                prevVelocity = rb2D.velocity;

            LineRendererUpdate();
        }
        else
        {
            // If I arrive here the bird has already flown
            catapultBackLR.enabled = false;
            catapultFrontLR.enabled = false;
        }
    }    

    // Also detected on tactile screen
    private void OnMouseDown()
    {
        spring.enabled = false;
        clickedOn = true;

        // Assign the Rigidbody2D to the Connected Body of the Spring Joint 2D Component
        spring.connectedBody = catapultRb2D;

        // Play Audio Fx
        catapult.PlayCatapultStretchedFx();
    }
    private void OnMouseUp()
    {
        spring.enabled = true;
        clickedOn = false;
        rb2D.bodyType = RigidbodyType2D.Dynamic;

        // Play Audio Fx
        catapult.PlayCatapultReleasedFx();
        PlayBirdFlyingFx();
    }
    #endregion

    #region Player Movement
    void LineRendererSetup()
    {
        catapultBackLR.SetPosition(0, catapultBackLR.transform.position);
        catapultFrontLR.SetPosition(0, catapultFrontLR.transform.position);

        //catapultBack.SetPosition(1, catapultBack.transform.position);
        //catapultFront.SetPosition(1, catapultFront.transform.position);
    }
    void LineRendererUpdate()
    {
        // Vector goes from the front catapult to the bird position
        Vector2 catapultToBird = transform.position - catapultFrontLR.transform.position;
        // Assign the same dir. vector to the raycast
        leftCatapultToBird.direction = catapultToBird;

        // Get a point along the raycast whose value = Vector.magnitude + BirdCollider.radius
        // "Point just behind the bird"
        Vector3 endRopePosition = leftCatapultToBird.GetPoint(catapultToBird.magnitude + circleRadius);
        endRopePosition.z = -1; // To keep the rendering order
        
        // Setting the Line Renderers End points
        catapultFrontLR.SetPosition(1, endRopePosition);
        catapultBackLR.SetPosition(1, endRopePosition);
    }
    // Update the bird pos. when I'm dragging him along the screen
    //void Dragging_B()
    //{
    //    if (Input.touchCount > 0)
    //    {
    //        // Calculate the Touched Screen Position on World space coordinates.            
    //        Vector3 screenPosTouch = new Vector3(Input.GetTouch(0).position.x,
    //                                            Input.GetTouch(0).position.y,
    //                                            Camera.main.transform.position.z);
    //        Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(screenPosTouch);            
            
    //        // Calculate the Raycast direction (from the catapult to the touched Screen Point)
    //        Vector2 catapultToMouse = mouseWorldPoint - rayToMouse.origin;
    //        rayToMouse.direction = catapultToMouse;                                   

    //        // If the touched screen point > Max. Stretch Distance
    //        // --> newBirdPos = Point along the raycast way at Max Stretch value

    //        // NEEDED to BE ADDED  an Edge Collider to the Ground
    //        if ((catapultToMouse.magnitude < maxStretch))
    //        {
    //            RaycastHit2D hit = Physics2D.Raycast(rayToMouse.origin, rayToMouse.direction);
    //            if (hit.collider != null)
    //                mouseWorldPoint = hit.point;
    //        }
    //        else if (catapultToMouse.magnitude > maxStretch)
    //        {
    //            mouseWorldPoint = rayToMouse.direction.normalized * maxStretch;
    //            mouseWorldPoint.z = 0;
    //        }                
    //        // Otherwise, the new Bird position will be the touched screen point

    //        // Update the New Bird Position
    //        transform.position = mouseWorldPoint;          
    //    }
    //}
    void Dragging()
    {
        // Calculate the Touched Screen Position on World space coordinates.
        Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Vector from the Back Catapult Init Point to the Touched screen point
        Vector2 catapultToMouse = mouseWorldPoint - catapultBackLR.transform.position;

        // Set the raycast clamped distance and the Raycast direction
        float rayDistance = Mathf.Clamp(catapultToMouse.magnitude, 0, playerData.MaxStretch);
        Vector2 rayDirection = catapultToMouse.normalized;

        // Raycast Launching
        RaycastHit2D hit = Physics2D.Raycast(rayToMouse.origin,
                                            rayDirection,
                                            rayDistance,
                                            playerData.GroundLayer);        

        // Floor detected --> Mouse Pos. = Floor pos.
        if (hit.collider != null)
        {
            mouseWorldPoint = hit.point;
            Debug.Log("Im hitting on the Gameobject" + hit.collider.gameObject);
        }
        // Max. Distance Reached --> Mouse Pos. = Point along the Raycast at Max Stretch Distance
        else if (catapultToMouse.magnitude > playerData.MaxStretch)
        {       
            rayToMouse.direction = rayDirection;                    // Needed to be assigned to can use Ray.GetPoint() afterwards                                                                        
            mouseWorldPoint = rayToMouse.GetPoint(playerData.MaxStretch);            
        }        
        // Update the new Bird Position (Reset Z-Axis to avoid problems)
        mouseWorldPoint.z = 0;
        transform.position = mouseWorldPoint;

        // Raycast Debugging
        Debug.DrawLine(rayToMouse.origin, 
                        rayToMouse.origin + rayToMouse.direction * rayDistance, 
                        Color.red);        
    }
    #endregion

    #region Player Disable
    void PlayerDisable()
    {
        PlayBirdHitFx();
        Destroy(gameObject,1f);               
    }
    #endregion

    #region Audio Fx
    private void PlayAudioFx(AudioSource audiousource, AudioClip audioClip)
    {        
        if (audiousource == null)
        {
            Debug.LogWarning("There is no Ref. of the Audio Source " + audiousource);
            return;
        }
        else if (audioClip == null)
        {            
            Debug.LogWarning("There is no Ref. of the Audio Fx " + audioClip);
            return;            
        }
        // If all the audio refs. are added. then we play the audio fx
        audiousource.PlayOneShot(audioClip);
    }    
    public void PlayBirdFlyingFx()
    {
        PlayAudioFx(birdAudioSource, playerData.BirdFlyingFx);
    }
    public void PlayBirdHitFx()
    {
        PlayAudioFx(birdAudioSource, playerData.BirdHitFx);
    }
    #endregion

    #region Animations
    public void PLayDustAnimation(Vector2 hitPosition)
    {
        Vector3 dustPos = new Vector3(hitPosition.x, hitPosition.y,0);
        GameObject dustInstance = Instantiate(playerData.DustEffect,dustPos,Quaternion.identity);
        StartCoroutine(DestroyDustAnimation(dustInstance));
    }
    IEnumerator DestroyDustAnimation(GameObject prefab)
    {
        yield return new WaitForSeconds(1f);
        if (prefab != null)
            Destroy(prefab);
    }
    #endregion

    #region Dependency Injection
    public void SetDependencies(Catapult catapult, LineRenderer catapultFrontLR, LineRenderer catapultBackLR, Rigidbody2D catapultRb2D)
    {
        this.catapult = catapult;
        this.catapultFrontLR = catapultFrontLR;
        this.catapultBackLR = catapultBackLR;
        this.catapultRb2D = catapultRb2D;
    }    
    #endregion
}
