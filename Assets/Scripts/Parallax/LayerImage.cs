using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerImage : MonoBehaviour {
    public float ppu = 16.0f; //pixels per unit
    private bool activeLayerImage = false;
    private float width;
    private void Awake() {
        Initialize();
    }

    private void Initialize() {
        Transform child = transform.GetChild(0);
        width = child.GetComponent<SpriteRenderer>().sprite.texture.width / ppu;
        child.localPosition = new Vector3(width / 2, 0f, 0f);
        BoxCollider2D bc=GetComponent<BoxCollider2D>();
        CameraController cc = Camera.main.GetComponent<CameraController>();
        bc.offset = new Vector2(width + cc.rightCameraOffset * (Camera.main.aspect / 1.7777f + 1.5f / cc.speed) + 10f, 0f);
        bc.size = new Vector2(20f, 20f);
    }
    
    public bool ActiveLayerImage {
        get => activeLayerImage;
        set {
            activeLayerImage = value;
            gameObject.SetActive(activeLayerImage);
        }
    }

    public float Width => width;
}
