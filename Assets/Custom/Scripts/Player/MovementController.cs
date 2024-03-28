using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class MovementController : MonoBehaviour, ISlowable
{
    
    public float WalkSpeed, SneakSpeed, Friction, MinJumpHeight, MaxJumpHeight, JumpDuration, FallDuration;
    public TriggerCheck GroundCheck;
    public CapsuleCollider DetectCollider;
    public GameObject YangXiaYu;
    public GameObject YusufPasha;
    
    private Animator animator;
    private static readonly int a_IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int a_IsCrouching = Animator.StringToHash("IsCrouching");
    
    private Vector3 lastMousePosition;
    private Vector3 mouseDelta;
    private GameInputActions inputActions;
    
    private CharacterController characterController;
    private Transform camTransform;
    private float hMovementSpeed;
    private float jumpGravity, fallGravity, minJumpVelocity, maxJumpVelocity, currentCoyoteTime;
    private Vector3 velocity;
    private bool isSlowed;
    private float slowedMult = 1.0f;
    //sound making
    [SerializeField] private  AudioClip walkSoundClip;
    private bool IsCrouched
    {
        get => isCrouched;
        set
        {
            if (isCrouched != value)
            {
                if (value)
                {
                    DetectCollider.height = .8f;
                    DetectCollider.center = new Vector3(0, 0.4f, 0);
                }
                else
                {
                    DetectCollider.height = 2.0f;
                    DetectCollider.center = new Vector3(0, 1.0f, 0);
                }
                animator.SetBool(a_IsCrouching, value);
                isCrouched = value;
            }
        }
    }
    private bool isCrouched;

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
    private bool isJumping;
    private bool isThrowing;
    private Action<ThrowStartEvent> throwStartEventHandler;
    private Action<ThrowEndEvent> throwEndEventHandler;
    private Vector3 startPosition;
    private Quaternion startRotation;

    private void Awake()
    {
        ServiceLocator.Provide(Strings.Player, transform);
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        camTransform = Camera.main.transform;
        animator = GetComponentInChildren<Animator>();
        
        jumpGravity = 2f * MaxJumpHeight / Mathf.Pow(JumpDuration, 2);
        fallGravity = MaxJumpHeight / Mathf.Pow(FallDuration, 2);
        minJumpVelocity = Mathf.Sqrt(2 * jumpGravity * MinJumpHeight);
        maxJumpVelocity = Mathf.Sqrt(2 * jumpGravity * MaxJumpHeight);
        inputActions = new GameInputActions();
        
        throwStartEventHandler = _ => isThrowing = true;
        throwEndEventHandler = _ => isThrowing = false;
        
        startPosition = transform.position;
        startRotation = transform.rotation;
    }
    
    private void Update()
    {
        Vector2 inputVector = inputActions.Player.Walk.ReadValue<Vector2>();
        
        Vector3 forward = new Vector3(camTransform.forward.x, .0f, camTransform.forward.z).normalized;
        Vector3 right = new Vector3(camTransform.right.x, .0f, camTransform.right.z).normalized;
        Vector3 relativeInput = forward * inputVector.y + right * inputVector.x;
        
        if (inputVector.sqrMagnitude > .0f)
        {
            float angle = Mathf.Atan2(relativeInput.x, relativeInput.z) * Mathf.Rad2Deg;
            Quaternion newRotation = Quaternion.AngleAxis(angle, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, .2f);
           // SoundManager.instance.PlaySoundClip(walkSoundClip, transform, 1f);
            animator.SetBool(a_IsWalking, true);
        }
        else
        {
            animator.SetBool(a_IsWalking, false);
            
        }
        
        HandleGravity();
        UpdateGrounded();

        
        Vector3 newSpeed = Vector3.Lerp(new Vector3(velocity.x, .0f, velocity.z), relativeInput * ((IsCrouched ? SneakSpeed : WalkSpeed) * slowedMult), Friction);
        velocity = new Vector3(newSpeed.x, velocity.y, newSpeed.z);
        characterController.Move(velocity * Time.deltaTime);
        //sound making
        
       
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Jump.performed += OnJump;
        inputActions.Player.Jump.canceled += OnJumpRelease;
        inputActions.Player.Crouch.performed += OnCrouch;

        switch (GameManager.Instance.currentRun)
        {
            case 0:
                YangXiaYu.SetActive(true);
                animator = YangXiaYu.GetComponent<Animator>();
                YusufPasha.SetActive(false);
                break;
            case 1:
                YangXiaYu.SetActive(false);
                YusufPasha.SetActive(true);
                animator = YusufPasha.GetComponent<Animator>();
                break;
        }
        
        GetComponent<ThrowHandler>().enabled = true;
        
        EventManager.Subscribe(typeof(ThrowStartEvent), throwStartEventHandler);
        EventManager.Subscribe(typeof(ThrowEndEvent), throwEndEventHandler);
    }

    private void OnDisable()
    {
        inputActions.Disable();
        inputActions.Player.Jump.performed -= OnJump;
        inputActions.Player.Jump.canceled -= OnJumpRelease;
        inputActions.Player.Crouch.performed -= OnCrouch;

        GetComponent<ThrowHandler>().enabled = false;
        
        EventManager.Unsubscribe(typeof(ThrowStartEvent), throwStartEventHandler);
        EventManager.Unsubscribe(typeof(ThrowEndEvent), throwEndEventHandler);
    }
    
    #region MOVEMENT_FUNCTIONS
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
    
    private void OnCrouch(InputAction.CallbackContext _ctx)
    {
        IsCrouched = !IsCrouched;
    }

    private void OnJump(InputAction.CallbackContext _ctx)
    {
        if (GroundCheck.HasCollision)
        {
            velocity = new Vector3(velocity.x, maxJumpVelocity, velocity.z);
            isJumping = true;
        }
    }

    private void OnJumpRelease(InputAction.CallbackContext _ctx)
    {
        if (velocity.y > minJumpVelocity)
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
    #endregion

    public void Slow(float _speedMult)
    {
        isSlowed = true;
        slowedMult = _speedMult;
    }

    public void Unslow()
    {
        isSlowed = false;
        slowedMult = 1.0f;
    }

    public void Reset()
    {
        isSlowed = false;
        isJumping = false;
        IsCrouched = false;
        IsGrounded = true;
        
        slowedMult = 1.0f;
        velocity = Vector3.zero;
        transform.SetPositionAndRotation(startPosition, startRotation);
    }
}
