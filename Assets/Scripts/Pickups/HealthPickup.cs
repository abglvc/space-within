using System;
using UnityEngine;

public class HealthPickup : Pickup {
    [Header("HealthPickup")]
    public int healValue;

    public void SetStatesOnSpawn(HealthPickup bluePrint) {
        HealValue = bluePrint.healValue;
    }

    public int HealValue {
        get => healValue;
        set {
            healValue = value;
            labelValueText.text = healValue.ToString();
        }
    }
}