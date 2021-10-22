using System.Collections;
using UnityEngine;

namespace Obstacles {
    public class Portal : Obstacle {
        [Header("Portal")] public Transform destination;
        const float TRAVEL_TIME = 0.8f;

        public void SetStatesOnSpawn(Transform transformInfo, Portal bluePrint) {
            Vector3 destinationPosition = bluePrint.destination.localPosition;
            destinationPosition.Scale(transform.localScale);
            destination.position = transformInfo.position + destinationPosition; //destinationPosition je localPosition
        }

        private IEnumerator PortalTravel(Player.Player p) {
            p.InPortal(true);
            yield return new WaitForSeconds(TRAVEL_TIME);
            p.transform.position = destination.position;
            p.InPortal(false);
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.CompareTag("Player"))
                StartCoroutine(PortalTravel(Player.Player.sng));
            else {
                other.gameObject.GetComponent<Projectile>().ActiveObstacle = false;
            }
        }
    }
}