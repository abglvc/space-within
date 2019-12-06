package com.lelo.f1s;

import android.os.Bundle;
import com.unity3d.player.UnityPlayerActivity;

public abstract class OverrideUnityActivity extends UnityPlayerActivity
{
    public static OverrideUnityActivity instance = null;

    protected int ATTACK_SENSITIVITY=750;
    protected int previousDepth=0;
    protected double previousPositionModule=0;

    abstract protected void showMainActivity();
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
