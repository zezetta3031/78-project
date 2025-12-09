using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UIElements;
using Unity.VisualScripting;


public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines; // The list of dialogue lines that will be displayed
    public float textspeed; // The speed that each letter is typed, lower means faster
    private int index; // The current text string in lines
    public bool typing; // true when the dialogue box is open
    public Animator animator;
    public GameObject player;
    public AdvancedMovementTest movementScript;
    public bool freezePlayerDuringDialogue;



    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        Debug.Log(player);
        movementScript = player.GetComponent<AdvancedMovementTest>();
        Debug.Log(movementScript);
        gameObject.SetActive(false);
        textComponent.text = string.Empty;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        }
    }
    public void StartDialogue() // Activates the dialogue box and begins typing
    {

        animator.SetTrigger("Open Dialogue");
        if (freezePlayerDuringDialogue)
        {
            movementScript.playerFreeze = true;
        } 
        if (typing == false)
        {
            gameObject.SetActive(true);
            index = 0;
            StartCoroutine(TypeLine());
        }
    }

    IEnumerator TypeLine() // Goes through each letter in the current line and displays it
    {
        typing = true;
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textspeed);
        }
        
    }

    void NextLine() // Sets the text component to next line in lines[] or disables if there are no more lines
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            textComponent.text = string.Empty;
            typing = false;
            movementScript.playerFreeze = false;
            animator.SetTrigger("End Dialogue");
            gameObject.SetActive(false);
        }
    }
}
