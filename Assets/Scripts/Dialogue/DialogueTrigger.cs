using UnityEngine;

public class DialogueTrigger : MonoBehaviour {

    public string name;

    public Dialogue[] dialogue;
    
    public void TriggerDialogue() {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue, name);
    }
}
