using System.Collections;
using UnityEngine;

public class Projectile : Obstahurt {
    [Header("Projectile")]
    public float speed;
    public Vector3 moveDirection;
    public float timeAlive;

    protected Rigidbody2D rb;
    private TrailRenderer tr;

    protected new void Awake() {
        Initialize();
        base.Awake();
    }

    private IEnumerator Destruct() {
        yield return new WaitForSeconds(timeAlive);
        ActiveObstacle = false;
    }

    private new void Initialize() {
        base.Initialize();
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponentInChildren<TrailRenderer>();
    }

    public void SetStatesOnSpawn(Projectile bluePrint, float initialSpeed, float difficulty) {
        if (bluePrint.moveDirection.x < 0) UserInterface.sng.SignalDanger(bluePrint.transform.position.y);
        moveDirection = bluePrint.moveDirection;
        speed = bluePrint.speed * (1f + difficulty);
        rb.velocity = moveDirection * (initialSpeed + speed);
        timeAlive = bluePrint.timeAlive;
        if(tr) tr.Clear();
        StartCoroutine(Destruct());
        base.SetStatesOnSpawn(bluePrint, difficulty);
    }
}