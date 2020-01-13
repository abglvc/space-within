using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour {
    public static UserInterface sng { get; private set; } //singletone
    public Text scoreText;
    public GameObject deathScreen;
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
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Unload();
            //F1SLink.sng.ShowMainActivity(); //Application.Unload();    BILO JE Quit() do prije komentara
    }

    public void SliderDepthSensorUpdate() {
        Player.sng.UpdateDepth((int) sensorSlider.value);
    }

    public void ButtonRestartClicked() {
        Destroy(GameController.sng.Consingletone);
        SceneManager.LoadScene(1);
    }

    public void ButtonMainMenuClicked() {
        Destroy(GameController.sng.Consingletone);
        SceneManager.LoadScene(0);
    }

    public void EnableDepthSensor(int i, bool b) {
        sensors[i].color = b ? sensorOnColor : sensorOffColor;
    }

    private void Initialize() {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
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