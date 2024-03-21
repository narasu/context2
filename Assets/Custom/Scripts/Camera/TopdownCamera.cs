using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TopdownCamera : MonoBehaviour
{
    private Camera cam;
    [SerializeField] private Transform Target;
    [SerializeField] private Vector3 Offset;
    [SerializeField] private float Speed;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }
    private void Update()
    {
        
    }
    
}
