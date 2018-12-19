using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

    public bool GameIsPaused = false;

    public GameObject pauseMenu;

    public GameObject centeredDot;
    public GameObject interactionPanel;
    public GameObject inventoryPanel;
    public GameObject dialogueBox;
    public DialogueManager dialogue;

    private GameObject player;

    private void Awake() {

        player = GameObject.FindWithTag("Player");
    }

    void Update() {

        if (player == null) {

            player = GameObject.FindWithTag("Player");
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {

            if (GameIsPaused) {

                Resume();

            } else {

                if (dialogueBox.activeSelf) {

                    dialogue.EndDialogue();
                    return;
                }

                Pause();
            }
        }
    }

    public void Resume() {

        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        player.GetComponent<FirstPersonController>().enabled = true;
        player.GetComponent<Player>().enabled = true;
        centeredDot.SetActive(true);
        interactionPanel.SetActive(true);
        Cursor.visible = false;
    }

    private void Pause() {
        
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        player.GetComponent<FirstPersonController>().enabled = false;
        player.GetComponent<Player>().enabled = false;
        centeredDot.SetActive(false);
        interactionPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LoadMenu() {

        pauseMenu.SetActive(false);
        GameIsPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void Quit() {

        // Just se we can see something happened in the Editor
        Debug.Log("QUIT!");

        Application.Quit();
    }
}
