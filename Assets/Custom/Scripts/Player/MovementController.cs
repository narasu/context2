using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovementController : MonoBehaviour
{
    private Animator animator;
    private CharacterController characterController;
    private Vector3 lastMousePosition;
    private Vector3 mouseDelta;

    private Transform camTransform;

    private float hMovementSpeed;
    private float jumpGravity, fallGravity, minJumpVelocity, maxJumpVelocity, currentCoyoteTime;
    public float WalkSpeed, SneakSpeed, Friction, MinJumpHeight, MaxJumpHeight, JumpDuration, FallDuration;
    public TriggerCheck GroundCheck;
    private Vector3 velocity;
    private static readonly int a_IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int a_IsCrouching = Animator.StringToHash("IsCrouching");

    private bool IsCrouched
    {
        get => isCrouched;
        set
        {
            if (isCrouched != value)
            {
                animator.SetBool(a_IsCrouching, value);
                isCrouched = value;
            }
        }
    }
    private bool isCrouched;
    private bool isJumping;

    private bool IsGrounded
    {
        set
        {
            if (isGrounded != value)
            {
                EventManager.Invoke(new GroundedChangedEvent(value));
                isGrounded = value;
            }
        }
    }

    private bool isGrounded;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        camTransform = Camera.main.transform;
        animator = GetComponentInChildren<Animator>();
        
        jumpGravity = 2f * MaxJumpHeight / Mathf.Pow(JumpDuration, 2);
        fallGravity = MaxJumpHeight / Mathf.Pow(FallDuration, 2);
        minJumpVelocity = Mathf.Sqrt(2 * jumpGravity * MinJumpHeight);
        maxJumpVelocity = Mathf.Sqrt(2 * jumpGravity * MaxJumpHeight);
    }

    private void Update()
    {
        Vector2 inputVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        
        Vector3 forward = new Vector3(camTransform.forward.x, .0f, camTransform.forward.z).normalized;
        Vector3 right = new Vector3(camTransform.right.x, .0f, camTransform.right.z).normalized;
        Vector3 relativeInput = forward * inputVector.y + right * inputVector.x;
        
        if (inputVector.sqrMagnitude > .0f)
        {
            float angle = Mathf.Atan2(relativeInput.x, relativeInput.z) * Mathf.Rad2Deg;
            Quaternion newRotation = Quaternion.AngleAxis(angle, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, .2f);
            animator.SetBool(a_IsWalking, true);
        }
        else
        {
            animator.SetBool(a_IsWalking, false);
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            IsCrouched = !IsCrouched;
        }
        HandleGravity();
        UpdateGrounded();
        HandleJumping();

        Vector3 newSpeed = Vector3.Lerp(new Vector3(velocity.x, .0f, velocity.z), relativeInput * (IsCrouched ? SneakSpeed : WalkSpeed), Friction);
        velocity = new Vector3(newSpeed.x, velocity.y, newSpeed.z);
        characterController.Move(velocity * Time.deltaTime);
    }
    
    private void HandleGravity()
    {
        if (velocity.y >= 0)
        {
            velocity = new Vector3(velocity.x, velocity.y - jumpGravity * Time.deltaTime, velocity.z);
        }
        else
        {
            velocity = new Vector3(velocity.x, velocity.y - fallGravity * Time.deltaTime, velocity.z);
        }
    }
    
    private void HandleJumping()
    {
        if (Input.GetKeyDown(KeyCode.Space) && GroundCheck.HasCollision)
        {
            velocity = new Vector3(velocity.x, maxJumpVelocity, velocity.z);
            isJumping = true;
            return;
        }
        
        if (Input.GetKeyUp(KeyCode.Space) && velocity.y > minJumpVelocity)
        {
            velocity = new Vector3(velocity.x, minJumpVelocity, velocity.z);
        }
    }
    
    private void UpdateGrounded()
    {
        if (!GroundCheck.HasCollision)
        {
            IsGrounded = false;
            return;
        }
        

        float distance = GroundCheck.GetShortestDistanceFromCenter();
        if (distance > .0f)
        {
            characterController.Move(Vector3.down * distance);
            if (velocity.y < .0f)
            {
                isJumping = false;
            }
        }

        if (!isJumping)
        {
            velocity = new Vector3(velocity.x, .0f, velocity.z);
            IsGrounded = true;
        }
        
    }
}
