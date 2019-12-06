using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class UnitProjectile : Projectile {
    public float timeAlive;

    private IEnumerator Destruct() {
        yield return new WaitForSeconds(timeAlive);
        ActiveObstacle = false;
    }

    private new void OnCollisionEnter2D(Collision2D other) {
        base.OnCollisionEnter2D(other);
        if (other.gameObject.CompareTag("Obstahurt")) {
            Obstahurt o = other.gameObject.GetComponentInParent<Obstahurt>();
            o.Health -= powerDamage;
        }
    }

    public void Spawn(int callerId, Vector3 position, Vector3 direction) {
        base.Spawn(callerId, position, direction);
        StartCoroutine(Destruct());
    }
}
