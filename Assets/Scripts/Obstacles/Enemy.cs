using System.Collections.Generic;
using UnityEngine;

public class Enemy : Obstahurt {
    [Header("Enemy")]
    public float speed;
    public enum MotionType {
        Closed, Open
    }
    public MotionType motionType;
    private bool forward = true;
    
    private LinkedList<Vector3> movementPathPoints = new LinkedList<Vector3>();
    private LinkedListNode<Vector3> currentPP;
    private Vector3 spawnPosition, goToPosition;
    private Rigidbody2D rb;

    protected new void Awake() {
        Initialize();
        base.Awake();
    }

    private void FixedUpdate() {
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
        else rb.AddForce((goToPosition-transform.position)*speed);
    }
    
    public void SetStatesOnSpawn(Transform transformInfo, Enemy bluePrint, float difficulty) {
        spawnPosition = transformInfo.position;
        goToPosition = spawnPosition;
        motionType = bluePrint.motionType;
        speed = bluePrint.speed * (1f + difficulty);
        movementPathPoints = bluePrint.MovementPathPoints;
        currentPP = movementPathPoints.First;
        base.SetStatesOnSpawn(bluePrint, difficulty);
    }

    protected new void Initialize() {
        rb = GetComponent<Rigidbody2D>();
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
