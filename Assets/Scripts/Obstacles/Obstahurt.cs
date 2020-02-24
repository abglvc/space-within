using UnityEngine;
using UnityEngine.UI;

public class Obstahurt : DestructableObstacle {
    [Header("Obstahurt")]
    public int powerDamage;
    public bool contactDestroy;

    private Text powerDamageText;
    protected new void Awake() {
        Initialize();
        base.Awake();
    }
    
    public void SetStatesOnSpawn(Obstahurt bluePrint, float difficulty, int newPowerDamage=0) {
        if (newPowerDamage==0) {
            PowerDamage = Mathf.RoundToInt(bluePrint.powerDamage * (1f + difficulty));
        }else{
            int newDmg = Mathf.RoundToInt(newPowerDamage / 2f);
            PowerDamage = newDmg > 0 ? newDmg : 1;
        }
        contactDestroy = bluePrint.contactDestroy;
        base.SetStatesOnSpawn(bluePrint, difficulty);
    }

    public void UsedPower() {
        if (contactDestroy) Health = 0;
    }
    
    private new void Initialize() {
        if(!UICanvas) UICanvas = GetComponentInChildren<Canvas>();
        if (UICanvas) {
            powerDamageText = UICanvas.GetComponentsInChildren<Text>()[1];
        }
    }

    public int PowerDamage {
        get => powerDamage;
        set {
            powerDamage = value;
            if(powerDamageText) powerDamageText.text = powerDamage.ToString();
        }
    }
}