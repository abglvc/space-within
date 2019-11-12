using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class ParallaxLayer : MonoBehaviour {
    public int layerIndex;
    public float stickyFactor;    //0-1
    public float width;
    
    private Transform lockOn;
    private bool activeParallaxLayer=true;
    
    void Start() {
        lockOn = Camera.main.transform;
    }

    void Update() {
        if (lockOn) {
            transform.position = new Vector3(stickyFactor*lockOn.position.x, 0, 0);
            if (Vector3.Distance(transform.position, lockOn.position) > width/2) ActiveParallaxLayer = false;
        }
        
    }

    public bool ActiveParallaxLayer {
        get => activeParallaxLayer;
        set {
            activeParallaxLayer = value;
            gameObject.SetActive(activeParallaxLayer);
            if (!activeParallaxLayer) Parallax.sng.CreateNew(layerIndex, transform.position);
        }
    }
}
