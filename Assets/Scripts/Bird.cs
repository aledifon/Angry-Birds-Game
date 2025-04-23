using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    [SerializeField] float maxStretch;  // Max stretching distance   

    [Header("Line Renderers")]
    [SerializeField] LineRenderer catapultFront;
    [SerializeField] LineRenderer catapultBack;

    // GO Components
    SpringJoint2D spring;
    Rigidbody2D rb2D;

    Ray rayToMouse;
    Ray leftCatapultToBird;

    Vector2 prevVelocity;       // To save the Bird Previous Frame Speed
    float circleRadius;         // Collider radius
    bool clickedOn;             // To know if the player has clicked over the bird

    [SerializeField] LayerMask groundLayer;    

    #region Unity API
    private void Awake()
    {
        spring = GetComponent<SpringJoint2D>();
        rb2D = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        LineRendererSetup();

        // Initial Raycasts Positions (direction will be set afterwards)
        rayToMouse = new Ray(catapultBack.transform.position, Vector3.zero);
        leftCatapultToBird = new Ray(catapultFront.transform.position, Vector3.zero);
        circleRadius = GetComponent<CircleCollider2D>().radius;
    }
    private void OnEnable()
    {
        EventManager.onPlayerDisable += PlayerDisable;
    }
    private void OnDisable()
    {
        EventManager.onPlayerDisable -= PlayerDisable;
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
            catapultBack.enabled = false;
            catapultFront.enabled = false;
        }
    }    

    // Also detected on tactile screen
    private void OnMouseDown()
    {
        spring.enabled = false;
        clickedOn = true;               
    }
    private void OnMouseUp()
    {
        spring.enabled = true;
        clickedOn = false;
        rb2D.bodyType = RigidbodyType2D.Dynamic;
    }
    #endregion

    void LineRendererSetup()
    {
        catapultBack.SetPosition(0, catapultBack.transform.position);
        catapultFront.SetPosition(0, catapultFront.transform.position);

        //catapultBack.SetPosition(1, catapultBack.transform.position);
        //catapultFront.SetPosition(1, catapultFront.transform.position);
    }
    void LineRendererUpdate()
    {
        // Vector goes from the front catapult to the bird position
        Vector2 catapultToBird = transform.position - catapultFront.transform.position;
        // Assign the same dir. vector to the raycast
        leftCatapultToBird.direction = catapultToBird;

        // Get a point along the raycast whose value = Vector.magnitude + BirdCollider.radius
        // "Point just behind the bird"
        Vector3 endRopePosition = leftCatapultToBird.GetPoint(catapultToBird.magnitude + circleRadius);
        endRopePosition.z = -1; // To keep the rendering order
        
        // Setting the Line Renderers End points
        catapultFront.SetPosition(1, endRopePosition);
        catapultBack.SetPosition(1, endRopePosition);
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
        Vector2 catapultToMouse = mouseWorldPoint - catapultBack.transform.position;

        // Set the raycast clamped distance and the Raycast direction
        float rayDistance = Mathf.Clamp(catapultToMouse.magnitude, 0, maxStretch);
        Vector2 rayDirection = catapultToMouse.normalized;

        // Raycast Launching
        RaycastHit2D hit = Physics2D.Raycast(rayToMouse.origin,
                                            rayDirection,
                                            rayDistance,
                                            groundLayer);        

        // Floor detected --> Mouse Pos. = Floor pos.
        if (hit.collider != null)
        {
            mouseWorldPoint = hit.point;
            Debug.Log("Im hitting on the Gameobject" + hit.collider.gameObject);
        }
        // Max. Distance Reached --> Mouse Pos. = Point along the Raycast at Max Stretch Distance
        else if (catapultToMouse.magnitude > maxStretch)
        {       
            rayToMouse.direction = rayDirection;                    // Needed to be assigned to can use Ray.GetPoint() afterwards                                                                        
            mouseWorldPoint = rayToMouse.GetPoint(maxStretch);            
        }        
        // Update the new Bird Position (Reset Z-Axis to avoid problems)
        mouseWorldPoint.z = 0;
        transform.position = mouseWorldPoint;

        // Raycast Debugging
        Debug.DrawLine(rayToMouse.origin, 
                        rayToMouse.origin + rayToMouse.direction * rayDistance, 
                        Color.red);        
    }

    void PlayerDisable()
    {
        Destroy(gameObject,1f);               
    }
}
