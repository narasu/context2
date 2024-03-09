using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TriggerCheck : MonoBehaviour
{
    public bool HasCollision { get; private set; } = false;
    private bool hadCollision;
    private List<Collider> colliders = new();
    private Collider col;
        
    private void Start()
    {
        col = GetComponent<Collider>();
        HasCollision = colliders.Count > 0;
    }

    private void FixedUpdate()
    {
        RemoveDestroyed();
    }

    private void OnTriggerStay(Collider _other)
    {
        if (colliders.Contains(_other))
        {
            return;
        }
        colliders.Add(_other);
            
        HasCollision = true;
        if (!hadCollision)
        {
            hadCollision = true;
        }
    }

    private void OnTriggerExit(Collider _other)
    {
            
        if (colliders.Contains(_other))
        {
            colliders.Remove(_other);
        }

        if (colliders.Count == 0)
        {
            HasCollision = false;
            if (hadCollision)
            {
                hadCollision = false;
            }
        }
    }

    public float GetShortestDistanceFromCenter()
    {
        if (colliders.Count == 0)
        {
            return .0f;
        }

        float distance = float.MaxValue;

        foreach (Collider t in colliders)
        {
            if (t.IsDestroyed())
            {
                continue;
            }
            float colliderTop = t.bounds.max.y;
            float diff = transform.position.y - colliderTop;
            if (diff < distance)
            {
                distance = diff;
            }
        }
        
        if (distance < col.bounds.size.y && distance > .0f)
        {
            return distance;
        }

        return .0f;
    }

    private void RemoveDestroyed()
    {
        if (colliders.Count == 0)
        {
            return;
        }
        
        List<Collider> destroyedList = new();
        foreach (Collider c in colliders)
        {
            if (c.IsDestroyed())
            {
                destroyedList.Add(c);
            }
        }
        foreach (Collider c in destroyedList)
        {
            colliders.Remove(c);
        }
    }

}