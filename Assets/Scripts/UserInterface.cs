﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour {
    public static UserInterface sng { get; private set; } //singletone
    public Text scoreText;
    public Text healthText;
    public GameObject ripSplash;
    public GameObject damageSplash;
    public Slider sensorSlider;
    public Image[] sensors;
    public Color sensorOnColor;
    public Color sensorOffColor;

    private int previousDepth; //premjestiti u drugu klasu
    
    private void Awake() {
        if (sng == null) sng = this;
        else {
            Destroy(sng);
            sng = this;
        }
    }

    // Start is called before the first frame update
    void Start() {
        Initialize();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) 
            Application.Quit(); 
    }

    public void SliderDepthSensorUpdate() {
        Player.sng.UpdateDepth((int)sensorSlider.value);
    }

    public void EnableDepthSensor(int i, bool b) {
        sensors[i].color = b ? sensorOnColor : sensorOffColor;
    }

    private void Initialize() {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        InitialiseSensorVisuals();
    }

    private void InitialiseSensorVisuals() {
        previousDepth = (int)sensorSlider.value;
        for(int i=0; i<sensors.Length; i++)
            if (i < sensorSlider.value)
                EnableDepthSensor(i, true);
            else EnableDepthSensor(i, false);
    }

    public void UpdateScore(int score) {
        scoreText.text = score.ToString();
    }

    public void UpdateHealth(int health) {
        healthText.text = health.ToString();
    }
}