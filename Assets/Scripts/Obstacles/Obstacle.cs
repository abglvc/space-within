using UnityEngine;

public class Obstacle : MonoBehaviour {
    [Header("Obstacle")]
    public int OBSTACLE_INDEX;
    protected bool activeObstacle;
    protected int callerId;

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
}