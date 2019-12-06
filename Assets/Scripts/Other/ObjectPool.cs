using System.Collections.Generic;
using UnityEngine;

namespace MyObjectPooling {
    [System.Serializable]
    public class ObjectPool<T> where T: Object{
        public T objectPrefab;
        public int maxObjects=2;
    
        private List<T> pooledObjects=new List<T>();

        public T Get(Transform spawnOn) {
            foreach (T obj in pooledObjects)
                if (obj is Obstacle && !(obj as Obstacle).ActiveObstacle || 
                    obj is ObstaclePack && !(obj as ObstaclePack).ActiveObstaclePack ||
                    obj is LayerImage && !(obj as LayerImage).ActiveLayerImage ||
                    obj is GameObject && !(obj as GameObject).activeSelf )
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
                if (obj is ObstaclePack) (obj as ObstaclePack).ActiveObstaclePack = false;
                else if (obj is Obstacle) (obj as Obstacle).ActiveObstacle = false;
                else if (obj is LayerImage) (obj as LayerImage).ActiveLayerImage = false;
                else if (obj is GameObject) (obj as GameObject).SetActive(false);;
            }
        }
    }
    //Da se moze pristupiti u editoru.
    [System.Serializable]
    public class ObstaclePool : ObjectPool<Obstacle> {}
    [System.Serializable]
    public class ObstaclePackPool : ObjectPool<ObstaclePack> {}
    [System.Serializable]
    public class LayerImagePool : ObjectPool<LayerImage> {}
    [System.Serializable]
    public class GameObjectPool : ObjectPool<GameObject> {}
    
}