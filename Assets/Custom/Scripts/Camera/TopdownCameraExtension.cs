using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Cinemachine.Utility;

[SaveDuringPlay][AddComponentMenu("")]
public class TopdownCameraExtension : CinemachineExtension
{
    private bool wasMoving;
    private bool isPlayerGrounded;

    private CinemachineVirtualCamera vc;
    private CinemachineComposer c;
    private CinemachineFramingTransposer ft;

    [Range(.0f,2.0f)] public float groundedDeadzoneHeight;
    [Range(.0f, 2.0f)] public float jumpingDeadzoneHeight;

    [Range(0.0f, 360.0f)] public float rotationSpeed;
    private float rotationVelocity;

    private Action<GroundedChangedEvent> groundedChangedEventHandler;

    private GameInputActions inputActions;

    protected override void Awake()
    {
        base.Awake();
        groundedChangedEventHandler = _event => isPlayerGrounded = _event.IsGrounded;
        EventManager.Subscribe(typeof(GroundedChangedEvent), groundedChangedEventHandler);
        vc = GetComponent<CinemachineVirtualCamera>();
        c = vc.GetCinemachineComponent<CinemachineComposer>();
        ft = vc.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    private void Start()
    {
        if (vc.Follow == null && ServiceLocator.TryLocate(Strings.Player, out object player))
        {
            vc.Follow = player as Transform;
        }
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            return;
        }
        Vector2 rotateInput = inputActions.Player.Look.ReadValue<Vector2>();

        rotationVelocity = Mathf.Lerp(rotationVelocity, rotateInput.magnitude * rotationSpeed, 0.99f * Time.deltaTime);
        
        // Rotate the camera around its target based on input values
        if (Mathf.Abs(rotateInput.x) > 0.1f || Mathf.Abs(rotateInput.y) > 0.1f)
        {
            Vector3 targetPosition = vc.Follow.position;
            
            transform.RotateAround(targetPosition, Vector3.up, rotateInput.x * rotationVelocity * Time.deltaTime);
            //transform.RotateAround(targetPosition, transform.right, -rotateInput.y * rotationVelocity * Time.deltaTime);
        }

    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventManager.Unsubscribe(typeof(GroundedChangedEvent), groundedChangedEventHandler);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        
        if (ServiceLocator.TryLocate(Strings.InputAsset, out object asset))
        {
            inputActions = asset as GameInputActions;
        }
    }

    private void OnDisable()
    {
        
    }

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase _vcam, CinemachineCore.Stage _stage, ref CameraState _state, float _deltaTime)
    {
        // if (vc == null || c == null)
        // {
        //     vc = _vcam as CinemachineVirtualCamera;
        //     if (vc == null)
        //     {
        //         Debug.Log("PlatformingCameraExtension: No virtual camera found");
        //         return;
        //     }
        //     c = vc.GetCinemachineComponent<CinemachineComposer>();
        //     ft = vc.GetCinemachineComponent<CinemachineFramingTransposer>();
        // }
        
        // if (_stage == CinemachineCore.Stage.Body)
        // {
        //     if (isPlayerGrounded)
        //     {
        //         ft.m_DeadZoneHeight = Mathf.Lerp(ft.m_DeadZoneHeight, groundedDeadzoneHeight, .1f);
        //     }
        //     else
        //     {
        //         ft.m_DeadZoneHeight = Mathf.Lerp(ft.m_DeadZoneHeight, jumpingDeadzoneHeight, .1f);
        //     }
        // }
    }

}
