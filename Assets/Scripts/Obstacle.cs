using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Obstacle : MonoBehaviour {
    public float width;
    protected bool activeObstacle=true;
    
    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        
    }

    public bool ActiveObstacle {
        get => activeObstacle;
        set {
            activeObstacle = value;
            gameObject.SetActive(activeObstacle);
        }
    }
}