using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    private AudioManager audioManager;

    private void Start() {
        audioManager = AudioManager.s_inst;
    }

    public void OnStartButtonPressed() {
        audioManager.Play2DSound(0);
        SceneManager.LoadScene(1);
    }

    public void OnQuitButtonPressed() {
        audioManager.Play2DSound(0);
        F1SLink.sng.QuitGameSession();
    }
}
