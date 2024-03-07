using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Cinemachine.Utility;

[SaveDuringPlay][AddComponentMenu("")]
public class PlatformingCameraExtension : CinemachineExtension
{
    private bool wasMoving;
    private bool isPlayerGrounded;

    private CinemachineVirtualCamera vc;
    private CinemachineComposer c;
    private CinemachineFramingTransposer ft;

    [Range(.0f,2.0f)] public float groundedDeadzoneHeight;
    [Range(.0f, 2.0f)] public float jumpingDeadzoneHeight;

    private Action<GroundedChangedEvent> groundedChangedEventHandler;

    protected override void Awake()
    {
        base.Awake();
        groundedChangedEventHandler = _event => isPlayerGrounded = _event.IsGrounded;
        EventManager.Subscribe(typeof(GroundedChangedEvent), groundedChangedEventHandler);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventManager.Unsubscribe(typeof(GroundedChangedEvent), groundedChangedEventHandler);
    }

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase _vcam, CinemachineCore.Stage _stage, ref CameraState _state, float _deltaTime)
    {
        if (vc == null || c == null)
        {
            vc = _vcam as CinemachineVirtualCamera;
            if (vc == null)
            {
                Debug.Log("PlatformingCameraExtension: No virtual camera found");
                return;
            }
            c = vc.GetCinemachineComponent<CinemachineComposer>();
            ft = vc.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
        
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
