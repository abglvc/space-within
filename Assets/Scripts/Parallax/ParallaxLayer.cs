using MyObjectPooling;
using UnityEngine;

public class ParallaxLayer : MonoBehaviour {
    public float stickyFactor;    //0-1
    public LayerImagePool[] layerImages;
    
    private Transform lockOn;
    private float nextSpawnDistance = 0f;

    void Start() {
        Initialize();
    }


    private LayerImage lastLI;
    private float rightCameraOffset;
    
    void Update() {
        if (lockOn) {
            transform.position=new Vector3(stickyFactor*(lockOn.position.x)-10f, 0, 0);
            if (!lastLI || lockOn.position.x + rightCameraOffset > lastLI.transform.position.x+lastLI.Width/2f) { //HEHEHRHE
                lastLI = layerImages[Random.Range(0, layerImages.Length)].GetOrSpawnIn(transform);
                if (lastLI) {
                    lastLI.transform.localPosition = new Vector3(nextSpawnDistance, 0f,0f);
                    nextSpawnDistance += lastLI.Width;
                    lastLI.ActiveLayerImage = true;
                }
            }
        }
    }

    private void Initialize() {
        lockOn = Camera.main.transform;
        rightCameraOffset = Camera.main.GetComponent<CameraController>().rightCameraOffset;
    }
}
