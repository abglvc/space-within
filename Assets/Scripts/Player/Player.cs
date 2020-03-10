using System.Collections;
using Controllers;
using Obstacles;
using Other;
using Parallax;
using Pickups;
using UnityEngine;
using UnityEngine.UI;

namespace Player {
    public class Player : MonoBehaviour {
        public static Player sng { get; private set; } //singletone
        public int maxHealth;
        public float verticalSpeed;
        public float horizontalSpeed;
        public int maxRotationDeg;
        public float recoveryTime;
        public float attackRecharge = 2f;
        public GameObject postDestructPrefab;
        public float postDestructTime;
        public ObstaclePool[] projectiles;
        public Transform[] shootSources;

        [Header("UI Health")] public Text healthText;
        public Slider healthSlider;
        public Text powerupDurationText;
        public Slider powerupDurationSlider;
        public Color colorHurt, colorHeal;

        private int health;
        private int score = 0, bonusScore = 0;
        private Rigidbody2D rb;
        private SpriteRenderer sr;
        private Collider2D col;
        private bool isRecovering = false;
        private int projectileIndex = -1;
        private int playerId;
        private Projectile[] projectileBluePrints = new Projectile[3];

        private AudioManager audioManager;
        private AudioSource projAudioSrc, impactAudioSrc, pickupAudioSource;

        private void Awake() {
            if (sng == null) sng = this;
            else {
                Destroy(sng);
                sng = this;
            }

            Initialize();
        }

        private float nextVelocityCheck = 0.5f;
        [HideInInspector] public int previousDistance = 0;

