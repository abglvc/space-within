using MyObjectPooling;
using UnityEngine;

public class Enemyhurt : Enemy {
    [Header("Enemyhurt")] 
    public float attackRange;
    public float attackRecharge;
    public ObstaclePool projectile;
    public Transform shootSource;
    
    private Transform playerTransform;
    private float nextShot = 0f;
    private Transform obstacleHeap;
    private int enemyId;
    private float difficulty;
    private Projectile projectileBluePrint;

    protected new void Awake() {
        Initialize();
        base.Awake();
    }
    
    protected new void FixedUpdate() {
        if (playerTransform == null) {
            if(rotatingPart!=null) LookAt(goToPosition);
            Move();
        } else {
            rb.velocity=Vector2.zero;
            if(rotatingPart!=null) LookAt(playerTransform.position);
            if (Time.time > nextShot) {
                ShootPlayer();
                nextShot = Time.time + attackRecharge;
            }
        }
    }

    private void ShootPlayer() {
        Projectile p = projectile.GetOrSpawnIn(obstacleHeap) as Projectile;
        if (p) {
            p.Spawn(enemyId, shootSource);
            p.SetStatesOnSpawn(projectileBluePrint, (playerTransform.position-transform.position).normalized ,0, difficulty);
            p.ActiveObstacle = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (col.CompareTag("Player")) {
            playerTransform = col.transform;
        }
    }

    public new void SetStatesOnSpawn(Transform transformInfo, Enemy bluePrint, float difficulty) {
        playerTransform = null;
        this.difficulty = difficulty;
        base.SetStatesOnSpawn(transformInfo, bluePrint, difficulty);
    }
    
    protected new void Initialize() {
        projectileBluePrint = projectile.objectPrefab as Projectile;
        enemyId = GetInstanceID();
        obstacleHeap = GameController.sng.obstacleHeap;
        GetComponent<CircleCollider2D>().radius = attackRange;
    }
}
