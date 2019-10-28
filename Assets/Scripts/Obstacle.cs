using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Obstacle : MonoBehaviour {
    public float width;
    protected bool activeObstacle=true;
    
    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        
    }

    public bool ActiveObstacle {
        get => activeObstacle;
        set {
            activeObstacle = value;
            Debug.Log(this.GetHashCode() + " active: " + value);
            gameObject.SetActive(activeObstacle);
        }
    }
}

[System.Serializable]
public class ObjectPool {
    public Object objectPrefab;
    public int maxObjects=3;
    
    private List<Object> pooledObjects=new List<Object>();

    public Object GetNewObject(Transform spawnOn) {
        foreach (Object obj in pooledObjects)
            if (obj is Obstacle && !((Obstacle) obj).ActiveObstacle)
                return obj;

        if (pooledObjects.Count < maxObjects) {
            Object x = MonoBehaviour.Instantiate(objectPrefab, spawnOn);
            pooledObjects.Add(x);
            return x;
        }
        return null;
    }

    public void DeactivateAllAlive() {
        foreach (Object obj in pooledObjects) {
            if (obj is Obstacle) ((Obstacle)obj).ActiveObstacle = false;
        }
    }
}