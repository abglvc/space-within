using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Obstahurt {
    public float speed;
    
    private Rigidbody2D rb;
    private TrailRenderer tr;
    private Quaternion qrotation;

    private new void Awake() {
        base.Initialize();
        Initialize();
    }

    private void OnEnable() { // maybe
        base.Initialize();
    }

    private new void Initialize() {
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<TrailRenderer>();
        qrotation = transform.rotation;
    }

    public void Spawn(int callerId, Vector3 position, Vector2 moveDirection) {
        if(tr) tr.emitting = false;
        base.Spawn(callerId, position, qrotation);
        rb.velocity = moveDirection * speed;
        if(tr) tr.emitting = true;
    }
}
