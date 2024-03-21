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

    [Range(0.0f, 10.0f)] public float rotationSpeed;

    private Action<GroundedChangedEvent> groundedChangedEventHandler;

    protected override void Awake()
    {
        base.Awake();
        groundedChangedEventHandler = _event => isPlayerGrounded = _event.IsGrounded;
        EventManager.Subscribe(typeof(GroundedChangedEvent), groundedChangedEventHandler);
        vc = GetComponent<CinemachineVirtualCamera>();
        c = vc.GetCinemachineComponent<CinemachineComposer>();
        ft = vc.GetCinemachineComponent<CinemachineFramingTransposer>();
    }
    
    private void Update()
    {
        // Get input values from the right control stick
        float rotateX = Input.GetAxis("RightStickHorizontal");
        float rotateY = Input.GetAxis("RightStickVertical");

        // Rotate the camera around its target based on input values
        if (Mathf.Abs(rotateX) > 0.1f || Mathf.Abs(rotateY) > 0.1f)
        {
            Vector3 targetPosition = vc.Follow.position;
            transform.RotateAround(targetPosition, Vector3.up, rotateX * rotationSpeed);
            transform.RotateAround(targetPosition, transform.right, -rotateY * rotationSpeed);
        }

        // Ensure the camera always looks at the target
        transform.LookAt(vc.Follow.position);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventManager.Unsubscribe(typeof(GroundedChangedEvent), groundedChangedEventHandler);
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
        
        if (_stage == CinemachineCore.Stage.Body)
        {
            if (isPlayerGrounded)
            {
                ft.m_DeadZoneHeight = Mathf.Lerp(ft.m_DeadZoneHeight, groundedDeadzoneHeight, .1f);
            }
            else
            {
                ft.m_DeadZoneHeight = Mathf.Lerp(ft.m_DeadZoneHeight, jumpingDeadzoneHeight, .1f);
            }
        }
    }

}
