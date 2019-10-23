using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FonePlugin : MonoBehaviour
{
    private String packageName = "com.lelo.sdk.ble.lelo_sdk_ble_f1_android_lib";
    private AndroidJavaObject o;
    
    void Start()
    {
        Debug.Log(AndroidJNI.AttachCurrentThread());
        o = new AndroidJavaObject(packageName+".LeloF1DeviceImpl");
        o.Call("enableWakeUp", true);
        o.Call("enableAccelerometerControl", true);
        o.Call("readAccelerometerControl");
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
