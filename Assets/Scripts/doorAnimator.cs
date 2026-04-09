using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorAnimatpr : MonoBehaviour
{
    public Animator animator;
    public GameObject playerObject;
    public Vector2 endPosition;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        // Debug.Log(other.gameObject.tag);
        if(other.gameObject.tag.Contains("Player")){
            StartCoroutine(startAnimation(other));
        }
        
    }
    IEnumerator startAnimation(Collider2D other)
    {
        float animationTime = 0.75f;
        animator.SetTrigger("levelChange");
        yield return new WaitForSeconds(animationTime);
        teleportPlayer(other);
        yield return null;
    }
    public void teleportPlayer(Collider2D other)
    {
        playerObject = other.gameObject;
        playerObject.transform.position = endPosition;
    }
}
