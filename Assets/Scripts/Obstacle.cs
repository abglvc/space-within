using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float width;

    private PlayerController player;
    
    // Start is called before the first frame update
    void Start()
    {
        player=PlayerController.sng;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x+width+8f < player.transform.position.x ) Destroy(gameObject);
    }
}