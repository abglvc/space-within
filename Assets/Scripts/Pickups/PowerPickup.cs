using UnityEngine;

public class PowerPickup : Pickup {
    [Header("PowerPickup")]
    public int projectile;
    public int effectTime;
    
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
