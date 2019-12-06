using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using MyObjectPooling;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
    public static GameController sng { get; private set; } //singletone
    public Transform obstacleHeap;
    public ObstaclePackPool[] obstaclePacksPool;
    public ObstaclePool[] obstaclesPool;
    
    private Player player;
    private float nextObstacleX = 0f;

    private Rigidbody2D rb;

    private void Awake() {
        if (sng == null) sng = this;
        else {
            Destroy(sng);
            sng = this;
        }
    }
    
    void Start() {
        Initialize();
    }

    void Update() {
        if (player.transform.position.x + 15f > nextObstacleX) {
            ObstaclePack obstaclePack = obstaclePacksPool[Random.Range(0,obstaclePacksPool.Length)].Get(obstacleHeap);
            if (obstaclePack) {
                obstaclePack.transform.position = new Vector3(nextObstacleX+obstaclePack.width/2, 0, 0);
                obstaclePack.ActiveObstaclePack = true;
                nextObstacleX += obstaclePack.width;
            }
        }
    }
    
    private void Initialize() {
        player = Player.sng;
    }

    public void RestartGame() {
        SceneManager.LoadScene("main");
    }
}