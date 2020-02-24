using System;
using MyObjectPooling;
using UnityEngine;

public abstract class GameController : MonoBehaviour {
    public static GameController sng { get; private set; } //singletone

    public String gameName = "Level";
    public int thisPlanet;
    public GameObject consingletonePrefab;
    public Transform obstaclePackHeap;
    public ObstaclePackPool[] obstaclePacksPool;

    protected DAO dao;
    protected float distanceTraveled;
    protected int currentObstPack = -1;
    protected Player player;
    private GameObject consingletone;
    private float nextObstacleX = 0f;
    private bool endReached = false;
    private bool flipWalls = true;
    private bool previousEvenNumberPlatform = false;

    private void Awake() {
        if (sng == null) sng = this;
        else {
            Destroy(sng);
            sng = this;
        }

        Initialize();
    }

    private void OnDisable() {
        dao.Save();
    }

    void Update() {
        distanceTraveled = player.DistanceTraveled();
        if (!endReached && distanceTraveled + 15f > nextObstacleX) SpawnObstaclePack();
    }

    private void Initialize() {
        dao = new DAO();
        if (Consingletone.sng == null) {
            consingletone = Instantiate(consingletonePrefab, Vector3.zero, new Quaternion());
            DontDestroyOnLoad(consingletone);
        }

        Camera.main.GetComponent<CameraController>()
            .AdjustBackgroundImage(Consingletone.sng.planetsSkins[thisPlanet - 1].planetBackground);
        player = Player.sng;

        player.previousDistance = 0;
        player.transform.position = new Vector3(3f, 0, 0);
        TrailRenderer[] trailRenderers = player.GetComponentsInChildren<TrailRenderer>();
        for (int i = 0; i < trailRenderers.Length; i++)
            trailRenderers[i].GetComponentInChildren<TrailRenderer>().Clear();
        Camera.main.transform.position = new Vector3(10, 0, -10);
        Camera.main.GetComponentInChildren<SpriteRenderer>().sortingOrder = -6;
    }

    private void SpawnObstaclePack() {
        if (nextObstacleX < 0.5f) {
            GameObject obstaclePack =
                Instantiate(Consingletone.sng.planetStartPlatforms[thisPlanet - 1], obstaclePackHeap);
            obstaclePack.transform.position = new Vector3(nextObstacleX + 20f / 2, 0, 0);
            nextObstacleX += 20f;
        }
        else if (Difficulty() >= 1f) {
            GameObject obstaclePack = Instantiate(Consingletone.sng.planetEndPlatform, obstaclePackHeap);
            obstaclePack.transform.position = new Vector3(nextObstacleX + 20f / 2, 0, 0);
            endReached = true;
        }
        else {
            int thisObstPack = NextObstaclePack();
            ObstaclePack obstaclePack =
                obstaclePacksPool[thisObstPack].GetOrSpawnIn(obstaclePackHeap);
            if (obstaclePack) {
                if(this is LevelGC) ((LevelGC)this).ConfirmObstPackSpawn();
                currentObstPack = thisObstPack;
                obstaclePack.Spawn(nextObstacleX, Difficulty(), flipWalls);
                previousEvenNumberPlatform = obstaclePack.evenNumberPlatforms;
                flipWalls = !flipWalls && !previousEvenNumberPlatform || flipWalls && previousEvenNumberPlatform;
                nextObstacleX += obstaclePack.width;
            }
        }
    }

    public abstract float Difficulty();

    public abstract int NextObstaclePack();

    public abstract int CalculateStars();
    
    public abstract void EndGame();
}