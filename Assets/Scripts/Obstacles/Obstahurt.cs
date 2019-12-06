using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstahurt : DestructableObstacle {
    public int powerDamage = 1;
    public bool oneAttack;
    
    private void OnEnable() { // maybe
        Initialize();
    }
    public void UsedPower() {
        if(oneAttack) ActiveObstacle = false;
    }
}