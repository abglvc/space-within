using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Ship
{
    [Header("Enemy")]
    public bool iterateMovement;
    public Projectile projectilePrefab;
    public List<Vector2> movePoints;
    
    protected int indexAtPoint=0;
    protected bool move = true;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (move) Move();
        
        
    }

    protected void Move()
    {
        if (Vector2.Distance(transform.position, movePoints[IndexAtPoint]) < 0.2)
        {
            IndexAtPoint++;
            rb.velocity = (movePoints[IndexAtPoint] - (Vector2) transform.position).normalized * speed;
        }
    }

    protected int IndexAtPoint
    {
        get { return indexAtPoint; }
        set
        {
            indexAtPoint=value;
            if (indexAtPoint >= movePoints.Count)
            {
                if (!iterateMovement)
                {
                    move = false;
                    rb.velocity=Vector2.zero;
                }
                else indexAtPoint = 0;
            }
            
        }
    }
    
}
