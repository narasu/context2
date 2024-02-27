using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Guard : MonoBehaviour
{
    private Animator animator;
    public GameObject PatrolPathRoot;
    private Transform[] pathNodes;
    private NavMeshAgent agent;

    private Transform player;
    private FieldOfView fov;

    private int currentNode;

    public float WalkSpeed, RunSpeed;
    
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        pathNodes = PatrolPathRoot.GetComponentsInChildren<Transform>();
        agent.SetDestination(pathNodes[0].position);
        player = FindObjectOfType<MovementController>().transform;
        fov = GetComponent<FieldOfView>();
    }

    private void FixedUpdate()
    {
        if (Vector3.Distance(agent.transform.position, agent.destination) <= agent.stoppingDistance)
        {
            currentNode++;
            if (currentNode == pathNodes.Length)
            {
                currentNode = 0;
            }
        }

        if (!fov.HasTarget)
        {
            agent.SetDestination(pathNodes[currentNode].transform.position);
            agent.speed = WalkSpeed;
            animator.SetBool("IsRunning", false);
        }
        else
        {
            agent.SetDestination(player.position);
            agent.speed = RunSpeed;
            animator.SetBool("IsRunning", true);
        }
    }
}
