using UnityEngine;

namespace Obstacles {
    public class RotaSpikes : Obstahurt {
        [Header("RotaSpikes")] public int rotationSpeed;

        private Transform rotatingPart;

        protected new void Awake() {
            Initialize();
            base.Awake();
        }

        public void SetStatesOnSpawn(RotaSpikes bluePrint, float difficulty) {
            rotationSpeed = bluePrint.rotationSpeed;
            base.SetStatesOnSpawn(bluePrint, difficulty);
        }

        private new void Initialize() {
            rotatingPart = transform.GetChild(1);
        }

        void Update() {
            rotatingPart.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
    }
}