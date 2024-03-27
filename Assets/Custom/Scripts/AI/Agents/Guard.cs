using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;

public enum PathType { LOOP, BACK_AND_FORTH }

public class Guard : MonoBehaviour, ISlowable
{
    public Transform ViewTransform;
    public List<PathNode> PathNodes;
    public PathNode RootNode;
    private PathNode[] patrolNodes;
    public PathType PathBehaviour;
    private bool hasPath = false;
    public float PatrolSpeed = 2.0f;
    public float ChaseSpeed = 4.0f;
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
        InitializePath();
    }

    private void Start()
    {
        blackboard.SetVariable(Strings.Agent, agent);
        blackboard.SetVariable(Strings.Animator, animator);
        blackboard.SetVariable(Strings.PatrolNodes, patrolNodes);
        blackboard.SetVariable(Strings.ViewCone, viewCone);
        blackboard.SetVariable(Strings.ViewTransform, ViewTransform);
        blackboard.SetVariable(Strings.PatrolSpeed, PatrolSpeed);
        blackboard.SetVariable(Strings.ChaseSpeed, ChaseSpeed);

        blackboard.SetVariable(Strings.AgentState, AgentState.PATROL);
        blackboard.SetVariable(Strings.IsSlowed, false);
        blackboard.SetVariable(Strings.SlowedMult, 1.0f);
        

        var moveTo = new BTMoveTo(blackboard);

        var detect = new BTSequence("DetectSequence",
            new BTCacheStatus(blackboard, Strings.DetectionResult, new BTDetect(blackboard)),
            new BTGotoTarget(blackboard));
        
        var path = new BTSequence("Path",
            new BTSetAgentState(blackboard, AgentState.PATROL),
            new BTGotoNextOnPath(blackboard), 
            moveTo,
            new BTStopOnPath(blackboard));
        
        var patrol = new BTParallel("Patrol", Policy.RequireAll, Policy.RequireOne,
            new BTInvert(new BTGetStatus(blackboard, Strings.DetectionResult)),

            path
        );

        
        var chase = new BTSelector("Chase Selector",
            new BTSequence("Chase",
                new BTSetAgentState(blackboard, AgentState.CHASE),
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

    private void InitializePath()
    {
        if (PathNodes.Count == 0)
        {
            return;
        }

        hasPath = true;
        
        // convert path node positions to global
        patrolNodes = PathNodes.ToArray();
        for (int i = 0; i < patrolNodes.Length; i++)
        {
            patrolNodes[i].Position = transform.position + transform.rotation * patrolNodes[i].Position;
            
            Vector3 sumRotation = patrolNodes[i].Rotation.eulerAngles + transform.rotation.eulerAngles;
            patrolNodes[i].Rotation = Quaternion.Euler(sumRotation);
        }
        
        // add starting position as node
        RootNode.Position = transform.position;
        RootNode.Rotation = transform.rotation;
        IEnumerable<PathNode> result = patrolNodes.Prepend(RootNode);
        patrolNodes = result.ToArray();
    }

    public void Slow(float _speedMult)
    {
        blackboard.SetVariable(Strings.IsSlowed, true);
        blackboard.SetVariable(Strings.SlowedMult, _speedMult);
        Debug.Log("guard slowed");
    }

    public void Unslow()
    {
        switch (blackboard.GetVariable<AgentState>(Strings.AgentState))
        {
            case AgentState.PATROL:
                agent.speed = blackboard.GetVariable<float>(Strings.PatrolSpeed);
                break;
            case AgentState.CHASE:
                agent.speed = blackboard.GetVariable<float>(Strings.ChaseSpeed);
                break;
        }
        blackboard.SetVariable(Strings.IsSlowed, false);
        blackboard.SetVariable(Strings.SlowedMult, 1.0f);
    }
}