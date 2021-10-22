using Obstacles;
using UnityEngine;
using UnityEngine.UI;

namespace Pickups {
    public abstract class Pickup : Obstacle {
        public AudioClip onPickupSound;
        protected Text labelValueText;

        //assume da svaki pickup ima neki label
        private void Awake() {
            Initialize();
        }

        protected void Initialize() {
            labelValueText = GetComponentInChildren<Text>();
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.CompareTag("Player")) {
                other.GetComponent<Player.Player>().CaughtPickup(this);
                ActiveObstacle = false;
            }
        }
    }
}