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
    private Vector3 landingPosition;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        inputActions = new GameInputActions();
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
        // if (ServiceLocator.TryLocate(Strings.InputManager, out object manager))
        // {
        //     var inputManager = manager as InputManager;
        //     inputActions = inputManager.InputActions;
        // }
        // else
        // {
        //     Debug.LogError("No input manager found!");
        // }

        inputActions.Enable();
        inputActions.Player.Throw.performed += OnThrow;
        inputActions.Player.Throw.canceled += OnThrowReleased;
    }

    private void OnDisable()
    {
        inputActions.Disable();
        
        inputActions.Player.Throw.performed -= OnThrow;
        inputActions.Player.Throw.canceled -= OnThrowReleased;
    }

    private void OnThrow(InputAction.CallbackContext context)
    {
        landingPosition = transform.position + transform.forward * 2.0f;
        lineRenderer.enabled = true;
        EventManager.Invoke(new ThrowStartEvent());
    }

    private void OnThrowReleased(InputAction.CallbackContext context)
    {
        GameObject bomb = Instantiate(BombDataAsset.Prefab, transform.position, Quaternion.identity);
        float initialVelocity = CalculateInitialVelocity(landingPosition);
        float mass = bomb.GetComponent<Rigidbody>().mass;
        float gravity = Physics.gravity.y;
        float timeOfFlight = 2 * initialVelocity / gravity;
        float maxHeight = initialVelocity * initialVelocity / (2 * -gravity);
        float horizontalRange = initialVelocity * timeOfFlight;

        // Update the bomb's rigidbody with the calculated trajectory
        bomb.GetComponent<Rigidbody>().velocity = CalculateLaunchVelocity(landingPosition, timeOfFlight);

        lineRenderer.enabled = false;
        Bomb b = bomb.GetComponent<Bomb>();
        b.Initialize(BombDataAsset);
        //b.Throw(transform.forward);
        EventManager.Invoke(new ThrowEndEvent());
    }
    
    // Function to calculate the initial velocity based on the chosen landing position
    private float CalculateInitialVelocity(Vector3 landingPosition)
    {
        // Calculate the displacement in the x and z directions
        Vector3 displacement = landingPosition - transform.position;
        displacement.y = 0; // Ignore vertical distance

        // Calculate the time of flight using the horizontal displacement and chosen throw force
        float timeOfFlight = displacement.magnitude / BombDataAsset.ThrowForce;

        // Calculate the initial velocity using the time of flight and displacement
        float initialVelocity = displacement.magnitude / timeOfFlight;

        return initialVelocity;
    }

    // Function to calculate the launch velocity based on the chosen landing position and time of flight
    private Vector3 CalculateLaunchVelocity(Vector3 _landingPosition, float timeOfFlight)
    {
        // Calculate the displacement in the x and z directions
        Vector3 displacement = transform.position - _landingPosition;
        displacement.y = 0; // Ignore vertical distance

        // Calculate the launch velocity using the horizontal displacement and time of flight
        Vector3 launchVelocity = displacement / timeOfFlight;

        // Set the y component of the launch velocity to account for the vertical motion
        launchVelocity.y = (displacement.y + 0.5f * Physics.gravity.y * timeOfFlight * timeOfFlight) / timeOfFlight;

        return launchVelocity;
    }
    
    private void DrawTrajectory()
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
            if (Physics.Raycast(origin + point, point, out RaycastHit hit, point.magnitude))
            {
                endPoint = hit.point;
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
