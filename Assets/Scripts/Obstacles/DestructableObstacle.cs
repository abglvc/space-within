using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DestructableObstacle : Obstacle {
    public int maxHealth;

    private int health;
    private Canvas UICanvas;
    private Text healthText;
    private Slider healthSlider;

    protected void Awake() {
        Initialize();
    }


    protected void OnCollisionEnter2D(Collision2D other) {
        string tag = other.gameObject.tag;
        if (tag.CompareTo("PlayerProjectile")==0) {
            UnitProjectile up = other.gameObject.GetComponent<UnitProjectile>();
            Health -= up.powerDamage;
            up.UsedPower();
        }else if (tag.CompareTo("Obstacle")==0) {
            ActiveObstacle = false;
        }
    }

    protected void Initialize() {
        UICanvas = GetComponentInChildren<Canvas>();
        health = maxHealth;
        if (UICanvas) {
            UICanvas.enabled = false;
            healthText = UICanvas.GetComponentInChildren<Text>();
            healthSlider = UICanvas.GetComponentInChildren<Slider>();
            healthSlider.maxValue = maxHealth;
            UpdateHealthUi(false);
        }
    }

    public int Health {
        get => health;
        set {
            health = value;
            if (health <= 0) ActiveObstacle = false;
            else if(UICanvas) UpdateHealthUi(true);
        }
    }
    
    private void UpdateHealthUi(bool showCanvas) {
        if (showCanvas) UICanvas.enabled = true;
        healthText.text = Health.ToString();
        healthSlider.value = Health;
    }
}
