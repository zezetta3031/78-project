using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleporter : MonoBehaviour
{
    public GameObject teleportDestination;
    private float xPos;
    private float yPos;
    public GameObject playerObject;
    public Vector2 endPosition;
    public bool levelChange;
    public Animator animator;
    public GameObject triggerField;
    public string nextLevelName;
    
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
        if(other.gameObject.CompareTag("Player")){
            if(levelChange){    
                // StartCoroutine(startAnimation(other));
                SceneManager.LoadScene(nextLevelName);
            }
            else
            {
                teleportPlayer(playerObject);
            }
        }
        
    }

    IEnumerator startAnimation(Collider2D other)
    {
        float animationTime = 0.75f;
        animator.SetTrigger("levelChange");
        yield return new WaitForSeconds(animationTime);
        teleportPlayer(playerObject);
        yield return null;
    }


    public void teleportPlayer(GameObject player)
    {
        player.transform.position = endPosition;
    }

}
