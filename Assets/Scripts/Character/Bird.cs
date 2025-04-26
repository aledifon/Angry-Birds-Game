using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    // Pos. Refs
    private Transform startPos;

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
        //LineRendererSetup();  --> Will be called from GameManager to pass it the Player Dependencies

        // Initial Raycasts Positions (direction will be set afterwards)
        //rayToMouse = new Ray(catapultBackLR.transform.position, Vector3.zero);
        //leftCatapultToBird = new Ray(catapultFrontLR.transform.position, Vector3.zero);
        circleRadius = GetComponent<CircleCollider2D>().radius;
    }
    private void OnEnable()
    {
        EventManager.onPlayerTouchGround += PlayerTouchGround;
        EventManager.onPlayerHitWood += PlayBirdHitFx;    
        EventManager.onPlayerDepsInjected += LineRendererSetup;
        EventManager.onPlayerDepsInjected += RaycastSetup;
        EventManager.onPlayerDepsInjected += ResetPlayerSetup;
    }
    private void OnDisable()
    {
        EventManager.onPlayerTouchGround -= PlayerTouchGround;
        EventManager.onPlayerHitWood -= PlayBirdHitFx;
        EventManager.onPlayerDepsInjected -= LineRendererSetup;
        EventManager.onPlayerDepsInjected -= RaycastSetup;
        EventManager.onPlayerDepsInjected -= ResetPlayerSetup;
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
                //Destroy(spring);
                spring.enabled = false;
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

    #region Raycast Setup Methods
    private void RaycastSetup()
    {
        // Initial Raycasts Positions (direction will be set afterwards)
        rayToMouse = new Ray(catapultBackLR.transform.position, Vector3.zero);
        leftCatapultToBird = new Ray(catapultFrontLR.transform.position, Vector3.zero);
    }
    #endregion

    #region Line Renderer Methods
    void LineRendererSetup()
    {
        catapultBackLR.SetPosition(0, catapultBackLR.transform.position);
        catapultFrontLR.SetPosition(0, catapultFrontLR.transform.position);
    }
    void LineRendererUpdate()
    {
        if (catapultBackLR != null && catapultFrontLR != null && leftCatapultToBird.origin != Vector3.zero)
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
    }
    #endregion

    #region Player Movement
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

    #region Touch Ground Event
    void PlayerTouchGround()
    {
        PlayBirdHitFx();
        // Launch Player Reset Coroutine
        StartCoroutine(nameof(PlayerReset));
    }
    #endregion

    #region Player Setup
    IEnumerator PlayerReset()
    {
        yield return new WaitForSeconds(2f);
        // Disable the Player 
        gameObject.SetActive(false);

        yield return new WaitForSeconds(2f);        
        // Update the PLayer's Sprite positions

        // Setup the Player's config & Catapult again
        ResetPlayerSetup();        
    }
    void ResetPlayerSetup()
    {
        // Move the player again to the Start Game Position
        transform.position = startPos.position;

        // Set the Player's Rb as Kinematic again
        rb2D.bodyType = RigidbodyType2D.Kinematic;
        // Set the Player's Rb Velocity to zero
        rb2D.velocity = Vector2.zero;

        // Connect the Catapult Rb to the Spring Joint 2D Component
        spring.connectedBody = catapultRb2D;
        // Enable the Spring Joint 2D
        spring.enabled = true;

        // Re-enable again the Catapult Line Renderers
        catapultBackLR.enabled = true;
        catapultFrontLR.enabled = true;

        // Enable again the Player 
        gameObject.SetActive(true);
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
    public void SetDependencies(Catapult catapult, LineRenderer catapultFrontLR, LineRenderer catapultBackLR, Rigidbody2D catapultRb2D, Transform startPos)
    {
        this.catapult = catapult;
        this.catapultFrontLR = catapultFrontLR;
        this.catapultBackLR = catapultBackLR;
        this.catapultRb2D = catapultRb2D;
        this.startPos = startPos;
    }    
    #endregion
}
