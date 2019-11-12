using System.Collections;
using System.Collections.Generic;
using MyObjectPooling;
using UnityEngine;

public class Parallax : MonoBehaviour {
    public static Parallax sng { get; private set; } //singletone
    
    public ParallaxLayerPool[] parallaxLayersPool;
    
    private void Awake() {
        if (sng == null) sng = this;
        else {
            Destroy(sng);
            sng = this;
        }
    }
    
    void Start(){
        
    }

    void Update(){
        
    }

    public void CreateNew(int index, Vector3 endPoint) {
        ParallaxLayer pL = parallaxLayersPool[index].GetFromPool(transform);
        pL.transform.position = endPoint + Vector3.right * pL.width;
    }
}
