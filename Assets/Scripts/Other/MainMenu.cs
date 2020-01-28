using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public void OnStartButtonPressed() {
        SceneManager.LoadScene(1);
    }

    public void OnQuitButtonPressed() {
        F1SLink.sng.QuitGameSession();
    }
}
