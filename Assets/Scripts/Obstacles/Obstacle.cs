using UnityEngine;

public class Obstacle : MonoBehaviour {
    [Header("Obstacle")]
    public int OBSTACLE_INDEX;
    protected bool activeObstacle;
    protected int callerId;
    
    private int planetSpawn = -1;
    
    public void Spawn(int callerId, Transform transformInfo) {
        this.callerId = callerId;
        transform.position = transformInfo.position;
        transform.rotation = transformInfo.rotation;
        ActiveObstacle = true;
    }
    

    public bool ActiveObstacle {
        get => activeObstacle;
        set {
            activeObstacle = value;
            gameObject.SetActive(activeObstacle);
        }
    }

    public int CallerId {
        get => callerId;
    }

    public int PlanetSpawn {
        get => planetSpawn;
        set => planetSpawn = value;
    }
}