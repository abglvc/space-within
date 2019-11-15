using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class ParallaxLayer : MonoBehaviour {
    public int layerIndex;
    public float stickyFactor;    //0-1
    private float startX;
    
    private float width;
    private Transform lockOn;
    private bool activeParallaxLayer=true;
    private bool spawnedNext = false;

    void Start() {
        Initialize();
    }

    void Update() {
        if (lockOn) {
            transform.position = new Vector3(startX+stickyFactor*(lockOn.position.x-startX+10), 0, 0);
            float distance = lockOn.position.x - transform.position.x;
            if (!spawnedNext) {
                if (distance+10 > width) spawnedNext = Parallax.sng.CreateNew(layerIndex, transform.position.x + width);
            }
            if(distance-10 > width) ActiveParallaxLayer = false;
        }
    }

    public bool ActiveParallaxLayer {
        get => activeParallaxLayer;
        set {
            activeParallaxLayer = value;
            spawnedNext = !value;
            gameObject.SetActive(activeParallaxLayer);
        }
    }

    public float StartX {
        get => startX;
        set => startX = value;
    }

    private void Initialize() {
        lockOn = Camera.main.transform;
        width = GetComponentInChildren<SpriteRenderer>().sprite.texture.width / 16.0f;
    }
}
