using System;
using UnityEngine;
using UnityEngine.Events;

public class OnTrigger : MonoBehaviour
{
    public string tag;
    public UnityEvent OnTriggerEnterEvent;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tag))
        {
            OnTriggerEnterEvent?.Invoke();
        }
    }
}
