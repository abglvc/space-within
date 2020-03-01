using System;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclePack : MonoBehaviour {
    public float width;
    public float height;
    public bool evenNumberPlatforms;
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
    private PlanetSprites planetSprites;
    private int spawnedOnPlanet = -1;
    
    void Awake() {
        Initialize();
    }

    private void AttachObstacles() {
        bool flipTop = flipWalls;
        bool flipBot = flipWalls;
        
        for (int i=0; i<obstacleInfo.Count; i++) {
            Obstacle o = csg.GetObstacleFromPool(obstacleInfo[i].bluePrint.OBSTACLE_INDEX);
            
            if (o != null) {
                o.Spawn(obstaclePackId, obstacleInfo[i].transf);
                
                if (o.OBSTACLE_INDEX == 0) { o.GetComponent<SpriteRenderer>().flipX = flipTop;
                    flipTop = !flipTop;
                } else if (o.OBSTACLE_INDEX == 15) { o.GetComponent<SpriteRenderer>().flipX = flipBot;
                    flipBot = !flipBot;
                }
                //Debug.Log(difficulty);
                switch (o) {
                    case Projectile projectile:
                        Projectile proj = (Projectile) obstacleInfo[i].bluePrint;
                        projectile.SetStatesOnSpawn(proj, proj.moveDirection, 0f, difficulty, dangerSignal:true);
                        break;
                    case Portal portal:
                        portal.SetStatesOnSpawn(obstacleInfo[i].transf, (Portal)obstacleInfo[i].bluePrint);
                        break;
                    case Enemyhurt enemyhurt:
                        enemyhurt.SetStatesOnSpawn((Enemyhurt)obstacleInfo[i].bluePrint, difficulty);
                        break;
                    case Enemy enemy:
                        enemy.SetStatesOnSpawn((Enemy)obstacleInfo[i].bluePrint, difficulty);
                        break;
                    case RotaSpikes rotaSpikes:
                        rotaSpikes.SetStatesOnSpawn((RotaSpikes)obstacleInfo[i].bluePrint, difficulty);
                        break;
                    case Obstahurt obstahurt:
                        obstahurt.SetStatesOnSpawn((Obstahurt)obstacleInfo[i].bluePrint, difficulty);
                        break;
                    case PowerPickup powerPickup:
                        powerPickup.SetStatesOnSpawn((PowerPickup)obstacleInfo[i].bluePrint);
                        break;
                    case HealthPickup healthPickup:
                        healthPickup.SetStatesOnSpawn((HealthPickup)obstacleInfo[i].bluePrint);
                        break;
                }
                attachedObstacles[i] = o;
                
                if (o.PlanetSpawn != spawnedOnPlanet && planetSprites.IdToIndex.ContainsKey(o.OBSTACLE_INDEX)) {
                    SpriteRenderer sr = o.GetComponent<SpriteRenderer>();
                    Sprite skin = planetSprites.GetSkin(o.OBSTACLE_INDEX);
                    o.PlanetSpawn = spawnedOnPlanet;
                    if (sr) sr.sprite = skin;
                    else {
                        SpriteRenderer[] srs = o.GetComponentsInChildren<SpriteRenderer>();
                        for (int k = 0; k < srs.Length; k++) srs[k].sprite = skin;
                    }
                }
            }
        }
    }
    
    private void DeactivateAttachedObstacles() {
        foreach (var o in attachedObstacles)
            if (o != null && obstaclePackId==o.CallerId && o.ActiveObstacle) o.ActiveObstacle = false;
    }
    //flip top/bot walls
    private bool flipWalls;
    public void Spawn(float nextObstacleX, float difficulty, bool flipWalls) {
        if (spawnedOnPlanet != GameController.sng.thisPlanet) {
            spawnedOnPlanet = GameController.sng.thisPlanet;
            planetSprites = Consingletone.sng.planetsSkins[spawnedOnPlanet-1];
        }
        transform.position = new Vector3(nextObstacleX + width / 2, 0, 0);
        this.difficulty = difficulty/2f; //0<difficulty<0.5f
        this.flipWalls = flipWalls;
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
            child.gameObject.SetActive(false); //(krijem ih i nosim njihov info) ih jer su mi potrebni samo da preuzmem info, a postoje in a first place radi vizualizacije paketa
        }
        
        attachedObstacles=new List<Obstacle>(new Obstacle[obstacleInfo.Count]);
    }
}