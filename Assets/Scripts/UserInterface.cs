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
    
    private void Awake() {
        if (sng == null) sng = this;
        else {
            Destroy(sng);
            sng = this;
        }
    }

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
    }

    public void UpdateScore(int score) {
        scoreText.text = score.ToString();
    }

    public void UpdateHealth(int health) {
        healthText.text = health.ToString();
    }
}