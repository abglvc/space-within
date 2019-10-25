using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController sng { get; private set; } //singletone
    public Transform obstacleHeap;
    public Obstacle[] obstacles;
    private PlayerController player;
    private float nextObstacleX = 0f;
    
    private Rigidbody2D rb;

    private void Awake()
    {
        if (sng == null) sng = this;
        else
        {
            Destroy(sng);
            sng = this;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        player=PlayerController.sng;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.x + 15f > nextObstacleX)
        {
            Obstacle o=Instantiate(obstacles[Random.Range(0, obstacles.Length)], obstacleHeap);
            o.transform.position=new Vector3(nextObstacleX, 0,0);
            nextObstacleX += o.width;
        }
    }
}
