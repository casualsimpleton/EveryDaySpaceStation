
using System;
using System.Collections;
using UnityEngine;

public class Server_TestCode : MonoBehaviour
{
    protected float _printTimer;
    protected float _printTimerDelta;

    public void Start()
    {
        _printTimer = 0f;
        _printTimerDelta = 1f;
    }

    public void Update()
    {
        if (Time.time > _printTimer)
        {
            Debug.Log("Time " + Time.time);
            System.Console.WriteLine("Time " + Time.time);
            _printTimer = Time.time + _printTimerDelta;
        }
    }
}