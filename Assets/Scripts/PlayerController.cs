using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public static PlayerController sng { get; private set; } //singletone
    public int health;

    public float gravityCoeff;
    public float speed;
    public bool topOriented;


    private Rigidbody2D rb;

    private void Awake() {
        if (sng == null) sng = this;
        else {
            Destroy(sng);
            sng = this;
        }
    }

    // Start is called before the first frame update
    void Start() {
        Initialize();
        rb.velocity = Vector2.right * speed;
    }

    // Update is called once per frame
    void Update() {
        if (health > 0 && (Input.GetKeyDown(KeyCode.DownArrow) && rb.gravityScale < 0 && topOriented ||
                           Input.GetKeyDown(KeyCode.UpArrow) && rb.gravityScale > 0 && !topOriented))
            rb.gravityScale *= -1;
        UserInterface.sng.UpdateScore(Mathf.FloorToInt(transform.position.x));
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (health > 0) {
            if (other.collider.CompareTag("Obstacle")) {
                topOriented = other.GetContact(0).point.y > transform.position.y;
                rb.velocity = Vector2.right * speed;
            }
            else if (other.collider.CompareTag("Obstahurt")) {
                Health -= other.gameObject.GetComponentInParent<Obstahurt>().damage;
                Debug.Log("HIT");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Obstacle") && other.isTrigger) {
            other.GetComponent<Obstacle>().ActiveObstacle = false;
        }
    }

    public void Initialize() {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityCoeff;
        topOriented = true;
    }

    public int Health {
        get => health;
        set {
            health = value;

            if (health <= 0) {
                //GAMEOVER
            }
        }
    }
}