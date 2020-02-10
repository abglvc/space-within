using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DestructableObstacle : Obstacle {
    [Header("DestructableObstacle")]
    public int maxHealth;
    public bool alwaysShowUi;
    public float postDestructTime;
    public GameObject postDestructPrefab;
    private int health;
    private Canvas UICanvas;
    private Text healthText;
    private Slider healthSlider;
    private SpriteRenderer sr;
    private Color realColor;

    protected void Awake() {
        Initialize();
    }

    protected void OnCollisionEnter2D(Collision2D other) {
        GameObject otherGO = other.gameObject;
        
        if (otherGO.CompareTag("PlayerProjectile")) {
            Projectile up = otherGO.GetComponent<Projectile>();
            if(health-up.powerDamage <= 0) //add to score
                Player.sng.BonusScore += maxHealth * 25;
            Health -= up.powerDamage;
            up.UsedPower();
        }else if (otherGO.CompareTag("Obstacle") || otherGO.CompareTag("Obstahurt") || otherGO.CompareTag("Enemy") || otherGO.CompareTag("ObstacleProjectile") ) {
            ActiveObstacle = false;
        }
    }

    public void SetStatesOnSpawn(DestructableObstacle bluePrint, float difficulty) {
        if(sr) sr.color = realColor;
        maxHealth = Mathf.RoundToInt(bluePrint.maxHealth * (1f + difficulty));
        health = maxHealth;
        if (maxHealth > 1) {
            healthSlider.maxValue = maxHealth;
            UpdateHealthUi(alwaysShowUi);
        }
    }

    protected void Initialize() {
        sr = GetComponent<SpriteRenderer>();
        if(sr) realColor = sr.color;
        UICanvas = GetComponentInChildren<Canvas>();
        if (UICanvas) {
            healthText = UICanvas.GetComponentInChildren<Text>();
            healthSlider = UICanvas.GetComponentInChildren<Slider>();
            UpdateHealthUi(alwaysShowUi);
        }
    }

    private void Destruct() {
        if (postDestructPrefab) {
            GameObject destruct = Instantiate(postDestructPrefab, GameController.sng.obstacleHeap);
            destruct.transform.position = transform.position;
            Destroy(destruct, postDestructTime);
        }
        ActiveObstacle = false;
    }

    private IEnumerator Blink(float duration, Color color) {
        sr.color = color;
        yield return new WaitForSeconds(duration);
        sr.color = realColor;
    }

    public int Health {
        get => health;
        set {
            if (value <= health && value > 0 && sr) StartCoroutine(Blink(0.1f, Color.gray));
            health = value;
            if (health <= 0) Destruct();
            else if (UICanvas) UpdateHealthUi(true);
        }
    }

    private void UpdateHealthUi(bool showCanvas) {
        UICanvas.enabled = showCanvas;
        healthText.text = Health.ToString();
        healthSlider.value = Health;
    }
}
