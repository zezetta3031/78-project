using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public GameObject teleportDestination;
    private float xPos;
    private float yPos;
    public GameObject playerObject;
    public Vector2 endPosition;
    public bool levelChange;
    public Animator animator;
    // Start is called before the first frame update
    void Awake()
    {
        xPos = teleportDestination.transform.position.x;
        yPos = teleportDestination.transform.position.y;
        endPosition = teleportDestination.transform.position;
    }
    public void Update()
    {
        // Debug.Log(playerObject);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.gameObject.tag);
        if(other.gameObject.tag.Contains("Player")){
            if(levelChange){    
                animator.SetTrigger("levelChange");
            }
            playerObject = other.gameObject;
            playerObject.transform.position = endPosition;
        }
        
    }

}
