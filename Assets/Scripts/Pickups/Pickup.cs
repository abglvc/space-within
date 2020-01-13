using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pickup : Obstacle {
    protected Transform playerTransform;
    private bool pickedUp = false;

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

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) pickedUp = true;
    }
}