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
    public int health;
    public float verticalSpeed;
    public float horizontalSpeed;
    public float recoveryTime;
    public float attackRecharge = 2f;
    public ObstaclePool[] projectiles;
    
    [Header("UI Health")]
    public Text healthText;
    public Slider healthSlider;
    [Header("UI Attack")]
    public Slider attackRechargeSlider;
    public Image attackRechargeFill;
    public Color attackColor;
    public CapsuleCollider2D attackCollider;
    
    private bool canAttack = true;
    private bool topOriented;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;
    private RectTransform playerCanvasTransform;
    private bool isRecovering = false;
    private int previousDepth = 0;

    private int playerId;
    
    private void Awake() {
        if (sng == null) sng = this;
        else {
            Destroy(sng);
            sng = this;
        }
    }

    void Start() {
        Initialize();
    }
    
    void Update() {
        if (health > 0 && (Input.GetKeyDown(KeyCode.DownArrow) && rb.gravityScale < 0 && topOriented ||
                           Input.GetKeyDown(KeyCode.UpArrow) && rb.gravityScale > 0 && !topOriented))
            rb.gravityScale *= -1;
        UserInterface.sng.UpdateScore(Mathf.FloorToInt(transform.position.x));
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (health > 0) {
            if (other.collider.CompareTag("Obstacle")) {
                Vector2 pointDifference = other.GetContact(0).point - (Vector2) transform.position;
                if (Mathf.Abs(pointDifference.y) > 0.2f) topOriented = pointDifference.y > 0;
            }
            else if (other.collider.CompareTag("Obstahurt") || other.collider.CompareTag("ObstacleProjectile")) {
                Obstahurt oh = other.gameObject.GetComponentInParent<Obstahurt>();
                if(oh==null) oh=other.gameObject.GetComponentInChildren<Obstahurt>();
                Health -= oh.powerDamage;
                oh.UsedPower();
            }
            rb.velocity = Vector2.right * horizontalSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) { //Vracanje u pool!
        if (other.gameObject.CompareTag("ObstaclePack") && other.isTrigger)
            other.GetComponent<ObstaclePack>().ActiveObstaclePack = false;
        else if (other.gameObject.CompareTag("ParallaxLayer"))
            other.GetComponent<LayerImage>().ActiveLayerImage = false;
    }

    public void Initialize() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        playerCanvasTransform = GetComponentInChildren<RectTransform>();
        rb.gravityScale = verticalSpeed;
        topOriented = true;
        rb.velocity = Vector2.right * horizontalSpeed;
        healthSlider.maxValue = Health;
        UpdateHealthUi();

        playerId = GetInstanceID();
    }

    private Coroutine activeAutomaticShoot;
    private Transform obstacleHeap;
    
    private IEnumerator AutomaticShoot(int index) {
        if (activeAutomaticShoot != null) {
            StopCoroutine(activeAutomaticShoot);
            obstacleHeap = GameController.sng.obstacleHeap;
        }
        WaitForSeconds rechargeTime = new WaitForSeconds(attackRecharge);
        while (true) {
            UnitProjectile p = projectiles[index].Get(obstacleHeap) as UnitProjectile;
            if (p) {
                p.Spawn(playerId, transform.position, Vector3.right);
                p.ActiveObstacle = true;
            }
            yield return rechargeTime;
        }
    }

    public void UpdateDepth(int depth) {
        //Debug.Log("UPDATE DEPTH IN UNITY");
        OnGravityDirectionChange((int) Mathf.Sign(previousDepth-depth));
        UserInterface ui=UserInterface.sng;
        for(int i=Mathf.Min(previousDepth, depth); i<Mathf.Max(previousDepth, depth); i++)
            if (previousDepth <= depth)
                ui.EnableDepthSensor(i, true);
            else ui.EnableDepthSensor(i, false);
        previousDepth = depth;
    }

    public void Attack() {
        if (canAttack) StartCoroutine(LoadAttack());
    }

    private IEnumerator AttackInFront(float duration) {
        attackCollider.enabled = true;
        yield return new WaitForSeconds(duration);
        attackCollider.enabled = false;
    }

    private IEnumerator LoadAttack() {
        canAttack = false;
        StartCoroutine(AttackInFront(0.3f));
        animator.SetTrigger("attack");
        WaitForSeconds tick = new WaitForSeconds(0.1f);
        float tAttackRecharge = attackRecharge;
        attackRechargeFill.color=Color.white;
        while (tAttackRecharge >= 0) {
            attackRechargeSlider.value = 1f - tAttackRecharge / attackRecharge;
            tAttackRecharge -= 0.1f;
            yield return tick;
        }
        canAttack = true;
        attackRechargeFill.color = attackColor;
    }

    public void OnGravityDirectionChange(int k) {
        if (health > 0 && (k == 1 && rb.gravityScale < 0 && topOriented) ||
            k == -1 && rb.gravityScale > 0 && !topOriented) {
            rb.gravityScale *= -1;
            transform.localScale = new Vector3(1f, Mathf.Sign(rb.gravityScale), 1f);
        }
    }

    private void OnDisable() {
        if (isRecovering) isRecovering = false; //zbog aktivne korutine i portala
    }

    private void OnEnable() {
        activeAutomaticShoot = StartCoroutine(AutomaticShoot(0));
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
    
    public int Health {
        get => health;
        set {
            if (value - health < 0) {
                if (!isRecovering) {
                    Debug.Log("IT HITS YOU");
                    animator.SetTrigger("harmed");
                    health = value;
                    if (health <= 0) {
                        UserInterface.sng.ripSplash.SetActive(true);
                        rb.velocity = Vector2.zero;
                        animator.enabled = false;
                    }
                    else StartCoroutine(DamageRecovery(0.1f));
                }
            }
            else health = value;
            UpdateHealthUi();
        }
    }
    
    public void UpdateHealthUi() {
        healthText.text = Health.ToString();
        healthSlider.value = Health;
    }
}