using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class F1SLink : MonoBehaviour {
    void Start() {
        //AndroidJavaClass jc = new AndroidJavaClass("com.lelo.f1s.OverrideUnityActivity");
        //AndroidJavaObject overrideActivity = jc.GetStatic<AndroidJavaObject>("instance");
        //jc.CallStatic("UnitySendMessage", "F1SLink", "ChangeColor", "Yellow");
        
        //overrideActivity.Call("showMainActivity", lastStringColor);
    }

    public void ChangeColor(String a) {
        PlayerController.sng.GetComponent<SpriteRenderer>().color = String.Compare("Yellow", a)==0 ? Color.yellow : Color.white;
    }
    
    void Update() {
        
    }
}
