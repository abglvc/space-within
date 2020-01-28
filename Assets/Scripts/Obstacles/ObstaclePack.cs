using System;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclePack : MonoBehaviour {
    public float width;
    public float height;
    [Serializable]
    public struct ObstacleInfo { //transform and obstacle in game controller
        public Transform transf;
        public Obstacle bluePrint;
    }
    private List<ObstacleInfo> obstacleInfo = new List<ObstacleInfo>();

    private List<Obstacle> attachedObstacles;
    private bool activeObstaclePack=true;
    private Consingletone csg;
    private Vector3 portalDestinationPosition; //=> Max one portal per Obstacle Pack
    private int obstaclePackId;

    private float difficulty;
    
    void Awake() {
        Initialize();
    }

    private void AttachObstacles() {
        for (int i=0; i<obstacleInfo.Count; i++) {
            Obstacle o = csg.GetObstacleFromPool(obstacleInfo[i].bluePrint.OBSTACLE_INDEX);
            if (o != null) {
                o.Spawn(obstaclePackId, obstacleInfo[i].transf);
                //Debug.Log(difficulty);
                switch (o) {
                    case Projectile projectile:
                        projectile.SetStatesOnSpawn((Projectile)obstacleInfo[i].bluePrint, 0f, difficulty);
                        break;
                    case Portal portal:
                        portal.SetStatesOnSpawn(obstacleInfo[i].transf, (Portal)obstacleInfo[i].bluePrint);
                        break;
                    case Enemy enemy:
                        enemy.SetStatesOnSpawn(obstacleInfo[i].transf, (Enemy)obstacleInfo[i].bluePrint, difficulty);
                        break;
                    case RotaSpikes rotaSpikes:
                        rotaSpikes.SetStatesOnSpawn((RotaSpikes)obstacleInfo[i].bluePrint, difficulty);
                        break;
                    case Obstahurt obstahurt:
                        obstahurt.SetStatesOnSpawn((Obstahurt)obstacleInfo[i].bluePrint, difficulty);
                        break;
                }
                attachedObstacles[i] = o;
            }
        }
    }

    private void DeactivateAttachedObstacles() {
        foreach (var o in attachedObstacles)
            if (o != null && obstaclePackId==o.CallerId && o.ActiveObstacle) o.ActiveObstacle = false;
    }

    public void Spawn(float nextObstacleX, float difficulty) {
        transform.position = new Vector3(nextObstacleX + width / 2, 0, 0);
        this.difficulty = difficulty;
        ActiveObstaclePack = true;
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
        csg = Consingletone.sng;
        obstaclePackId = GetInstanceID();
        
        foreach (Transform child in transform) {
            ObstacleInfo opi;
            opi.transf = child.transform;
            opi.bluePrint = child.GetComponentInChildren<Obstacle>();
            obstacleInfo.Add(opi);
            child.GetChild(0).gameObject.SetActive(false); //(krijem ih i nosim njihov info) ih jer su mi potrebni samo da preuzmem info, a postoje in a first place radi vizualizacije paketa
        }
        
        attachedObstacles=new List<Obstacle>(new Obstacle[obstacleInfo.Count]);
    }
}