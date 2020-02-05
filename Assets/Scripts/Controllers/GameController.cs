using MyObjectPooling;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
    public static GameController sng { get; private set; } //singletone
    public GameObject consingletonePrefab;
    public Transform obstacleHeap;
    public GameObject startObstaclePack, endObstaclePack;
    public ObstaclePackPool[] obstaclePacksPool;
    public float distanceUntilNextPlanet;
    public int nextPlanet;
    
    private Player player;
    private GameObject consingletone;
    private float nextObstacleX = 0f;
    private bool endReached = false;
    private Rigidbody2D rb;

    private void Awake() {
        if (sng == null) sng = this;
        else {
            Destroy(sng);
            sng = this;
        }
        Initialize();
    }

    private float distanceTraveled;
    void Update() {
        distanceTraveled = player.DistanceTraveled();
        if (distanceTraveled + 15f > nextObstacleX) SpawnObstaclePack();
    }

    private void Initialize() {
        if (Consingletone.sng == null) {
            consingletone = Instantiate(consingletonePrefab, Vector3.zero, new Quaternion());
            DontDestroyOnLoad(consingletone);
        }
        player = Player.sng;
        
        player.previousDistance = 0;
        player.transform.position=new Vector3(3f,0,0);
        TrailRenderer[] trailRenderers = player.GetComponentsInChildren<TrailRenderer>();
        for (int i =0; i<trailRenderers.Length; i++)
            trailRenderers[i].GetComponentInChildren<TrailRenderer>().Clear();
        Camera.main.transform.position=new Vector3(10,0,-10);
        Camera.main.GetComponentInChildren<SpriteRenderer>().sortingOrder = -6;
    }

    private bool flipWalls = true;
    private bool previousEvenNumberPlatform = false;
    private void SpawnObstaclePack() {
        if (nextObstacleX < 0.5f) {
            GameObject obstaclePack = Instantiate(startObstaclePack, obstacleHeap);
            obstaclePack.transform.position = new Vector3(nextObstacleX + 20f / 2, 0, 0);
            nextObstacleX += 20f;
        } else if (!endReached && distanceTraveled > distanceUntilNextPlanet) {
            GameObject obstaclePack = Instantiate(endObstaclePack, obstacleHeap);
            obstaclePack.transform.position = new Vector3(nextObstacleX +  25f / 2, 0, 0);
            endReached = true;
        } else if (!endReached){
            ObstaclePack obstaclePack = obstaclePacksPool[Random.Range(0,obstaclePacksPool.Length)].GetOrSpawnIn(obstacleHeap);
            if (obstaclePack) {
                obstaclePack.Spawn(nextObstacleX, distanceTraveled/distanceUntilNextPlanet, flipWalls);
                previousEvenNumberPlatform = obstaclePack.evenNumberPlatforms;
                flipWalls = !flipWalls && !previousEvenNumberPlatform || flipWalls && previousEvenNumberPlatform;
                nextObstacleX += obstaclePack.width;
            }
        }
    }

    public void LoadNextPlanet() {
        Camera.main.GetComponentInChildren<SpriteRenderer>().sortingOrder = 6;
        SceneManager.LoadScene(nextPlanet);
    }
}