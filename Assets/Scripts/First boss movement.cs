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
    private int maxTravellDistance = 30; //SE
    private float distanceTravelled; //SE 
    private float distanceToNext; //Distance to next waypoint on the track
    private float acceleration = 10; 
    [SerializeField] GameObject nextWaypoint; //nearest waypoint in direction
    [SerializeField] GameObject[] waypoints;
    private Rigidbody2D rb;


    private Vector2 destination;

    void Awake()
    {
        GenerateDestination();
        rb = GetComponent<Rigidbody2D>();
        nextWaypoint = waypoints[UnityEngine.Random.Range(0,waypoints.Length)];
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(direction);
        Debug.Log(Array.IndexOf(waypoints,nextWaypoint));
        MoveToDestination();
        UpdateValues();
    }

    public void GenerateDestination()
    {
        direction = directionsArr[UnityEngine.Random.Range(0,directionsArr.Length)];
        distanceToDestination = UnityEngine.Random.Range(0,30) * direction;
    }   

    private void MoveToDestination()
    {  
        destination = (nextWaypoint.transform.position - transform.position).normalized;
        Vector2 targetVelocity = destination * maxSpeed;
        rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, acceleration * Time.deltaTime);
        if(distanceToNext < 0.01)
        {
            rb.velocity = new Vector2(0,0);
            int currentIndex = Array.IndexOf(waypoints, nextWaypoint);
            nextWaypoint = waypoints[ currentIndex + ( 1 *direction)];

        }


    }

    private void UpdateValues()
    {
        distanceToNext = Vector2.Distance(transform.position, nextWaypoint.transform.position);
    }

    // private IEnumerator MoveToPoint(Vector2 position, Vector2 destination, float speed)
    //  {
        
        
    //  }
}
