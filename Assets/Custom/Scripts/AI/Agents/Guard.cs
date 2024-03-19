using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;

public enum PathType { LOOP, BACK_AND_FORTH }
public class Guard : MonoBehaviour
{
    public Transform ViewTransform;
    public List<PathNode> PathNodes;
    public PathType PathBehaviour;
    public float PatrolSpeed, ChaseSpeed;
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
        blackboard.SetVariable(Strings.PatrolNodes, PathNodes);
        blackboard.SetVariable(Strings.ViewCone, viewCone);
        blackboard.SetVariable(Strings.ViewTransform, ViewTransform);

        var moveTo = new BTMoveTo(blackboard);

        var detect = new BTSequence("DetectSequence", false,
            new BTCacheStatus(blackboard, Strings.DetectionResult, new BTDetect(blackboard)),
            new BTSetDestinationOnTarget(blackboard));
        
        var patrol = new BTParallel("Patrol", Policy.RequireAll, Policy.RequireOne,
            new BTInvert(new BTGetStatus(blackboard, Strings.DetectionResult)),
            new BTSequence("path sequence", false,
                new BTSetSpeed(blackboard, 2.0f),
                new BTPath(blackboard, moveTo),
                new BTLookAround(blackboard)
            )
        );
        
        var chase = new BTSelector("Chase Selector",
            new BTSequence("Chase", false,
                new BTSetSpeed(blackboard, 4.0f),
                moveTo,
                new BTTimeout(2.0f, TaskStatus.Failed, new BTGetStatus(blackboard, Strings.DetectionResult)))
        );
        
        
        var detectionSelector = new BTSelector("DetectionSelector",
            patrol,
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
}