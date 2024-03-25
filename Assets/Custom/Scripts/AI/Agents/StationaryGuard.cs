using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class StationaryGuard : MonoBehaviour
{
    [SerializeField] private Transform ViewTransform;
    private BTBaseNode tree;
    
    private NavMeshAgent agent;
    private Animator animator;
    private ViewCone viewCone;
    private Blackboard blackboard = new();
    
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        viewCone = GetComponentInChildren<ViewCone>();
    }

    private void Start()
    {
        blackboard.SetVariable(Strings.Agent, agent);
        blackboard.SetVariable(Strings.Animator, animator);
        blackboard.SetVariable(Strings.ViewCone, viewCone);
        blackboard.SetVariable(Strings.ViewTransform, ViewTransform);
        blackboard.SetVariable(Strings.StartingPosition, transform.position);

        var moveTo = new BTMoveTo(blackboard);

        var detect = new BTSequence("DetectSequence", false,
            new BTCacheStatus(blackboard, Strings.DetectionResult, new BTDetect(blackboard)),
            new BTGotoTarget(blackboard));
        
        var standWatch = new BTParallel("StandWatch", Policy.RequireAll, Policy.RequireOne,
            new BTInvert(new BTGetStatus(blackboard, Strings.DetectionResult)),
            new BTSequence("StandWatch", false,
                new SetDestinationOnStart(blackboard),
                new BTSetSpeed(blackboard, 2.0f),
                moveTo,
                new BTStopOnPath(blackboard)
            )
        );
        
        var chase = new BTSelector("Chase Selector",
            new BTSequence("Chase", false,
                new BTSetSpeed(blackboard, 4.0f),
                moveTo,
                new BTTimeout(2.0f, TaskStatus.Failed, new BTGetStatus(blackboard, Strings.DetectionResult)))
        );
        
        
        var detectionSelector = new BTSelector("DetectionSelector",
            standWatch,
            chase
        );

        tree = new BTParallel("Tree", Policy.RequireAll, Policy.RequireAll,
            detect,
            detectionSelector
        );
    }

    private void FixedUpdate()
    {
        tree?.Tick();
    }

    private void OnDestroy()
    {
        tree?.OnTerminate();
    }

    private void OnTriggerEnter(Collider other)
    {
        
        // if (other.GetComponent<IPickup>() is { } pickup)
        // {
        //     EventManager.Invoke(new WeaponPickedUpEvent(pickup.PickUp()));
        // }
    }
}