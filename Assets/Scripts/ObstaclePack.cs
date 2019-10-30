using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclePack : MonoBehaviour {
    public float width;
    public float height;
    [Serializable]
    public struct TI { //transform and obstacle index in game controller
        public Transform transf;
        public int index;
    }
    public List<TI> obstaclePlaceIndex;

    private List<Obstacle> attachedObstacles;
    private bool activeObstaclePack=true;
    private GameController gc;
    
    void Awake() {
        Initialize();
    }

    private void AttachObstacles() {
        for (int i=0; i<obstaclePlaceIndex.Count; i++) {
            Obstacle o = gc.obstaclesPool[obstaclePlaceIndex[i].index].GetFromPool(gc.obstacleHeap);
            if (o != null) {
                o.transform.position = obstaclePlaceIndex[i].transf.position;
                o.transform.rotation = obstaclePlaceIndex[i].transf.rotation;
                o.ActiveObstacle = true;
                attachedObstacles[i] = o;
            }
        }
    }

    private void DeactivateAttachedObstacles() {
        foreach (var o in attachedObstacles)
            if (o != null) o.ActiveObstacle = false;
    }

    public bool ActiveObstaclePack {
        get => activeObstaclePack;
        set {
            activeObstaclePack = value;
            if (activeObstaclePack) AttachObstacles();
            else DeactivateAttachedObstacles();
            gameObject.SetActive(activeObstaclePack);
        }
    }

    private void Initialize() {
        BoxCollider2D bc=GetComponent<BoxCollider2D>();
        bc.offset=new Vector2(width/2+10f, 0f);
        bc.size=new Vector2(1f, height+2f);
        gc=GameController.sng;
        attachedObstacles=new List<Obstacle>(new Obstacle[obstaclePlaceIndex.Count]);
    }
}