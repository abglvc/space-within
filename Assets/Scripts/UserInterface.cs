using System.Collections;
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
        InitialiseSensorVisuals();
    }

    // Update is called once per frame
    void Update() {
    }

    public void VisualSensorUpdate() {
        int sensVal = (int)sensorSlider.value;
        PlayerController.sng.OnGravityDirectionChange((int) Mathf.Sign(previousDepth-sensVal));
        for(int i=Mathf.Min(previousDepth, sensVal); i<Mathf.Max(previousDepth, sensVal); i++)
            if (previousDepth <= sensVal)
                sensors[i].color = sensorOnColor;
            else sensors[i].color = sensorOffColor;
        previousDepth = sensVal;
    }

    private void InitialiseSensorVisuals() {
        previousDepth = (int)sensorSlider.value;
        for(int i=0; i<sensors.Length; i++)
            if (i < sensorSlider.value)
                sensors[i].color = sensorOnColor;
            else sensors[i].color = sensorOffColor;
    }

    public void UpdateScore(int score) {
        scoreText.text = score.ToString();
    }

    public void UpdateHealth(int health) {
        healthText.text = health.ToString();
    }
}