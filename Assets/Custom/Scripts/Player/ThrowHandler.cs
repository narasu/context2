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
    public GameObject LandingDisc;
    private LineRenderer lineRenderer;
    
    [Header("Trajectory Display")]
    public int linePoints = 175;
    public float timeIntervalInPoints = 0.01f;
    
    private GameInputActions inputActions;
    private Vector3 landingPosition;
    private Vector3 launchVelocity;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        inputActions = new GameInputActions();
        //LandingDisc.transform.SetParent(null);
    }
    private void Update()
    {
        

        if (lineRenderer.enabled)
        {
            landingPosition += GetWorldAimDelta();
            landingPosition = Vector3.ClampMagnitude(landingPosition, 5.0f);
            Vector3 actualPosition = landingPosition + transform.position;
            Debug.DrawLine(actualPosition, actualPosition + Vector3.up * 2.0f, Color.green);
            float initialVelocity = CalculateInitialVelocity(actualPosition);
            float gravity = Physics.gravity.y;
            float timeOfFlight = 2 * initialVelocity / gravity;
            float maxHeight = initialVelocity * initialVelocity / (2 * -gravity);
            float horizontalRange = initialVelocity * timeOfFlight;

            // Update the bomb's rigidbody with the calculated trajectory
            launchVelocity = CalculateLaunchVelocity(actualPosition, timeOfFlight);
            DrawTrajectory(launchVelocity);
        }
    }

    private void OnEnable()
    {
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
        landingPosition = transform.forward * 2.0f;
        Debug.Log(landingPosition);
        LandingDisc.SetActive(true);
        
        lineRenderer.enabled = true;
        EventManager.Invoke(new ThrowStartEvent());
    }

    private void OnThrowReleased(InputAction.CallbackContext context)
    {
        LandingDisc.SetActive(false);
        GameObject bomb = Instantiate(BombDataAsset.Prefab, transform.position, transform.rotation);
        bomb.GetComponent<Rigidbody>().velocity = launchVelocity;
        
        lineRenderer.enabled = false;
        Bomb b = bomb.GetComponent<Bomb>();
        b.Initialize(BombDataAsset);
        EventManager.Invoke(new ThrowEndEvent());
    }
    
    private float CalculateInitialVelocity(Vector3 _landingPosition)
    {
        Vector3 displacement = transform.position - _landingPosition;
        displacement.y = 0;
        
        float timeOfFlight = displacement.magnitude / BombDataAsset.ThrowForce;
        float initialVelocity = displacement.magnitude / timeOfFlight;

        return initialVelocity;
    }

    private Vector3 CalculateLaunchVelocity(Vector3 _landingPosition, float timeOfFlight)
    {
        Vector3 displacement = transform.position - _landingPosition;
        displacement.y = 0;
        
        Vector3 launchVelocity = displacement / timeOfFlight;
        launchVelocity.y = (displacement.y + 0.5f * Physics.gravity.y * timeOfFlight * timeOfFlight) / timeOfFlight;
        
        return launchVelocity;
    }
    
    private Vector3 GetWorldAimDelta()
    {
        Transform camTransform = Camera.main.transform;
        Vector2 gpDelta = inputActions.Player.gp_Aim.ReadValue<Vector2>();
        Vector2 mouseDelta = inputActions.Player.mkb_Aim.ReadValue<Vector2>() * (Time.deltaTime * 5.0f);

        Vector2 combinedInput = gpDelta + mouseDelta;
        
        Vector3 forward = new Vector3(camTransform.forward.x, .0f, camTransform.forward.z).normalized;
        Vector3 right = new Vector3(camTransform.right.x, .0f, camTransform.right.z).normalized;
        Vector3 relativeInput = forward * combinedInput.y + right * combinedInput.x;
        return relativeInput;
    }

    
    private void DrawTrajectory(Vector3 _startVelocity)
    {
        Vector3 origin = transform.position;
        Vector3 startVelocity = _startVelocity;
        
        Vector3 endPoint = Utility.InvalidEndpoint;
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
            RaycastHit hit;

            if (i > 0)
            {
                Vector3 previous = lineRenderer.GetPosition(i - 1);
                if (Physics.Linecast(previous, origin + point, out hit))
                {
                    endPoint = hit.point;
                    cutoffPoints = i + 1;
                    break;
                }
            }
            time += timeIntervalInPoints;
        }

        if (endPoint != Utility.InvalidEndpoint)
        {
            lineRenderer.positionCount = cutoffPoints;
            LandingDisc.transform.position = endPoint;
        }
    }
}
