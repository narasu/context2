using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Trajectory draw code by Unity3D School
/// https://www.youtube.com/watch?v=K4DMCseZA08
/// </summary>

[RequireComponent(typeof(LineRenderer))]
public class ThrowHandler : MonoBehaviour
{
    public BombData BombDataAsset;
    private LineRenderer lineRenderer;
    
    [Header("Trajectory Display")]
    public int linePoints = 175;
    public float timeIntervalInPoints = 0.01f;
    
    private GameInputActions inputActions;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        

        if (lineRenderer.enabled)
        {
            
            DrawTrajectory();
        }
    }

    private void OnEnable()
    {
        if (ServiceLocator.TryLocate(Strings.InputManager, out object manager))
        {
            var inputManager = manager as InputManager;
            inputActions = inputManager.InputActions;
        }
        else
        {
            Debug.LogError("No input manager found!");
        }
        
        inputActions.Player.Throw.performed += OnThrow;
        inputActions.Player.Throw.canceled += OnThrowReleased;
    }
    
    private void OnThrow(InputAction.CallbackContext context)
    {
        lineRenderer.enabled = true;
        EventManager.Invoke(new ThrowStartEvent());
    }

    private void OnThrowReleased(InputAction.CallbackContext context)
    {
        lineRenderer.enabled = false;
        GameObject bomb = Instantiate(BombDataAsset.Prefab, transform.position, Quaternion.identity);
        Bomb b = bomb.GetComponent<Bomb>();
        b.Initialize(BombDataAsset);
        b.Throw(transform.forward);
        EventManager.Invoke(new ThrowEndEvent());
    }
    void DrawTrajectory()
    {
        Vector3 origin = transform.position;
        Vector3 startVelocity = BombDataAsset.ThrowForce * transform.forward;
        Vector3 endPoint = Vector3.positiveInfinity;
        int cutoffPoints = linePoints;
        lineRenderer.positionCount = linePoints;
        float time = 0;
        for (int i = 0; i < linePoints; i++)
        {
            // s = u*t + 1/2*g*t*t
            float x = (startVelocity.x * time) + (Physics.gravity.x / 2 * time * time);
            float y = (startVelocity.y * time) + (Physics.gravity.y / 2 * time * time);
            float z = (startVelocity.z * time) + (Physics.gravity.z / 2 * time * time);
            Vector3 point = new Vector3(x, y, z);
            lineRenderer.SetPosition(i, origin + point);
            if (Physics.Raycast(origin + point, point, 0.1f))
            {
                endPoint = origin + point;
                cutoffPoints = i + 1;
                break;
            }
            time += timeIntervalInPoints;
        }

        if (endPoint != Vector3.positiveInfinity)
        {
            lineRenderer.positionCount = cutoffPoints;
        }
    }
}
