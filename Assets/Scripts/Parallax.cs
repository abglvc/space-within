using System.Collections;
using System.Collections.Generic;
using MyObjectPooling;
using UnityEngine;

public class Parallax : MonoBehaviour {
    public static Parallax sng { get; private set; } //singletone

    public SpriteRenderer bgSpriteRenderer;
    public ParallaxLayerPool[] parallaxLayersPool;

    private void Awake() {
        if (sng == null) sng = this;
        else {
            Destroy(sng);
            sng = this;
        }
    }
    
    void Start(){
        Initialize();
    }

    void Update(){
        
    }

    public void Initialize() {
        AdjustBacgroundImage();
        for (int i = 0; i < parallaxLayersPool.Length; i++) CreateNew(i, 0);
    }

    public bool CreateNew(int index, float startx) {
        ParallaxLayer pL = parallaxLayersPool[index].GetFromPool(transform);
        if (pL) {
            pL.StartX = startx;
            pL.ActiveParallaxLayer = true;
            return true;
        }
        return false;
    }

    private void AdjustBacgroundImage() {
        //BACGROUND IMAGE SET
        float cameraHeight = Camera.main.orthographicSize * 2;
        Vector2 cameraSize = new Vector2(Camera.main.aspect * cameraHeight, cameraHeight);
        Vector2 spriteSize = bgSpriteRenderer.sprite.bounds.size;
        
        Vector2 scale = bgSpriteRenderer.transform.localScale;
        if (cameraSize.x >= cameraSize.y) { // Landscape (or equal)
            scale *= cameraSize.x / spriteSize.x;
        } else { // Portrait
            scale *= cameraSize.y / spriteSize.y;
        }
        
        //bgSpriteRenderer.transform.position = Vector2.zero; // Optional
        bgSpriteRenderer.transform.localScale = scale;
    }
}