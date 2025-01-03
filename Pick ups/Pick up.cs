using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using UnityEngine;

public class Pickup : MonoBehaviour, ICollectible
{
    protected bool hasCollected = false;
    public virtual void Collect()
    {
        hasCollected = true;
    }
    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
