using System;
using System.Collections.Generic;
using UnityEngine;

public class MainThreadDispatcher : MonoBehaviour
{
    private static readonly Queue<Action> _executionQueue = new Queue<Action>();

    public static void Enqueue(Action action)
    {
        lock (_executionQueue)
        {
            //Debug.Log($"Enqueueing Count: {_executionQueue.Count}");
            _executionQueue.Enqueue(action);
        }
    }

    private void Update()
    {
        lock (_executionQueue)
        {
            while (_executionQueue.Count > 0)
            {
                //Debug.LogWarning($"Dequeue Count: {_executionQueue.Count}");
                _executionQueue.Dequeue().Invoke();
            }
        }
    }
}