        void Update() {
            if (health > 0) {
                if (Mathf.FloorToInt(DistanceTraveled()) > previousDistance) { // add distance to score
                    int dist = Mathf.FloorToInt(DistanceTraveled());
                    Score += dist - previousDistance;
                    previousDistance = dist;
                }

                int k = 0;
                if (Input.GetKeyDown(KeyCode.DownArrow)) k = 1;
                else if (Input.GetKeyDown(KeyCode.UpArrow)) k = -1;
                OnGravityDirectionChange(k);

                if (Time.time > nextVelocityCheck) {
                    if (rb.velocity.x < 0 || rb.velocity.magnitude < horizontalSpeed / 2) {
                        rb.velocity = Vector2.right * horizontalSpeed;
                    }

                    nextVelocityCheck += 0.5f;
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D other) {
            if (activeTilt != null && other.gameObject.CompareTag("Obstacle") &&
                (int) Mathf.Sign(other.GetContact(0).point.y - transform.position.y) !=
                (int) Mathf.Sign(rb.gravityScale)) {
                StopCoroutine(activeTilt);
                transform.rotation = new Quaternion();
            }

            if (health > 0) {
                GameObject g = other.gameObject;
                if (g.CompareTag("Obstahurt") || g.CompareTag("ObstacleProjectile") || g.CompareTag("Enemy")) {
                    Obstahurt oh = g.GetComponentInParent<Obstahurt>();
                    if (oh == null) oh = g.GetComponentInChildren<Obstahurt>();
                    if (oh) {
                        Health -= oh.powerDamage;
                        oh.UsedPower();
                    }
                }

                rb.velocity = Vector2.right * horizontalSpeed;
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
                    GameController gc = GameController.sng;
                    switch (gc) {
                        case FreerunGC freerunGc:
                            StartCoroutine(freerunGc.LoadNextPlanet(this));
                            break;
                        case LevelGC levelGc:
                            InPortal(true);
                            levelGc.EndGame();
                            break;
                    }

                    break;
                case "StartPack":
                    Destroy(other.gameObject);
                    break;
                case "ManualsOP":
                    other.gameObject.SetActive(false);
                    break;
            }
        }

        public void Initialize() {
            audioManager = AudioManager.sng;
            rb = GetComponent<Rigidbody2D>();
            sr = GetComponent<SpriteRenderer>();
            col = GetComponent<Collider2D>();
            health = maxHealth;
            rb.gravityScale = verticalSpeed;
            rb.velocity = Vector2.right * horizontalSpeed;

            if (healthSlider) {
                healthSlider.maxValue = Health;
                UpdateHealthUi();
            }

            playerId = GetInstanceID();

            if (audioManager) {
                AudioSource[] x = GetComponents<AudioSource>();
                projAudioSrc = x[0];
                impactAudioSrc = x[1];
                pickupAudioSource = x[2];

                projAudioSrc.mute = !audioManager.PlaySounds;
                impactAudioSrc.mute = !audioManager.PlaySounds;
                pickupAudioSource.mute = !audioManager.PlaySounds;
                postDestructPrefab.GetComponent<AudioSource>().mute = !GameController.sng.Dao.data.playSounds;
            }

            for (int i = 0; i < projectileBluePrints.Length; i++)
                projectileBluePrints[i] = projectiles[i].objectPrefab as Projectile;
        }

        public float DistanceTraveled() {
            return transform.position.x;
        }

        private Coroutine activeAutomaticShoot;

        private IEnumerator AutomaticShoot(int index) {
            projectileIndex = index;
            if (activeAutomaticShoot != null) {
                StopCoroutine(activeAutomaticShoot);
            }

            if (projectileIndex == -1) yield break;
            WaitForSeconds rechargeTime = new WaitForSeconds(attackRecharge);
            float difficulty = GameController.sng.Difficulty();
            Projectile proj = projectileBluePrints[index];
            projAudioSrc.clip = proj.soundEffect;

            while (true) {
                for (int i = 0; i < shootSources.Length; i++) {
                    Projectile p = projectiles[index].GetOrSpawnIn(transform.parent) as Projectile;
                    if (p) {
                        if (i % 2 == 0) projAudioSrc.Play();
                        p.Spawn(playerId, shootSources[i]);
                        p.SetStatesOnSpawn(proj, proj.moveDirection, horizontalSpeed, difficulty);
                        p.ActiveObstacle = true;
                    }
                }

                yield return rechargeTime;
            }
        }

        public void InPortal(bool value) {
            if (value) {
                audioManager.Play2DSound(4);
                if (activeAutomaticShoot != null) {
                    StopCoroutine(activeAutomaticShoot);
                }
            }
            else activeAutomaticShoot = StartCoroutine(AutomaticShoot(projectileIndex));

            rb.simulated = !value;
            col.enabled = !value;
            sr.enabled = !value;
        }

        public void CaughtPickup(Pickup pickup) {
            pickupAudioSource.clip = pickup.onPickupSound;
            pickupAudioSource.Play();

            switch (pickup) {
                case PowerPickup powerPickup:
                    activePowerPickup = StartCoroutine(ActivatePowerPickup(powerPickup));
                    break;
                case HealthPickup healthPickup:
                    Health += healthPickup.healValue;
                    break;
            }
        }

        private void Death() {
            if (postDestructPrefab) {
                GameObject destruct = Instantiate(postDestructPrefab, transform.parent);
                destruct.transform.position = transform.position;
                Destroy(destruct, postDestructTime);
            }

            gameObject.SetActive(false);
            GameController.sng.EndGame();
        }

        private int previousDepth = 0;
        public void UpdateDepth(int depth) {
            OnGravityDirectionChange((int) Mathf.Sign(previousDepth - depth));
            
            UserInterface ui = UserInterface.sng;
            if(ui) ui.UpdateDepth(depth);
            else ManualsController.sng.UpdateDepth(depth);

            previousDepth = depth;
        }

        public void OnGravityDirectionChange(int k) {
            if (health > 0 && (k == 1 && rb.gravityScale < 0) ||
                k == -1 && rb.gravityScale > 0) {
                rb.gravityScale *= -1;
                if (gameObject.activeSelf) activeTilt = StartCoroutine(TiltRotate(k, 0.2f, 0.05f));
            }
        }

        private Coroutine activeTilt;

        private IEnumerator TiltRotate(int k, float tiltTime, float tiltTick) {
            if (activeTilt != null) {
                StopCoroutine(activeTilt);
                transform.rotation = new Quaternion();
            }

            float initialTime = tiltTime;
            float tiltAmountPerTick = -k * maxRotationDeg * tiltTick / initialTime;
            WaitForSeconds tick = new WaitForSeconds(tiltTick);
            while (tiltTime > 0) {
                transform.Rotate(Vector3.forward, tiltAmountPerTick);
                tiltTime -= tiltTick;
                yield return tick;
            }
        }

        private Coroutine activeFrameSplash;

        private IEnumerator FrameSplash(float tickTime, Color color, bool recover) {
            if (activeFrameSplash != null) {
                StopCoroutine(activeFrameSplash);
            }

            isRecovering = recover;
            WaitForSeconds tick = new WaitForSeconds(tickTime);
            float time = recover ? recoveryTime : tickTime;
            UserInterface.sng.frameSplash.color = color;
            GameObject frameSplashObj = UserInterface.sng.frameSplash.gameObject;
            while (time > 0) {
                sr.color = sr.color == Color.white ? color : Color.white;
                frameSplashObj.SetActive(!frameSplashObj.activeSelf);
                time -= tickTime;
                yield return tick;
            }

            if (recover) isRecovering = false;
            sr.color = Color.white;
            frameSplashObj.SetActive(false);
        }

        private Coroutine activePowerPickup;

        public IEnumerator ActivatePowerPickup(PowerPickup powerPickup) {
            int index = powerPickup.projectile;
            int effectTime = powerPickup.effectTime;
            AudioSource musicSource = audioManager.MusicSource;

            if (activePowerPickup != null) {
                StopCoroutine(activePowerPickup);
            }

            musicSource.Stop();
            musicSource.clip = powerPickup.onPickMusic;
            musicSource.Play();
            activeAutomaticShoot = StartCoroutine(AutomaticShoot(index));
            //slider
            WaitForSeconds tick = new WaitForSeconds(1);
            powerupDurationSlider.gameObject.SetActive(true);
            powerupDurationSlider.maxValue = effectTime;
            while (effectTime > 0) {
                powerupDurationSlider.value = effectTime;
                powerupDurationText.text = effectTime.ToString();
                effectTime -= 1;
                yield return tick;
            }

            musicSource.Stop();
            musicSource.clip = audioManager.loopMusicTrack;
            musicSource.Play();

            powerupDurationSlider.gameObject.SetActive(false);
            activeAutomaticShoot = StartCoroutine(AutomaticShoot(-1));
        }

        public int Health {
            get => health;
            set {
                if (health > 0) {
                    if (value < health) {
                        if (!isRecovering) {
                            //Debug.Log("IT HITS YOU");
                            health = value <= 0 ? 0 : value;
                            if (health == 0) Death();
                            else {
                                impactAudioSrc.Play();
                                activeFrameSplash = StartCoroutine(FrameSplash(0.2f, colorHurt, true));
                            }
                        }
                    }
                    else {
                        health = value > maxHealth ? maxHealth : value;
                        activeFrameSplash = StartCoroutine(FrameSplash(0.3f, colorHeal, false));
                    }

                    UpdateHealthUi();
                }
            }
        }

        public int BonusScore {
            get => bonusScore;
            set {
                Score += value - bonusScore;
                bonusScore = value;
                UserInterface.sng.UpdateBonusScore(bonusScore);
            }
        }

        public int Score {
            get => score;
            set {
                score = value;
                UserInterface ui = UserInterface.sng;
                if (ui) ui.UpdateScore(score);
            }
        }

        public void UpdateHealthUi() {
            healthText.text = Health.ToString();
            healthSlider.value = Health;
        }
    }
}