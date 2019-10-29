using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclePack : MonoBehaviour {
    public float width;
    public float height;
    public List<TI> obstaclePlaceIndex;
    [Serializable]
    public struct TI { //transform and obstacle index in game controller
        public Transform transf;
        public int index;
    }
    
    private bool activeObstaclePack=true;
    
    
    void Start() {
        Initialize();
    }
    
    public bool ActiveObstaclePack {
        get => activeObstaclePack;
        set {
            activeObstaclePack = value;
            gameObject.SetActive(activeObstaclePack);
        }
    }

    private void Initialize() {
        BoxCollider2D bc=GetComponent<BoxCollider2D>();
        bc.offset=new Vector2(width+10f, 0f);
        bc.size=new Vector2(1f, height+2f);
    }
}