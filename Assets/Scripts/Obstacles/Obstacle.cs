using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class Obstacle : MonoBehaviour {
    public int obstacleIndex;
    protected bool activeObstacle;
    protected int callerId;
    
    public void Spawn(int callerId, Vector3 position, Quaternion rotation) {
        this.callerId = callerId;
        transform.position = position;
        transform.rotation = rotation;
        ActiveObstacle = true;
    }

    public bool ActiveObstacle {
        get => activeObstacle;
        set {
            activeObstacle = value;
            gameObject.SetActive(activeObstacle);
        }
    }

    public int CallerId {
        get => callerId;
    }
}