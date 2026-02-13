using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class DialogueInteraction : MonoBehaviour
{
    public GameObject dialogueBox;
    public DialogueSetter dialogueSetterScript; // the script on a dialogueSetter gameobject
    public Dialogue dialogueScript; // the overall dialogue box object

    public void OnTriggerEnter2D(Collider2D other) // When the player enters a dialogue trigger
    {
        if (!other.CompareTag("Dialogue Trigger") && dialogueScript.typing)
            return;
        
        dialogueBox = other.gameObject;
        dialogueSetterScript = dialogueBox.GetComponent<DialogueSetter>();
        dialogueScript.lines = dialogueSetterScript.Lines;
        dialogueScript.freezePlayerDuringDialogue = dialogueSetterScript.freezePlayer;
        dialogueScript.StartDialogue();
    }

}
