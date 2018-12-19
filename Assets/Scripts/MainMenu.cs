using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    [SerializeField]
    private Texture2D _newCursor;

    public void Awake() {

        // Cursor is visible, unlocked and changed
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.SetCursor(_newCursor, Vector2.zero, CursorMode.Auto);
    }

    public void PlayGame() {

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame() {

        // Just se we can see something happened in the Editor
        Debug.Log("QUIT!");

        Application.Quit();
    }
}
