using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;


//SE = Self Explanatory | function does not require further explanation
public class Firstbossmovement : MonoBehaviour
{
    private int[] directionsArr = {-1,1}; //Right = 1 | Left = -1
    private int direction;
    private float currentSpeed; //SE
    private float maxSpeed; //SE
    private float distanceToDestination; //total distance to cover
    private int maxTravellDistance = 30; //SE
    private float distanceTravelled; //SE 
    private float distanceToNext; //Distance to next waypoint on the track
    private float acceleration = 5; 
    [SerializeField] GameObject nextWaypoint; //nearest waypoint in direction
    [SerializeField] GameObject[] waypoints;
    private Rigidbody2D rb;


    private Vector2 destination;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.JumpWasPressed)
        {
            MoveToDestination();
        }
    }

    public void GenerateDestination()
    {
        direction = directionsArr[Random.Range(0,directionsArr.Length)];
        distanceToDestination = Random.Range(0,30) * direction;
    }   

    private void MoveToDestination()
    {
        nextWaypoint = waypoints[Random.Range(0,waypoints.Length)];
        destination = nextWaypoint.transform.position;
        currentSpeed = Mathf.Lerp( currentSpeed, maxSpeed, acceleration * Time.deltaTime);
        rb.velocity = new Vector2(currentSpeed, 0f);


    }

    // private IEnumerator MoveToPoint(Vector2 position, Vector2 destination, float speed)
    // {
        
        
    // }
}
