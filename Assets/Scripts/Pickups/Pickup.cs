using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class Pickup : Obstacle {
    protected Transform playerTransform;
    private bool pickedUp = false;

    protected Text labelValueText;

    //assume da svaki pickup ima neki label
    private void Awake() {
        Initialize();
    }

    private void Start() {
        playerTransform = Player.sng.transform;
    }

    void Update() {
        if (pickedUp) {
            if (Vector3.Distance(playerTransform.position, transform.position) < 0.5f) {
                pickedUp = false;
                playerTransform.GetComponent<Player>().CaughtPickup(this);
                ActiveObstacle = false;
            }
            else transform.position = Vector3.Lerp(transform.position, playerTransform.position, 0.55f);
        }
    }
    
    protected void Initialize() {
        labelValueText = GetComponentInChildren<Text>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) pickedUp = true;
    }
}