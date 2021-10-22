using System.Collections;
using Controllers;
using Other;
using UnityEngine;
using UnityEngine.UI;

namespace Obstacles {
    public class DestructableObstacle : Obstacle {
        [Header("DestructableObstacle")] public int maxHealth;
        public bool alwaysShowUi;
        public float postDestructTime;
        public GameObject postDestructPrefab;
        private int health;
        protected Canvas UICanvas;
        private Text healthText;
        private Slider healthSlider;
        private SpriteRenderer sr;
        private Color realColor;
        private AudioSource impactAudioSrc;

        protected void Awake() {
            Initialize();
        }

        protected void OnCollisionEnter2D(Collision2D other) {
            GameObject otherGO = other.gameObject;

            if (otherGO.CompareTag("PlayerProjectile")) {
                Projectile up = otherGO.GetComponent<Projectile>();
                if (health - up.powerDamage <= 0) //add to score
                    Player.Player.sng.BonusScore += maxHealth * 25;
                else impactAudioSrc.Play();
                Health -= up.powerDamage;
                up.UsedPower();
            }
            else if (tag.Equals("PlayerProjectile") || tag.Equals("ObstacleProjectile")) {
                ActiveObstacle = false;
            }
        }

        public void SetStatesOnSpawn(DestructableObstacle bluePrint, float difficulty) {
            if (sr) sr.color = realColor;
            maxHealth = Mathf.RoundToInt(bluePrint.maxHealth * (1f + difficulty));
            health = maxHealth;
            if (maxHealth > 0 && healthSlider) {
                healthSlider.maxValue = maxHealth;
                UpdateHealthUi(alwaysShowUi);
            }
        }

        protected void Initialize() {
            impactAudioSrc = GetComponent<AudioSource>();
            if (impactAudioSrc && AudioManager.sng) impactAudioSrc.mute = !AudioManager.sng.PlaySounds;

            sr = GetComponent<SpriteRenderer>();
            if (sr) realColor = sr.color;
            if (!UICanvas) UICanvas = GetComponentInChildren<Canvas>();
            if (UICanvas) {
                healthText = UICanvas.GetComponentInChildren<Text>();
                healthSlider = UICanvas.GetComponentInChildren<Slider>();
                UpdateHealthUi(alwaysShowUi);
            }
        }

        private void Destruct() {
            if (postDestructPrefab) {
                GameObject destruct = Instantiate(postDestructPrefab, GameController.sng.obstaclePackHeap);
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

        public int MaxHealth {
            get => maxHealth;
            set {
                if (UICanvas) {
                    healthSlider.maxValue = value;
                    healthSlider.value = value;
                    healthText.text = value.ToString();
                }

                maxHealth = value;
            }
        }
    }
}