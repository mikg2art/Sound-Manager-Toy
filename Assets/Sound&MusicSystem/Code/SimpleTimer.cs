using System;

public class SimpleTimer
{
    private float maxTime;
    public float curTime;
    public float Progress => curTime / maxTime;

    public SimpleTimer() { }

    public SimpleTimer(float _maxTime)
    {
        maxTime = _maxTime; curTime = 0;
    }

    public void Tick(float deltaTime)
    {
        if (curTime < maxTime)
            curTime += deltaTime;
    }

    public void SetMaxTime(float newMaxTime) { maxTime = newMaxTime; curTime = 0; }
    public float GetMaxTime() => maxTime;

    public void TimerReset() => curTime = 0;

    public float GetTime() => curTime;

    public bool IsCompleted() => curTime >= maxTime;
}
