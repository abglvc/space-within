using System;
using System.Collections;
using System.Collections.Generic;
using MyObjectPooling;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour {
    public static Player sng { get; private set; } //singletone
    public int maxHealth;
    public float verticalSpeed;
    public float horizontalSpeed;
    public float recoveryTime;
    public float attackRecharge = 2f;
    public GameObject postDestructPrefab;
    public float postDestructTime;
    public ObstaclePool[] projectiles;
    
    [Header("UI Health")]
    public Text healthText;
    public Slider healthSlider;

    private int health;
    private int score = 0;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Collider2D col;
    private GameObject uiStats;
    private bool isRecovering = false;
    private int previousDepth = 0;
    private int projectileIndex = -1;
    private int playerId;
    
    private void Awake() {
        if (sng == null) sng = this;
        else {
            Destroy(sng);
            sng = this;
        }
        Initialize();
    }

    private float nextVelocityCheck = 0.5f; 
    [HideInInspector]
    public int previousDistance = 0;

    void Update() {
        if (health > 0) {
            if (Mathf.FloorToInt(DistanceTraveled()) > previousDistance) { // add distance to score
                int dist = Mathf.FloorToInt(DistanceTraveled());
                Score += dist - previousDistance;
                previousDistance = dist;
            }

            if (Input.GetKeyDown(KeyCode.DownArrow) && rb.gravityScale < 0 || Input.GetKeyDown(KeyCode.UpArrow) && rb.gravityScale > 0 )
                rb.gravityScale *= -1;
        
            if (Time.time > nextVelocityCheck) {
                if (previousDepth > 0 && (rb.velocity.x < 0 || rb.velocity.magnitude < horizontalSpeed / 2)) {
                    rb.velocity = Vector2.right * horizontalSpeed;
                }
                nextVelocityCheck += 0.5f;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (health > 0) {
            GameObject g = other.gameObject;
            if (g.CompareTag("Obstahurt") || g.CompareTag("ObstacleProjectile") || g.CompareTag("Enemy")) {
                Obstahurt oh = g.GetComponentInParent<Obstahurt>();
                if(oh==null) oh = g.GetComponentInChildren<Obstahurt>();
                if (oh) {
                    Health -= oh.powerDamage;
                    oh.UsedPower();
                }
            }
            if(previousDepth > 0) rb.velocity = Vector2.right * horizontalSpeed;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other) { //Vracanje u pool!
         switch (other.gameObject.tag) {
             case "ObstaclePack":
                 other.GetComponent<ObstaclePack>().ActiveObstaclePack = false;
                 break;
             case "ParallaxLayer":
                 other.GetComponent<LayerImage>().ActiveLayerImage = false;
                 break;
             case "EndPortal":
                 GameController.sng.LoadNextPlanet();
                 break;
             case "StartPack":
                 Destroy(other.gameObject);
                 break;
         }
    }

    public void Initialize() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        uiStats = GetComponentInChildren<RectTransform>().gameObject;

        health = maxHealth;
        rb.gravityScale = verticalSpeed;
        if(previousDepth > 0) rb.velocity = Vector2.right * horizontalSpeed;
        healthSlider.maxValue = Health;
        UpdateHealthUi();
        playerId = GetInstanceID();
    }

    public float DistanceTraveled() {
        return transform.position.x;
    }

/*
    private bool TopOriented() {
        RaycastHit2D rhUp = Physics2D.Raycast(transform.position, Vector2.up, 100f, raycastLayerMask);
        RaycastHit2D rhDown = Physics2D.Raycast(transform.position, Vector2.down, 100f, raycastLayerMask);
        return rhUp.distance < rhDown.distance;
    }
*/
    private Coroutine activeAutomaticShoot;
    
    private IEnumerator AutomaticShoot(int index) {
        projectileIndex = index;
        if (activeAutomaticShoot != null) {
            StopCoroutine(activeAutomaticShoot);
        }
        if(projectileIndex == -1) yield break;
        WaitForSeconds rechargeTime = new WaitForSeconds(attackRecharge);
        while (true) {
            Projectile p = projectiles[index].GetOrSpawnIn(transform.parent) as Projectile;
            if (p) {
                p.Spawn(playerId, transform);
                p.SetStatesOnSpawn(p, horizontalSpeed, 0f);
                p.ActiveObstacle = true;
            }
            yield return rechargeTime;
        }
    }

    public void InPortal(bool value) {
        if (value) {
            if (activeAutomaticShoot != null) {
                StopCoroutine(activeAutomaticShoot);
            }   
        } else activeAutomaticShoot = StartCoroutine(AutomaticShoot(projectileIndex));
        
        uiStats.SetActive(!value);
        rb.simulated = !value;
        col.enabled = !value;
        sr.enabled = !value;
    }

    public void CaughtPickup(Pickup pickup) {
        switch (pickup) {
            case PowerPickup powerPickup:
                activePowerPickup = StartCoroutine(ActivatePowerPickup(powerPickup.projectile, powerPickup.effectTime));
                break;
            case HealthPickup healthPickup:
                Health += healthPickup.healValue;
                break;
        }
    }

    private void Death() {
        if (postDestructPrefab) {
            GameObject destruct = Instantiate(postDestructPrefab, GameController.sng.obstacleHeap);
            destruct.transform.position = transform.position;
            Destroy(destruct, postDestructTime);
        }
        gameObject.SetActive(false);
        UserInterface.sng.deathScreen.SetActive(true);
    }

    public void UpdateDepth(int depth) {
        OnGravityDirectionChange((int) Mathf.Sign(previousDepth-depth));
        UserInterface ui=UserInterface.sng;
        for(int i=Mathf.Min(previousDepth, depth); i<Mathf.Max(previousDepth, depth); i++)
            if (previousDepth <= depth)
                ui.EnableDepthSensor(i, true);
            else ui.EnableDepthSensor(i, false);
        previousDepth = depth;
        if (depth == 0) rb.velocity = Vector2.zero;
    }

    public void OnGravityDirectionChange(int k) {
        if (health > 0 && (k == 1 && rb.gravityScale < 0 ) ||
            k == -1 && rb.gravityScale > 0 ) {
            rb.gravityScale *= -1;
            transform.localScale = new Vector3(1f, Mathf.Sign(rb.gravityScale), 1f);
        }
    }

    private IEnumerator DamageRecovery(float tickTime) {
        isRecovering = true;
        WaitForSeconds tick = new WaitForSeconds(tickTime);
        float tRecovery = recoveryTime;
        while (tRecovery > 0) {
            UserInterface.sng.damageSplash.SetActive(!UserInterface.sng.damageSplash.activeSelf);
            tRecovery -= tickTime;
            yield return tick;
        }
        isRecovering = false;
        UserInterface.sng.damageSplash.SetActive(false);
    }
    
    private IEnumerator Blink(float duration, Color color) {
        sr.color = color;
        yield return new WaitForSeconds(duration);
        sr.color = Color.white;
    }
    
    private Coroutine activePowerPickup;
    
    public IEnumerator ActivatePowerPickup(int index, float effectTime) {
        if (activePowerPickup != null) {
            StopCoroutine(activePowerPickup);
        }
        activeAutomaticShoot = StartCoroutine(AutomaticShoot(index));
        yield return new WaitForSeconds(effectTime);
        activeAutomaticShoot = StartCoroutine(AutomaticShoot(-1));
    }

    public int Health {
        get => health;
        set {
            if (health > 0) {
                if (value < health) {
                    if (!isRecovering) {
                        Debug.Log("IT HITS YOU");
                        health = value <= 0 ? 0 : value;
                        if (health == 0) Death();
                        else {
                            StartCoroutine(DamageRecovery(0.2f));
                            StartCoroutine(Blink(0.1f, Color.red));
                        }
                    }
                }
                else {
                    health = value > maxHealth ? maxHealth : value;
                    StartCoroutine(Blink(0.15f, Color.green));
                }
                UpdateHealthUi();
            }
        }
    }

    public int Score {
        get => score;
        set {
            score = value;
            UserInterface.sng.UpdateScore(score);
        }
    }

    public void UpdateHealthUi() {
        healthText.text = Health.ToString();
        healthSlider.value = Health;
    }
}