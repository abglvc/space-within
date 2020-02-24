using MyObjectPooling;
using UnityEngine;

public class Enemyhurt : Enemy {
    [Header("Enemyhurt")] 
    public float attackRange;
    public float attackRecharge;
    public Transform shootSource;
    public Projectile projectileBluePrint;
    
    private Consingletone csg;
    private Transform playerTransform;
    private float nextShot = 0f;
    private int enemyId;
    private float difficulty;

    private AudioSource mainAudioSource;

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
        Projectile p = csg.obstaclesPool[projectileBluePrint.OBSTACLE_INDEX].GetOrSpawnIn(transform.parent) as Projectile;
        mainAudioSource.clip = p.soundEffect;
        if (p) {
            mainAudioSource.Play();
            p.Spawn(enemyId, shootSource);
            p.SetStatesOnSpawn(projectileBluePrint, (playerTransform.position-transform.position).normalized ,0, difficulty, powerDamage);
            p.ActiveObstacle = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (col.CompareTag("Player")) {
            playerTransform = col.transform;
        }
    }

    public new void SetStatesOnSpawn(Enemy bluePrint, float difficulty) {
        playerTransform = null;
        this.difficulty = difficulty;
        base.SetStatesOnSpawn(bluePrint, difficulty);
    }
    
    protected new void Initialize() {
        csg = Consingletone.sng;
        enemyId = GetInstanceID();
        GetComponent<CircleCollider2D>().radius = attackRange;
        mainAudioSource = GetComponent<AudioSource>();
    }
}
