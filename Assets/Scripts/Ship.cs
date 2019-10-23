using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    [Header("Ship")]
    public float speed;
    public int health;
    public List<Projectile> projectilePrefabs=new List<Projectile>();
    
    protected Vector2 newPosition = Vector2.zero;
    protected Rigidbody2D rb=null;
    
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector2 NewPosition
    {
        get { return newPosition; }
    }
}