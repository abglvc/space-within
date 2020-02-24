using System.Collections;
using UnityEngine;

public class Projectile : Obstahurt {
    [Header("Projectile")]
    public float speed;
    public Vector3 moveDirection;
    public float timeAlive;

    public int rotationSpeed;
    public Transform rotatingPart;

    public AudioClip soundEffect;
    
    protected Rigidbody2D rb;
    private TrailRenderer tr;

    protected new void Awake() {
        Initialize();
        base.Awake();
    }
    
    void Update() {
        if(rotationSpeed != 0)
            rotatingPart.Rotate (0,0,rotationSpeed*Time.deltaTime);
    }

    private IEnumerator Destruct() {
        yield return new WaitForSeconds(timeAlive);
        ActiveObstacle = false;
    }

    private new void Initialize() {
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponentInChildren<TrailRenderer>();
        if (rotatingPart == null) rotatingPart = transform;
    }
    
    public void SetStatesOnSpawn(Projectile bluePrint, Vector3 dir, float initialSpeed, float difficulty, int newPowerDamage=0, bool dangerSignal=false) {
        if (dangerSignal) {
            UserInterface.sng.SignalDanger(bluePrint.transform.position.y);
        }
        moveDirection = dir;
        speed = bluePrint.speed * (1f + difficulty);
        rb.velocity = moveDirection * (initialSpeed + speed);
        timeAlive = bluePrint.timeAlive;
        if(tr) tr.Clear();
        StartCoroutine(Destruct());
        base.SetStatesOnSpawn(bluePrint, difficulty, newPowerDamage);
    }
}