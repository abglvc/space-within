using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour {
    public static UserInterface sng { get; private set; } //singletone
    public Text scoreText;
    public GameObject deathScreen, pauseScreen;
    public GameObject damageSplash;
    public Slider sensorSlider;
    public Image[] sensors;
    public Color sensorOnColor;
    public Color sensorOffColor;

    private void Awake() {
        if (sng == null) sng = this;
        else {
            Destroy(sng);
            sng = this;
        }
        Initialize();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) F1SLink.sng.QuitGameSession();
    }

    public void SliderDepthSensorUpdate() {
        Player.sng.UpdateDepth((int) sensorSlider.value);
    }

    public void ButtonPauseClicked() {
        pauseScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ButtonResumeClicked() {
        pauseScreen.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ButtonRestartClicked() {
        Destroy(Consingletone.sng.gameObject);
        SceneManager.LoadScene(1);
    }
    
    public void ButtonMainMenuClicked() {
        Destroy(Consingletone.sng.gameObject);
        SceneManager.LoadScene(0);
    }

    public void ButtonQuitClicked() {
        F1SLink.sng.QuitGameSession();
    }

    public void EnableDepthSensor(int i, bool b) {
        sensors[i].color = b ? sensorOnColor : sensorOffColor;
    }

    private void Initialize() {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Time.timeScale = 1f;
        InitialiseSensorVisuals();
    }

    private void InitialiseSensorVisuals() {
        for (int i = 0; i < sensors.Length; i++)
            if (i < sensorSlider.value)
                EnableDepthSensor(i, true);
            else EnableDepthSensor(i, false);
    }

    public void UpdateScore(int score) {
        scoreText.text = score.ToString();
    }
}