package com.lelo.f1s;

import android.os.Bundle;
import com.unity3d.player.UnityPlayerActivity;

public abstract class OverrideUnityActivity extends UnityPlayerActivity
{
    public static OverrideUnityActivity instance = null;
    
    abstract protected void showMainActivity(String setToColor);
    abstract public void UpdateDepthFromSensor(Integer x);
    abstract public void UpdateAccelerationRate(Integer x, Integer y);

    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        instance = this;
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        instance = null;
    }
}
