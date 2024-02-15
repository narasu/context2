using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public GameObject LookAt;
    private Animator animator;
    private CharacterController characterController;
    private Vector3 lastMousePosition;
    private Vector3 mouseDelta;

    private Transform camTransform;

    private float hMovementSpeed;
    public float WalkSpeed, Friction;
    private Vector3 velocity;
    
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        camTransform = Camera.main.transform;
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        Vector2 inputVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        
        Vector3 forward = new Vector3(camTransform.forward.x, .0f, camTransform.forward.z).normalized;
        Vector3 right = new Vector3(camTransform.right.x, .0f, camTransform.right.z).normalized;
        Vector3 relativeInput = forward * inputVector.y + right * inputVector.x;
        
        
        hMovementSpeed = WalkSpeed;
            
        if (inputVector.sqrMagnitude > .0f)
        {
            float angle = Mathf.Atan2(relativeInput.x, relativeInput.z) * Mathf.Rad2Deg;
            Quaternion newRotation = Quaternion.AngleAxis(angle, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, .2f);
            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }

        Vector3 newSpeed = Vector3.Lerp(velocity, relativeInput * hMovementSpeed, Friction);
        velocity = new Vector3(newSpeed.x, -9.81f, newSpeed.z);
        
        
        characterController.Move(velocity * Time.deltaTime);
        // mouseDelta = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * (Time.deltaTime * 48.0f);
        // transform.Rotate(Vector3.up, mouseDelta.x);
        //
        // LookAt.transform.Translate(Vector3.up * mouseDelta.y);
        // LookAt.transform.localPosition =
        //     new Vector3(.0f, Mathf.Clamp(LookAt.transform.localPosition.y, -2.0f, 2.0f), LookAt.transform.localPosition.z);
    }
}
