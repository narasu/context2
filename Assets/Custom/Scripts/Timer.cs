using System;
using UnityEngine;

public class Timer
{
    public float TimeRemaining => length - currentTime;
    
    private float currentTime;
    private float length;
    private bool isLooping;
    private bool isStarted;
    private Delegate onExpired;

    public Timer(float _length, Delegate _onExpired, bool _startImmediately = true, bool _isLooping = false)
    {
        length = _length;
        onExpired = _onExpired;
        isStarted = _startImmediately;
        isLooping = _isLooping;
    }
    
    public void RunTimer()
    {
        if (!isStarted) return;
        if (currentTime < length)
        {
            currentTime += Time.deltaTime;
        }
        else
        {
            if (!isLooping) isStarted = false;
            currentTime = .0f;
            
            onExpired.DynamicInvoke();
        }
    }
    
    public void SetLength(float _length) => length = _length;
    
    public void Start() => isStarted = true;
    
    public void Pause() => isStarted = false;
    
    public float Stop()
    {
        float timeLeft = currentTime;
        isStarted = false;
        currentTime = .0f;
        return timeLeft;
    }
    
    public void Restart()
    {
        isStarted = true;
        currentTime = .0f;
    }
    
}
