using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotaSpikes : DestructableObstacle {
    public int rotationSpeed;

    private new void Awake() {
        Initialize();
    }

    void Update() {
        transform.Rotate (0,0,rotationSpeed*Time.deltaTime);
    }
}