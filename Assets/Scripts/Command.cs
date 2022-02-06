using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Command : MonoBehaviour
{
    public UnityEvent method;

    public void Run()
    {
        method.Invoke();
    }
}
