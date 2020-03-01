using System.Collections.Generic;
using UnityEngine;


public class Enemy : Obstahurt {
    [Header("Enemy")] public float speed;

    public enum MotionType {
        Closed,
        Open
    }

    public MotionType motionType;
    public Transform rotatingPart;

    private bool forward = true;
    private LinkedList<Vector3> movementPathPoints = new LinkedList<Vector3>();
    private LinkedListNode<Vector3> currentPP;
    protected Vector3 spawnPosition, goToPosition;
    protected Rigidbody2D rb;

    protected new void Awake() {
        Initialize();
        base.Awake();
    }

    protected void FixedUpdate() {
        Move();
        if(rotatingPart!=null)
            LookAt(goToPosition);
    }

    protected void Move() {
        if (Vector3.Distance(goToPosition, transform.position) < 0.5f) {
            switch (motionType) {
                case MotionType.Open:
                    LinkedListNode<Vector3> current = currentPP;
                    if(forward) currentPP = currentPP.Next!=null ? currentPP.Next : currentPP.Previous;
                    else currentPP = currentPP.Previous!=null ? currentPP.Previous : currentPP.Next;
                    forward = current.Next == currentPP;
                    break;
                case MotionType.Closed:
                    currentPP = currentPP.Next!=null ? currentPP.Next : movementPathPoints.First;
                    break;
            }
            goToPosition = spawnPosition + currentPP.Value;
            rb.velocity = Vector2.zero;
        }
        else rb.AddForce((goToPosition-transform.position).normalized*speed);
    }
    
    protected void LookAt(Vector3 target) {
        Vector3 dir = target - rotatingPart.position;
        float angle = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg;
        rotatingPart.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void SetStatesOnSpawn(Enemy bluePrint, float difficulty) {
        motionType = bluePrint.motionType;
        speed = bluePrint.speed * (1f + difficulty);
        SetMovementPathPoints(bluePrint.MovementPathPoints);
        base.SetStatesOnSpawn(bluePrint, difficulty);
    }

    protected new void Initialize() {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetMovementPathPoints(LinkedList<Vector3> movementPP) {
        spawnPosition = transform.position;
        goToPosition = spawnPosition;
        movementPathPoints = movementPP;
        currentPP = movementPathPoints.First;
    }

    public LinkedList<Vector3> MovementPathPoints {
        get {
            movementPathPoints.Clear();
            foreach(Transform t in transform)
                movementPathPoints.AddLast(t.localPosition);
            return movementPathPoints;
        }
    }
}
