using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TouchFinger : MonoBehaviour
{
    [SerializeField] GameObject ball;
    private GameObject ballInstance;
    [SerializeField] GameObject asteroid;
    private GameObject asteroidInstance;
    [SerializeField] TextMeshProUGUI textUI;    

    [SerializeField] private float cameraDistance;

    [SerializeField] private int maxNumFingers;
    private Vector3[] screenPosTouch;    

    private void Start()
    {
        //Instantiate(ball);
        screenPosTouch = new Vector3[maxNumFingers];
    }

    void Update()
    {
        FingersUI();
        CreateObjects();
    }
    void FingersUI()
    {
        int n = Input.touchCount;
        textUI.text = "Dedos: "+ n.ToString();
    }
    void CreateObjects()
    {
        int n = Input.touchCount;

        // If there is one finger over the screen device
        if (n > 0)
        {
            Touch touch = Input.GetTouch(0);    // It gets the 1st finger            

            // Ball instance            
            if ((int)touch.phase <= 2)
            {
                if(ballInstance == null)
                    ballInstance = Instantiate(ball);

                screenPosTouch[0] = new Vector3(touch.position.x,
                                            touch.position.y,
                                            Mathf.Abs(Camera.main.transform.position.z));
                ballInstance.transform.position =
                    Camera.main.ScreenToWorldPoint(screenPosTouch[0]);
            }
            else if (ballInstance != null && (int)touch.phase >= 3)
            {
                Destroy(ballInstance,1f);
                ballInstance = null;
            }

            if (n > 1)
            {
                Touch touch2 = Input.GetTouch(1);   // It gets the 2nd finger

                // Asteroid instance                
                if ((int)touch2.phase <= 2)
                {
                    if (asteroidInstance == null)
                        asteroidInstance = Instantiate(asteroid);

                    screenPosTouch[1] = new Vector3(touch2.position.x,
                                                touch2.position.y,
                                                Mathf.Abs(Camera.main.transform.position.z));
                    asteroidInstance.transform.position =
                        Camera.main.ScreenToWorldPoint(screenPosTouch[1]);
                }
                else if (asteroidInstance != null && (int)touch2.phase >= 3)
                {
                    Destroy(asteroidInstance,1f);
                    asteroidInstance = null;
                }
            }
        }
        else 
        {
            if (ballInstance != null)
            {
                Destroy(ballInstance,1f);
                ballInstance = null;
            }
            if (asteroidInstance != null)
            {
                Destroy(asteroidInstance,1f);
                asteroidInstance = null;
            }
        }
    }
}
