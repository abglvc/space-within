using System.Collections.Generic;
using UnityEngine;

namespace MyObjectPooling {
    [System.Serializable]
    public class ObjectPool<T> where T: Object{
        public T objectPrefab;
        public int maxObjects=2;
    
        private List<T> pooledObjects=new List<T>();

        public T GetFromPool(Transform spawnOn) {
            foreach (T obj in pooledObjects)
                if (obj is Obstacle && !(obj as Obstacle).ActiveObstacle)
                    return obj;

            if (pooledObjects.Count < maxObjects) {
                T x = Object.Instantiate<T>(objectPrefab, spawnOn);
                pooledObjects.Add(x);
                return x;
            }

            return default(T);
        }

        public void DeactivateAllAlive() {
            foreach (T obj in pooledObjects) {
                if (obj is Obstacle) (obj as Obstacle).ActiveObstacle = false;
            }
        }
    }
    
    [System.Serializable]
    public class ObstaclePool : ObjectPool<Obstacle> {
    }
    
}