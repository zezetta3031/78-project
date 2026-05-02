using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;


//SE = Self Explanatory | function does not require further explanation
public class Firstbossmovement : MonoBehaviour
{
    private int[] directionsArr = {-1,1}; //Right = 1 | Left = -1
    private int direction;
    private float currentSpeed; //SE
    private float maxSpeed = 10; //SE
    private float distanceToDestination; //total distance to cover
    private int maxTravelDistance = 30; //SE
    private float distanceTravelled; //SE 
    private float distanceToNext; //Distance to next waypoint on the track
    private Vector2 lastPosition;
    private float acceleration = 2; 
    [SerializeField] GameObject nextWaypoint; //nearest waypoint in direction
    [SerializeField] GameObject[] waypoints;
    private Rigidbody2D rb;


    private Vector2 destination;

    void Awake()
    {
        
        rb = GetComponent<Rigidbody2D>();
        nextWaypoint = waypoints[UnityEngine.Random.Range(0,waypoints.Length)];
        GenerateDestination();
    }



    // Update is called once per frame
    void Update()
    {
        MoveToDestination();
        UpdateValues();
    }

    public void GenerateDestination()
    {
        lastPosition = rb.transform.position;
        direction = directionsArr[UnityEngine.Random.Range(0,directionsArr.Length)];
        distanceToDestination = UnityEngine.Random.Range(15,300);
    }   

    private void MoveToDestination()
    {  
        destination = (nextWaypoint.transform.position - transform.position).normalized;
        Vector2 targetVelocity = destination * maxSpeed;
        rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, acceleration * Time.deltaTime);
        distanceTravelled += Vector2.Distance(rb.transform.position, lastPosition);
        lastPosition = rb.transform.position;
        if(distanceToNext < 0.01)
        {
            ChangeDirection(nextWaypoint.transform);
            int currentIndex = Array.IndexOf(waypoints, nextWaypoint);
            Debug.Log("Index: "+ currentIndex);
            Debug.Log("Direction: " + direction);
            Debug.Log("Distance Travelled: " + distanceTravelled);
            if(currentIndex  + 1 * direction < 0)
            {
                currentIndex = 3;
                nextWaypoint = waypoints[currentIndex];
            }
            else if(currentIndex + 1 * direction > 3)
            {
                Debug.Log("test");
                currentIndex = 0;
                nextWaypoint = waypoints[currentIndex];
            }
            else 
            {
                nextWaypoint = waypoints[ currentIndex + ( 1 * direction)];
            }
        }
    }

    private void UpdateValues()
    {
        distanceToNext = Vector2.Distance(transform.position, nextWaypoint.transform.position);
    }

    private void ChangeDirection(Transform waypoint)
    {
        rb.velocity = waypoint.forward * rb.velocity.magnitude;
    }
}
