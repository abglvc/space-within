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
    private List<TI> obstaclePlaceIndex = new List<TI>();

    private List<Obstacle> attachedObstacles;
    private bool activeObstaclePack=true;
    private GameController gc;
    private Vector3 portalDestinationPosition; //=> Max one portal per Obstacle Pack
    private int obstaclePackId;
    
    void Awake() {
        Initialize();
    }

    private void AttachObstacles() {
        for (int i=0; i<obstaclePlaceIndex.Count; i++) {
            Obstacle o = gc.obstaclesPool[obstaclePlaceIndex[i].index].Get(gc.obstacleHeap);
            if (o != null) {
                if (o is Projectile) (o as Projectile).Spawn(obstaclePackId, obstaclePlaceIndex[i].transf.position, Vector2.left);
                else if(o is Portal) (o as Portal).Spawn(obstaclePackId, obstaclePlaceIndex[i].transf.position, portalDestinationPosition);
                else o.Spawn(obstaclePackId, obstaclePlaceIndex[i].transf.position, obstaclePlaceIndex[i].transf.rotation);
                attachedObstacles[i] = o;
            }
        }
    }

    private void DeactivateAttachedObstacles() {
        foreach (var o in attachedObstacles)
            if (o != null && obstaclePackId==o.CallerId && o.ActiveObstacle) o.ActiveObstacle = false;
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
        bc.offset=new Vector2(width/2+10f+10f, 0f);
        bc.size=new Vector2(20f, height+2f);
        gc = GameController.sng;

        obstaclePackId = GetInstanceID();
        
        foreach (Transform child in transform) {
            TI opi;
            opi.transf = child.transform;
            Obstacle obstacle = child.GetComponentInChildren<Obstacle>();
            opi.index = obstacle.obstacleIndex;
            if (obstacle is Portal) portalDestinationPosition = (obstacle as Portal).destination.localPosition;
            obstaclePlaceIndex.Add(opi);
            Destroy(child.GetChild(0).gameObject); //unistavam ih jer su mi potrebni samo da preuzmem info, a postoje in a first place radi vizualizacije paketa
        }
        
        attachedObstacles=new List<Obstacle>(new Obstacle[obstaclePlaceIndex.Count]);
    }
}