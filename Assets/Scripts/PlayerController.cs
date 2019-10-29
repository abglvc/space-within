using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

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
    }

    // Update is called once per frame
    void Update() {
        if (health > 0 && (Input.GetKeyDown(KeyCode.DownArrow) && rb.gravityScale < 0 && topOriented ||
                           Input.GetKeyDown(KeyCode.UpArrow) && rb.gravityScale > 0 && !topOriented))
            rb.gravityScale *= -1;
        UserInterface.sng.UpdateScore(Mathf.FloorToInt(transform.position.x));
    }

    public void OnGravityDirectionButton(int k) {
        if (health > 0 && (k == 1 && rb.gravityScale < 0 && topOriented) ||
            k == -1 && rb.gravityScale > 0 && !topOriented)
            rb.gravityScale *= -1;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (health > 0) {
            if (other.collider.CompareTag("Obstacle")) {
                Vector2 pointDifference = other.GetContact(0).point-(Vector2)transform.position;
                if(Mathf.Abs(pointDifference.y)>0.2f) topOriented = pointDifference.y > 0;
                if (Mathf.Abs(pointDifference.x) < 0.25f) rb.velocity = Vector2.right * speed;
            }
            else if (other.collider.CompareTag("Obstahurt")) {
                Health -= other.gameObject.GetComponentInParent<Obstahurt>().damage;
                Debug.Log("IT HITS YOU");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) { //Vracanje prepreka u pool!
        if (other.gameObject.CompareTag("ObstaclePack") && other.isTrigger) {
            other.GetComponent<ObstaclePack>().ActiveObstaclePack = false;
        }
    }

    public void Initialize() {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityCoeff;
        topOriented = true;
        rb.velocity = Vector2.right * speed;
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