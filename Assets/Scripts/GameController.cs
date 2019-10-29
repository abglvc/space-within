using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using MyObjectPooling;

public class GameController : MonoBehaviour {
    public static GameController sng { get; private set; } //singletone
    public Transform obstacleHeap;
    public ObstaclePool[] obstaclesPool;
    private PlayerController player;
    private float nextObstacleX = 0f;

    private Rigidbody2D rb;

    private void Awake() {
        if (sng == null) sng = this;
        else {
            Destroy(sng);
            sng = this;
        }
    }

    // Start is called before the first frame update
    void Start() {
        player = PlayerController.sng;
    }

    // Update is called once per frame
    void Update() {
        if (player.transform.position.x + 15f > nextObstacleX) {
            Obstacle obstacle = obstaclesPool[Random.Range(0,obstaclesPool.Length)].GetFromPool(obstacleHeap);
            if (obstacle != null) {
                obstacle.transform.position = new Vector3(nextObstacleX, 0, 0);
                obstacle.ActiveObstacle = true;
                nextObstacleX += obstacle.width;
            }
        }
    }
}