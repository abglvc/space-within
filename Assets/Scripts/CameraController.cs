using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    private Transform target;
    private Vector3 newPosition;
    public float speed;
    public float xMin;
    public static float aspectMultiplier;

    void Awake() {
        aspectMultiplier = GetComponent<Camera>().aspect / (16f / 9);
    }

    void Start() {
        target = Player.sng.transform;
        if (target)
            transform.position = new Vector3(Mathf.Max(xMin, target.transform.position.x + 5f), transform.position.y,
                transform.position.z);
    }

    void FixedUpdate() {
        if (target) {
            newPosition = new Vector3(Mathf.Max(xMin, target.transform.position.x + 5f), transform.position.y,
                transform.position.z);
            transform.position = Vector3.Lerp(transform.position, newPosition, speed * Time.deltaTime);
        }
    }

    public Transform Target {
        set { target = value; }
    }
}