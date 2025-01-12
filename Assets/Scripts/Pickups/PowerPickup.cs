﻿using UnityEngine;

namespace Pickups {
    public class PowerPickup : Pickup {
        [Header("PowerPickup")] public int projectile;
        public int effectTime;
        public AudioClip onPickMusic;

        public void SetStatesOnSpawn(PowerPickup bluePrint) {
            EffectTime = bluePrint.effectTime;
        }

        public int EffectTime {
            get => effectTime;
            set {
                effectTime = value;
                labelValueText.text = effectTime.ToString();
            }
        }
    }
}