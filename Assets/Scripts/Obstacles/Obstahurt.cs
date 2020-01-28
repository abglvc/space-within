using UnityEngine;

public class Obstahurt : DestructableObstacle {
    [Header("Obstahurt")]
    public int powerDamage = 1;
    public bool contactDestroy;

    public void SetStatesOnSpawn(Obstahurt bluePrint, float difficulty) {
        powerDamage = Mathf.RoundToInt(bluePrint.powerDamage * (1f + difficulty));
        contactDestroy = bluePrint.contactDestroy;
        base.SetStatesOnSpawn(bluePrint, difficulty);
    }

    public void UsedPower() {
        if (contactDestroy) Health = 0;
    }
}