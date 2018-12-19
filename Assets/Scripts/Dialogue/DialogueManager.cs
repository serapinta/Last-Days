using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour {

    public Animator dialogueBox;
    public Text nameText;
    public Text dialogueText;
    public Button[] buttons;
    public GameObject myPlayer;

    private FirstPersonController _playerController;
    private Player _playerStats;
    private Queue<string> sentences;
    private Dialogue dialogue;

    // Use this for initialization
    void Start () {
        sentences = new Queue<string>();
        _playerController = myPlayer.GetComponent<FirstPersonController>();
        _playerStats = myPlayer.GetComponent<Player>();

    }

    private void Update() {
        
        if (dialogueBox == null) {

            dialogueBox = GameObject.Find("DialogueBox").GetComponent<Animator>();
        }
    }

    public void StartDialogue (Dialogue[] dialogue, string name) {
        dialogueBox.gameObject.SetActive(true);
        sentences.Clear();

        this.dialogue = dialogue[_playerStats.currentMission];

        dialogueBox.SetBool("isOpen", true);
        _playerController.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //nameText.text = name;

        foreach (string sentence in this.dialogue.sentence) {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence() {
        if (sentences.Count == 0) {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();

        dialogueText.text = sentence;
    }

    public void EndDialogue() {
        dialogueBox.SetBool("isOpen", false);
        dialogueBox.gameObject.SetActive(false);
        _playerController.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
