using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : Obstacle {
    public Transform destination;
    public float traveltime;
    
    private Quaternion qrotation;
    
    void Start() {
        Initialize();
    }

    private void Initialize() {
        qrotation = transform.rotation;
    }

    public void Spawn(int obstPackId, Vector3 position, Vector3 destinationPosition) {
        base.Spawn(obstPackId, position, qrotation);
        destinationPosition.Scale(transform.localScale);
        destination.position = position + destinationPosition; //destinationPosition je localPosition
    }

    private IEnumerator PortalTravel(GameObject o) {
        o.SetActive(false);
        yield return new WaitForSeconds(traveltime);
        o.transform.position = destination.position;
        o.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.gameObject.CompareTag("PlayerProjectile"))
            StartCoroutine(PortalTravel(other.gameObject));
        else {
            other.gameObject.GetComponent<UnitProjectile>().ActiveObstacle = false;
        }
    }
}
