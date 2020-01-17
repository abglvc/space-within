using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class F1SLink : MonoBehaviour {
    public static F1SLink sng { get; private set; }
    private Player p;

    
    [DllImport ("__Internal")]
    public static extern void showMainScreen (string service, string domain);


    private void Awake() {
        if (sng == null) sng = this;
        else {
            Destroy(sng);
            sng = this;
        }
    }
    
    void Start() {
        Initialize();
    }

    public void QuitGameSession() {
        Application.Unload();
    }
    
    public void UpdateDepthFromSensor(String s) {
        if(p) p.UpdateDepth(Int32.Parse(s));
    }

    private void Initialize() {
        p=Player.sng;
    }
}
