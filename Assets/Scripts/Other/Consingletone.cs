using System;
using System.Collections.Generic;
using MyObjectPooling;
using UnityEngine;

public class Consingletone : MonoBehaviour {
    public static Consingletone sng { get; private set; } //singletone
    public ObstaclePool[] obstaclesPool;
    public PlanetSprites[] planetsSkins;
    public GameObject[] planetStartPlatforms;
    public GameObject planetEndPlatform;

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

[Serializable]
public class PlanetSprites {
    public Sprite[] sprites;
    public Sprite planetBackground;
    private Dictionary<int, int> idToIndex = new Dictionary<int, int> {
        {7, 0}, {1, 1}, {28, 2}, {3, 3}, {0, 4}, {15, 5}, {14, 4}
    };

    public Sprite GetSkin(int obsID) {
        return sprites[idToIndex[obsID]];
    }

    public Dictionary<int, int> IdToIndex => idToIndex;
}