using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class DialogueInteraction : MonoBehaviour
{
    public GameObject dialogueBox;
    public DialogueSetter dialogueSetterScript;
    public Dialogue dialogueScript;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
  

    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Dialogue Trigger" && dialogueScript.typing == false)
        {
            dialogueBox = other.gameObject;
            dialogueSetterScript = dialogueBox.GetComponent<DialogueSetter>();
            dialogueScript.lines = dialogueSetterScript.Lines;
            dialogueScript.StartDialogue();
        }
    }

}
