using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyObjectPooling;

public class Consingletone : MonoBehaviour {
    public static Consingletone sng { get; private set; } //singletone
    public ObstaclePool[] obstaclesPool;
    
    private void Awake() {
        if (sng == null) sng = this;
        else {
            Destroy(sng);
            sng = this;
        }
    }

    public Obstacle GetObstacleFromPool(int obstacleIndex) {
        return obstaclesPool[obstacleIndex].GetOrSpawnIn(transform);
    }
}
